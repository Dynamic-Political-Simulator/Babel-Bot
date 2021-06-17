using BabelBot.CustomPreconditions;
using BabelDatabase;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace BabelBot.Modules
{
    public class VoteModule : InteractiveBase<SocketCommandContext>
    {
        private readonly BabelContext _context;

        public VoteModule(BabelContext context)
        {
            _context = context;
        }

        string ConvertVoteTypeToString(VoteType type)
        {
            switch (type)
            {
                case VoteType.MAJORITY:
                    return "Majority";
                case VoteType.TWOTHIRD:
                    return "Two Thirds";
                default:
                    return "Undefined";
            }
        }

        string[] formats = new[] { "h'h'", "h'h'm'm'", "m'm'", "%m" }; // should ensure that XhMmin format works
        TimeSpan getTimeSpan(string time)
        {
            TimeSpan timeSpan;
            if (TimeSpan.TryParseExact(time, formats, CultureInfo.InvariantCulture, out timeSpan))
            {
                return timeSpan;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        [Command("makevote", RunMode = RunMode.Async)]
        [RequireNoProfile]
        public async Task CreateProfile(string type = null, string time = null, string link = null, [Remainder] string title = null)
        {
            if (type == null)
            {
                List<IUserMessage> conversation = new List<IUserMessage>();
                conversation.Add(Context.Message);
                conversation.Add(await ReplyAsync("Welcome to the vote wizard. Type 'cancel' any time to exit.\nWhat type is the vote? (Valid types: Majority, Twothird)"));
                IUserMessage reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                conversation.Add(reply);
                VoteType voteType; // Placeholder value.
                while (!Enum.TryParse(reply.Content, true, out voteType))
                {
                    if (reply.Content == "cancel")
                    {
                        await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                        return;
                    }
                    conversation.Add(await ReplyAsync("Sorry, that isn't a valid type."));
                    reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                    conversation.Add(reply);
                }
                conversation.Add(await ReplyAsync("How long should the vote last? (Please format it as ??h??m where ? are numbers)"));
                reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                conversation.Add(reply);
                TimeSpan span = getTimeSpan(reply.Content);
                while (span == TimeSpan.Zero)
                {
                    if (reply.Content == "cancel")
                    {
                        await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                        return;
                    }
                    conversation.Add(await ReplyAsync("Sorry, that isn't a valid time span."));
                    reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                    conversation.Add(reply);
                    span = getTimeSpan(reply.Content);
                }
                conversation.Add(await ReplyAsync("What should the link/description of the thing being voted on be?"));
                reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                conversation.Add(reply);
                if (reply.Content == "cancel")
                {
                    await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                    return;
                }
                string linke = reply.Content;
                conversation.Add(await ReplyAsync("What should the title of the vote be?"));
                reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                conversation.Add(reply);
                if (reply.Content == "cancel")
                {
                    await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                    return;
                }
                string titlee = reply.Content;

                string timeStr = "In ";
                if (span.Hours != 0)
                {
                    timeStr += span.Hours + " hour";
                    if (span.Hours > 1) timeStr += "s";
                    timeStr += " ";
                }
                if (span.Minutes != 0)
                {
                    timeStr += span.Minutes + " minute";
                    if (span.Minutes > 1) timeStr += "s";
                    timeStr += " ";
                }
                EmbedBuilder emb = new EmbedBuilder()
                    .WithTitle(titlee)
                    .WithDescription(linke)
                    .AddField("Ends:", timeStr)
                    .AddField("Type: ", ConvertVoteTypeToString(voteType))
                    .WithColor(Color.LightGrey);
                RestUserMessage mid = await Context.Channel.SendMessageAsync("", false, emb.Build());
                emb.WithFooter("Message ID: " + mid.Id);
                await mid.ModifyAsync((x) =>
                {
                    x.Embed = emb.Build();
                });
                await mid.AddReactionAsync(new Emoji("✅"));
                await mid.AddReactionAsync(new Emoji("❌"));
                await mid.AddReactionAsync(new Emoji("🇴"));
                VoteMessage message = new VoteMessage();
                message.MessageId = mid.Id;
                message.CreatorId = Context.User.Id;
                message.ChannelId = Context.Channel.Id;
                message.Type = (int)voteType;
                message.EndTime = DateTime.Now.AddHours(span.Hours).AddMinutes(span.Minutes).ToFileTime();
                _context.VoteMessages.Add(message);
                await _context.SaveChangesAsync();
                await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
            }
            else if (time == null || title == null || link == null)
            {
                await ReplyAsync("Missing parameters, run without parameters to use the wizard.");
            }
            else
            {
                VoteType voteType; // Placeholder value.
                if (Enum.TryParse(type, true, out voteType))
                {
                    if (Enum.IsDefined(typeof(VoteType), voteType) | voteType.ToString().Contains(","))
                    {
                        // Vote type is certified epic :)
                        TimeSpan timeSpan = getTimeSpan(time);
                        if (timeSpan == TimeSpan.Zero)
                        {
                            await ReplyAsync("You have specified an invalid time.");
                        }
                        else
                        {
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
                            EmbedBuilder emb = new EmbedBuilder()
                                .WithTitle(title)
                                .WithDescription(link)
                                .AddField("Ends:", timeStr)
                                .AddField("Type: ", ConvertVoteTypeToString(voteType))
                                .WithColor(Color.LightGrey);
                            RestUserMessage mid = await Context.Channel.SendMessageAsync("", false, emb.Build());
                            emb.WithFooter("Message ID: " + mid.Id);
                            await mid.ModifyAsync((x) =>
                            {
                                x.Embed = emb.Build();
                            });
                            await mid.AddReactionAsync(new Emoji("✅"));
                            await mid.AddReactionAsync(new Emoji("❌"));
                            await mid.AddReactionAsync(new Emoji("🇴"));
                            VoteMessage message = new VoteMessage();
                            message.MessageId = mid.Id;
                            message.CreatorId = Context.User.Id;
                            message.ChannelId = Context.Channel.Id;
                            message.Type = (int)voteType;
                            message.EndTime = DateTime.Now.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).ToFileTime();
                            _context.VoteMessages.Add(message);
                            await _context.SaveChangesAsync();
                            await Context.Message.DeleteAsync();
                        }
                    }
                    else
                    {
                        await ReplyAsync("You specified an invalid vote type.");
                    }
                }
                else
                {
                    await ReplyAsync("You specified an invalid vote type.");
                }
            }
        }
    }
}
