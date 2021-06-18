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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

namespace BabelBot.Modules
{
    public class VoteModule : InteractiveBase<SocketCommandContext>
    {
        private readonly BabelContext _context;

        // Provides the number emotes. Usage: numberEmotes[numberYouWant]
        private readonly Emoji[] numberEmotes = new Emoji[] { new Emoji("0️⃣"), new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣"), new Emoji("4️⃣"), new Emoji("5️⃣"), new Emoji("6️⃣"), new Emoji("7️⃣"), new Emoji("8️⃣"), new Emoji("9️⃣") };

        // Types with multiple options
        private readonly VoteType[] multipleOptions = new VoteType[] {
            VoteType.FPTP,
            VoteType.TWOROUND
        };


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
                case VoteType.FPTP:
                    return "First Past the Post";
                case VoteType.TWOROUND:
                    return "Two Round";
                case VoteType.TWOROUNDFINAL:
                    return "Two Round Run-off";
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

        bool isMultipleOption(VoteType type)
        {
            return Array.Exists(multipleOptions, (x) => x == type);
        }

        // Parses the type using regex that allows for a degree of variation on the words
        VoteType? parseType(string type)
        {
            type = type.ToLower(); // Gets rid of the capitalisation, this is not a case sensitive server :rage:
            switch (type)
            {
                case string s when new Regex(@"(majority)").IsMatch(type): // This is art.
                    return VoteType.MAJORITY;
                case string s when new Regex(@"(two( |-)?thirds?)").IsMatch(type):
                    return VoteType.TWOTHIRD;
                case string s when new Regex(@"(f(irst)?(-| )?p(ast)?(-| )?t(he)?(-| )?p(ost)?)").IsMatch(type):
                    return VoteType.FPTP;
                case string s when new Regex(@"(two( |-)?rounds?)").IsMatch(type):
                    return VoteType.TWOROUND;
                default:
                    return null;
            }
        }

        // TryParse variation of the function so you don't have to write a check for null every time you use it
        bool parseType(string type, out VoteType res)
        {
            VoteType? tmp = parseType(type); // Call the original function to get the actual type
            if (tmp == null) // Test if it found something
            {
                res = 0; // Feed it some bogus 
                return false;
            }
            else
            {
                res = (VoteType)tmp;
                return true;
            }
        }

        [Command("makevote", RunMode = RunMode.Async)]
        [RequireNoProfile]
        public async Task CreateProfile(string type = null, string time = null, [Remainder] string text = null)
        {
            if (type == null)
            {
                List<IUserMessage> conversation = new List<IUserMessage>();
                conversation.Add(Context.Message);
                conversation.Add(await ReplyAsync("Welcome to the vote wizard. Type 'cancel' any time to exit.\nWhat type is the vote? (Valid types: Majority, Twothird)"));
                IUserMessage reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                conversation.Add(reply);
                VoteType voteType; // Placeholder value.
                while (!parseType(reply.Content, out voteType))
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
                await mid.AddReactionsAsync(new IEmote[] {
                                    new Emoji("✅"), // yay
                                    new Emoji("❌"), // nay
                                    new Emoji("🇴") // abstain
                                });
                VoteMessage message = new VoteMessage();
                message.MessageId = mid.Id;
                message.CreatorId = Context.User.Id;
                message.ChannelId = Context.Channel.Id;
                message.Type = (int)voteType;
                message.EndTime = DateTime.Now.AddHours(span.Hours).AddMinutes(span.Minutes).ToFileTime();
                message.TimeSpan = span.Ticks;
                _context.VoteMessages.Add(message);
                await _context.SaveChangesAsync();
                await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
            }
            else if (time == null || text == null)
            {
                await ReplyAsync("Missing parameters, run without parameters to use the wizard.");
            }
            else
            {
                string[] textArgs = text.Split("|");
                VoteType voteType; // Placeholder value.
                if (parseType(type, out voteType))
                {
                    // Vote type is certified epic :)
                    TimeSpan timeSpan = getTimeSpan(time);
                    if (timeSpan == TimeSpan.Zero)
                    {
                        await ReplyAsync("You have specified an invalid time.");
                    }
                    else
                    {
                        if (!isMultipleOption(voteType))
                        {
                            // textArg format: [Title] | [Link/Desc]
                            if (textArgs.Length != 2)
                            {
                                await ReplyAsync("You have specified an invalid amount of text arguments");
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
                                    .WithTitle(textArgs[0])
                                    .WithDescription(textArgs[1])
                                    .AddField("Ends:", timeStr)
                                    .AddField("Type: ", ConvertVoteTypeToString(voteType))
                                    .WithColor(Color.LightGrey);
                                RestUserMessage mid = await Context.Channel.SendMessageAsync("", false, emb.Build());
                                emb.WithFooter("Message ID: " + mid.Id);
                                await mid.ModifyAsync((x) =>
                                {
                                    x.Embed = emb.Build();
                                });
                                await mid.AddReactionsAsync(new IEmote[] {
                                    new Emoji("✅"), // yay
                                    new Emoji("❌"), // nay
                                    new Emoji("🇴") // abstain
                                });
                                VoteMessage message = new VoteMessage();
                                message.MessageId = mid.Id;
                                message.CreatorId = Context.User.Id;
                                message.ChannelId = Context.Channel.Id;
                                message.Type = (int)voteType;
                                message.EndTime = DateTime.Now.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).ToFileTime();
                                message.TimeSpan = timeSpan.Ticks;
                                _context.VoteMessages.Add(message);
                                await _context.SaveChangesAsync();
                                await Context.Message.DeleteAsync();
                            }
                        }
                        else
                        {
                            // textArg format: [Title] | [Candidate 1] | [Candidate 2] | ... | [Candidate N], where N <= 9
                            // If at some point we end up needing more than 9 candidates then I guess it'll automatically go to anon mode
                            if (textArgs.Length < 2 || textArgs.Length > 10)
                            {
                                await ReplyAsync("You have specified an invalid amount of text arguments");
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
                                    .WithTitle(textArgs[0])
                                    .AddField("Ends:", timeStr)
                                    .AddField("Type: ", ConvertVoteTypeToString(voteType))
                                    .WithColor(Color.LightGrey);
                                for (int x = 1; x < textArgs.Length; x++)
                                {
                                    emb.AddField("Option #" + x + ":", textArgs[x], true);
                                }
                                RestUserMessage mid = await Context.Channel.SendMessageAsync("", false, emb.Build());
                                emb.WithFooter("Message ID: " + mid.Id);
                                await mid.ModifyAsync((x) =>
                                {
                                    x.Embed = emb.Build();
                                });
                                await mid.AddReactionsAsync(numberEmotes.AsSpan(1, textArgs.Length - 1).ToArray());
                                VoteMessage message = new VoteMessage();
                                message.MessageId = mid.Id;
                                message.CreatorId = Context.User.Id;
                                message.ChannelId = Context.Channel.Id;
                                message.Type = (int)voteType;
                                message.EndTime = DateTime.Now.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).ToFileTime();
                                message.TimeSpan = timeSpan.Ticks;
                                _context.VoteMessages.Add(message);
                                await _context.SaveChangesAsync();
                                await Context.Message.DeleteAsync();
                            }
                        }
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
