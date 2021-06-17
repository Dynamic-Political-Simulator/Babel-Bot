using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Timers;
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
                    _context.Remove(vms);
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
                            .AddField("Ends:", timeStr)
                            .AddField("Type: ", og.Fields[1].Value)
                            .WithColor(og.Color == null ? (Color)og.Color : Color.LightGrey);
                    await msg.ModifyAsync((e) =>
                        {
                            e.Embed = emb.Build();
                        });
                }
            }

            await _context.SaveChangesAsync();
            return;
        }

        private async Task HandleReactAsync(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel channel, SocketReaction react)
        {
            if (react.UserId == _discordSocketClient.CurrentUser.Id) return;
            if (_context.VoteMessages.Find(msg.Id) != null
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
            return;
        }
    }
}