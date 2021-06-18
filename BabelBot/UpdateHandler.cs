using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Timers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Discord.Rest;
using BabelDatabase;

namespace BabelBot
{
    // Class for various non-command related updates.
    public class UpdateHandler
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly BabelContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly Timer timer;
        // Provides the number emotes. Usage: numberEmotes[numberYouWant]
        private readonly Emoji[] numberEmotes = new Emoji[] { new Emoji("0️⃣"), new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣"), new Emoji("4️⃣"), new Emoji("5️⃣"), new Emoji("6️⃣"), new Emoji("7️⃣"), new Emoji("8️⃣"), new Emoji("9️⃣") };

        // Types with multiple options
        private readonly VoteType[] multipleOptions = new VoteType[] {
            VoteType.FPTP,
            VoteType.TWOROUND
        };

        bool isMultipleOption(VoteType type)
        {
            return Array.Exists(multipleOptions, (x) => x == type);
        }

        public UpdateHandler(DiscordSocketClient discordSocketClient, IServiceProvider serviceProvider)
        {
            _discordSocketClient = discordSocketClient;
            _serviceProvider = serviceProvider;

            _context = serviceProvider.GetService<BabelContext>();

            // Setup timer
            timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            timer.Elapsed += async (s, e) => { await HandleUpdateAsync(); };
            timer.AutoReset = true;
            timer.Enabled = true;

            _discordSocketClient.ReactionAdded += HandleReactAsync;
        }

        private async Task HandleUpdateAsync()
        {
            // This function should be used for anything that needs updating
            // TODO: If anything other than votes need updating, make this modular.

            // Check and update votes
            foreach (VoteMessage vms in _context.VoteMessages)
            {
                // This is disgusting.
                RestUserMessage msg = (RestUserMessage)await ((ITextChannel)_discordSocketClient.GetChannel(vms.ChannelId)).GetMessageAsync(vms.MessageId);
                if (vms.EndTime < DateTime.Now.ToFileTime())
                {
                    // await mid.AddReactionAsync(new Emoji("✅"));
                    // await mid.AddReactionAsync(new Emoji("❌"));
                    // await mid.AddReactionAsync(new Emoji("🇴"));
                    if (!isMultipleOption((VoteType)vms.Type))
                    {
                        int yesVotes = 0;
                        int noVotes = 0;
                        int abstainVotes = 0;
                        try
                        {
                            yesVotes = msg.Reactions[new Emoji("✅")].ReactionCount - 1;
                            noVotes = msg.Reactions[new Emoji("❌")].ReactionCount - 1;
                            abstainVotes = msg.Reactions[new Emoji("🇴")].ReactionCount - 1;
                        }
                        catch (Exception) { }

                        bool pass = false;
                        switch ((VoteType)vms.Type)
                        {
                            case VoteType.MAJORITY:
                                pass = yesVotes > noVotes;
                                break;
                            case VoteType.TWOTHIRD:
                                pass = (yesVotes / ((float)yesVotes + noVotes)) > 0.66f;
                                break;
                            default:
                                pass = false;
                                Console.WriteLine("Undefined VoteType: " + vms.Type);
                                break;
                        }
                        if (pass)
                        {
                            Embed og = null;
                            foreach (Embed e in msg.Embeds)
                            {
                                og = e;
                                break; // I fucking hate IReadOnlyCollection.
                            }
                            EmbedBuilder emb = new EmbedBuilder()
                                    .WithTitle(og.Title)
                                    .WithDescription(og.Description)
                                    .AddField("Result:", "Passed with " + yesVotes + " for, " + noVotes + " against and " + abstainVotes + " abstaining.")
                                    .WithColor(Color.DarkGreen);
                            await msg.ModifyAsync((e) =>
                            {
                                e.Embed = emb.Build();
                            });
                            try
                            {
                                await _discordSocketClient.GetUser(vms.CreatorId).SendMessageAsync(embed: new EmbedBuilder()
                                        .WithTitle("Vote " + og.Title + " has passed.")
                                        .WithDescription($"[Jump]({msg.GetJumpUrl()})")
                                        .WithColor(Color.DarkGreen)
                                        .Build());
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            Embed og = null;
                            foreach (Embed e in msg.Embeds)
                            {
                                og = e;
                                break;
                            }
                            EmbedBuilder emb = new EmbedBuilder()
                                    .WithTitle(og.Title)
                                    .WithDescription(og.Description)
                                    .AddField("Result:", "Failed with " + yesVotes + " for, " + noVotes + " against and " + abstainVotes + " abstaining.")
                                    .WithColor(Color.DarkRed);
                            await msg.ModifyAsync((e) =>
                            {
                                e.Embed = emb.Build();
                            });
                            try
                            {
                                await _discordSocketClient.GetUser(vms.CreatorId).SendMessageAsync(embed: new EmbedBuilder()
                                        .WithTitle("Vote " + og.Title + " has failed.")
                                        .WithDescription($"[Jump]({msg.GetJumpUrl()})")
                                        .WithColor(Color.DarkRed)
                                        .Build());
                            }
                            catch (Exception) { }
                        }
                    }
                    else
                    {
                        List<int> votes = new List<int>();
                        for (int x = 1; x < numberEmotes.Length; x++)
                        {
                            try
                            {
                                int tmp = msg.Reactions[numberEmotes[x]].ReactionCount - 1;
                                votes.Add(tmp);
                            }
                            catch (Exception)
                            {
                                break; // If we get an exception, it must mean we have gone through all the existing options, so we should just end the loop.
                            }
                        }
                        int winner = 0;
                        switch ((VoteType)vms.Type)
                        {
                            case VoteType.FPTP:
                                winner = votes.IndexOf(votes.Max());
                                break;
                            case VoteType.TWOROUND:
                                int a = votes.IndexOf(votes.Max());
                                if (votes[a] / (float)votes.Sum() > 0.5f)
                                { // We have a majority
                                    winner = a;
                                    break;
                                }
                                votes.Remove(a);
                                int b = votes.IndexOf(votes.Max());
                                if (b >= a) b++; // Jank, but makes sense.
                                // start run-off with a and b
                                Embed ogg = null;
                                foreach (Embed e in msg.Embeds)
                                {
                                    ogg = e;
                                    break;
                                }
                                TimeSpan timeSpan = new TimeSpan(vms.TimeSpan);
                                string timeStr = "In ";
                                if (timeSpan.Hours != 0)
                                {
                                    timeStr += timeSpan.Hours + " hour";
                                    if (timeSpan.Hours > 1) timeStr += "s";
                                    timeStr += " ";
                                }
                                if (timeSpan.Minutes != 0)
                                {
                                    timeStr += timeSpan.Minutes + " minute";
                                    if (timeSpan.Minutes > 1) timeStr += "s";
                                    timeStr += " ";
                                }
                                EmbedBuilder embo = new EmbedBuilder()
                                    .WithTitle(ogg.Title)
                                    .AddField("Ends:", timeStr)
                                    .AddField("Type: ", "Two Round Runoff")
                                    .WithColor(Color.LightGrey);
                                for (int x = 2; x < ogg.Fields.Length; x++)
                                {
                                    embo.AddField("Option #" + x + ":", ogg.Fields[x].Value, true);
                                }
                                RestUserMessage mid = (RestUserMessage)await ((ITextChannel)_discordSocketClient.GetChannel(vms.ChannelId)).SendMessageAsync("", false, embo.Build());
                                embo.WithFooter("Message ID: " + mid.Id);
                                await mid.ModifyAsync((x) =>
                                {
                                    x.Embed = embo.Build();
                                });
                                await mid.AddReactionsAsync(numberEmotes.AsSpan(1, ogg.Fields.Length - 2).ToArray());
                                VoteMessage message = new VoteMessage();
                                message.MessageId = mid.Id;
                                message.CreatorId = vms.CreatorId;
                                message.ChannelId = vms.ChannelId;
                                message.Type = (int)VoteType.TWOROUNDFINAL;
                                message.EndTime = DateTime.Now.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).ToFileTime();
                                _context.VoteMessages.Add(message);

                                embo = new EmbedBuilder()
                                .WithTitle(ogg.Title)
                                .AddField("Result:", ogg.Fields[a + 2].Value + " and " + ogg.Fields[b + 2].Value + " continue onto the runoff vote.")
                                .WithColor(Color.DarkTeal);
                                await msg.ModifyAsync((e) =>
                                {
                                    e.Embed = embo.Build();
                                });
                                try
                                {
                                    await _discordSocketClient.GetUser(vms.CreatorId).SendMessageAsync(embed: new EmbedBuilder()
                                            .WithTitle(ogg.Fields[a + 2].Value + " and " + ogg.Fields[b + 2].Value + " have continued onto the runoff vote.")
                                            .WithDescription($"[Jump]({msg.GetJumpUrl()})")
                                            .WithColor(Color.DarkTeal)
                                            .Build());
                                }
                                catch (Exception) { }
                                _context.Remove(vms);
                                continue;
                            case VoteType.TWOROUNDFINAL:
                                winner = votes.IndexOf(votes.Max());
                                break;
                            default:
                                winner = 0;
                                break;
                        }
                        Embed og = null;
                        foreach (Embed e in msg.Embeds)
                        {
                            og = e;
                            break; // I fucking hate IReadOnlyCollection.
                        }
                        EmbedBuilder emb = new EmbedBuilder()
                                .WithTitle(og.Title)
                                .AddField("Result:", og.Fields[winner + 2].Value + " has won with " + votes[winner] + " votes.")
                                .WithColor(Color.DarkTeal);
                        await msg.ModifyAsync((e) =>
                        {
                            e.Embed = emb.Build();
                        });
                        try
                        {
                            await _discordSocketClient.GetUser(vms.CreatorId).SendMessageAsync(embed: new EmbedBuilder()
                                    .WithTitle(og.Fields[winner + 2].Value + " has won the vote " + og.Title + ".")
                                    .WithDescription($"[Jump]({msg.GetJumpUrl()})")
                                    .WithColor(Color.DarkTeal)
                                    .Build());
                        }
                        catch (Exception) { }
                    }
                    _context.Remove(vms);
                }
                else
                {
                    if (!isMultipleOption((VoteType)_context.VoteMessages.Find(msg.Id).Type))
                    {
                        TimeSpan timeSpan = new TimeSpan(vms.EndTime - DateTime.Now.ToFileTime());
                        string timeStr = "In ";
                        if (timeSpan.Hours != 0)
                        {
                            timeStr += timeSpan.Hours + " hour";
                            if (timeSpan.Hours > 1) timeStr += "s";
                            timeStr += " ";
                        }
                        if (timeSpan.Minutes != 0)
                        {
                            timeStr += timeSpan.Minutes + " minute";
                            if (timeSpan.Minutes > 1) timeStr += "s";
                            timeStr += " ";
                        }
                        if (timeStr == "In ") timeStr = "In 1 minute"; // I am not wrong.
                        Embed og = null;
                        foreach (Embed e in msg.Embeds)
                        {
                            og = e;
                            break;
                        }
                        EmbedBuilder emb = new EmbedBuilder()
                                .WithTitle(og.Title)
                                .WithDescription(og.Description)
                                .AddField("Ends:", timeStr)
                                .AddField("Type: ", og.Fields[1].Value)
                                .WithColor(og.Color == null ? (Color)og.Color : Color.LightGrey);
                        await msg.ModifyAsync((e) =>
                            {
                                e.Embed = emb.Build();
                            });
                    }
                    else
                    {
                        TimeSpan timeSpan = new TimeSpan(vms.EndTime - DateTime.Now.ToFileTime());
                        string timeStr = "In ";
                        if (timeSpan.Hours != 0)
                        {
                            timeStr += timeSpan.Hours + " hour";
                            if (timeSpan.Hours > 1) timeStr += "s";
                            timeStr += " ";
                        }
                        if (timeSpan.Minutes != 0)
                        {
                            timeStr += timeSpan.Minutes + " minute";
                            if (timeSpan.Minutes > 1) timeStr += "s";
                            timeStr += " ";
                        }
                        if (timeStr == "In ") timeStr = "In 1 minute"; // I am not wrong.
                        Embed og = null;
                        foreach (Embed e in msg.Embeds)
                        {
                            og = e;
                            break;
                        }
                        EmbedBuilder emb = new EmbedBuilder()
                                .WithTitle(og.Title)
                                .WithDescription(og.Description)
                                .WithFooter(og.Footer.Value.Text)
                                .AddField("Ends:", timeStr)
                                .AddField("Type: ", og.Fields[1].Value)
                                .WithColor(og.Color == null ? (Color)og.Color : Color.LightGrey);
                        for (int x = 2; x < og.Fields.Length; x++)
                        {
                            emb.AddField(og.Fields[x].Name, og.Fields[x].Value, og.Fields[x].Inline);
                        }
                        await msg.ModifyAsync((e) =>
                            {
                                e.Embed = emb.Build();
                            });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return;
        }

        private async Task HandleReactAsync(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel channel, SocketReaction react)
        {
            if (react.UserId == _discordSocketClient.CurrentUser.Id) return;
            VoteMessage vms = _context.VoteMessages.Find(msg.Id);
            if (vms != null)
            {
                if (!isMultipleOption((VoteType)vms.Type)
                    && (react.Emote.Name == (new Emoji("✅")).Name
                        || react.Emote.Name == (new Emoji("❌")).Name
                        || react.Emote.Name == (new Emoji("🇴")).Name)) // Yes, I wrote this conditional purely to allow meme reacts on votes.
                {
                    IUserMessage message = await msg.GetOrDownloadAsync();
                    ulong reactor = react.UserId;
                    if (react.Emote.Name != (new Emoji("✅")).Name)
                    {
                        await message.RemoveReactionAsync(new Emoji("✅"), reactor);
                    }
                    if (react.Emote.Name != (new Emoji("❌")).Name)
                    {
                        await message.RemoveReactionAsync(new Emoji("❌"), reactor);
                    }
                    if (react.Emote.Name != (new Emoji("🇴")).Name)
                    {
                        await message.RemoveReactionAsync(new Emoji("🇴"), reactor);
                    }
                }
                else
                {
                    Emoji reacte = numberEmotes.FirstOrDefault(x => x.Name == react.Emote.Name);
                    if (reacte != null)
                    {
                        IUserMessage message = await msg.GetOrDownloadAsync();
                        ulong reactor = react.UserId;
                        List<Emoji> temp = numberEmotes.ToList();
                        temp.Remove(reacte);
                        await message.RemoveReactionsAsync(_discordSocketClient.GetUser(reactor), temp.ToArray()); // WHY THE FUCK DOES ID NOT WORK FOR THIS EVEN THOUGH IT WORKS FOR NON-BULK REACT REMOVAL??? DISCORD.NET PLEAAAAAAAAAAAASE
                    }
                }
            }
            return;
        }
    }
}