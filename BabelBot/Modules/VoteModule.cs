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
using System.Linq;

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
                case string s when new Regex(@"^(majority)\b").IsMatch(type): // This is art.
                    return VoteType.MAJORITY;
                case string s when new Regex(@"^(two( |-)?thirds?)\b").IsMatch(type):
                    return VoteType.TWOTHIRD;
                case string s when new Regex(@"^(f(irst)?(-| )?p(ast)?(-| )?t(he)?(-| )?p(ost)?)\b").IsMatch(type):
                    return VoteType.FPTP;
                case string s when new Regex(@"^(two( |-)?rounds?)\b").IsMatch(type):
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

        bool? parseBool(string text)
        {
            text = text.ToLower();
            switch (text)
            {
                case string s when new Regex(@"^((y(es|ay|ea?)?)|(a(ye|ffirmative)?)|(t(rue)?))\b").IsMatch(text):
                    return true;
                case string s when new Regex(@"^((n(o|ay|egative|ah)?)|(f(alse)?))\b").IsMatch(text):
                    return false;
                default:
                    return null;
            }
        }

        bool parseBool(string text, out bool res)
        {
            bool? tmp = parseBool(text);
            if (tmp == null)
            {
                res = false;
                return false;
            }
            else
            {
                res = (bool)tmp;
                return true;
            }
        }

        [Command("makevote", RunMode = RunMode.Async)]
        [RequireNoProfile]
        public async Task MakeVote(string type = null, string time = null, bool? anon = null, [Remainder] string text = null)
        {
            if (type == null)
            {
                List<IUserMessage> conversation = new List<IUserMessage>();
                conversation.Add(Context.Message);
                conversation.Add(await ReplyAsync("Welcome to the vote wizard. Type 'cancel' any time to exit.\nWhat type is the vote? (Valid types: Majority, Two Thirds, First Past The Post, Two Rounds)"));
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
                conversation.Add(await ReplyAsync("Should the vote be anonymous?"));
                reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                conversation.Add(reply);
                bool anonymouse;
                while (!parseBool(reply.Content, out anonymouse))
                {
                    if (reply.Content == "cancel")
                    {
                        await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                        return;
                    }
                    conversation.Add(await ReplyAsync("Sorry, that isn't a valid response."));
                    reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                    conversation.Add(reply);
                }
                if (!isMultipleOption(voteType))
                {
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
                    if (!anonymouse)
                    {
                        emb.WithFooter("Message ID: " + mid.Id);
                    }
                    else
                    {
                        emb.WithFooter("Use `;;vote " + mid.Id + "` to vote.");
                    }
                    await mid.ModifyAsync((x) =>
                    {
                        x.Embed = emb.Build();
                    });
                    if (!anonymouse)
                    {
                        await mid.AddReactionsAsync(new IEmote[] {
                                            new Emoji("✅"), // yay
                                            new Emoji("❌"), // nay
                                            new Emoji("🇴") // abstain
                                        });
                    }
                    VoteMessage message = new VoteMessage();
                    message.MessageId = mid.Id;
                    message.CreatorId = Context.User.Id;
                    message.ChannelId = Context.Channel.Id;
                    message.Type = (int)voteType;
                    message.EndTime = DateTime.Now.AddHours(span.Hours).AddMinutes(span.Minutes).ToFileTime();
                    message.TimeSpan = span.Ticks;
                    message.Anonymous = anonymouse;
                    _context.VoteMessages.Add(message);
                    await _context.SaveChangesAsync();
                    await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                }
                else
                {
                    conversation.Add(await ReplyAsync("What should the title of the vote be?"));
                    reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                    conversation.Add(reply);
                    if (reply.Content == "cancel")
                    {
                        await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                        return;
                    }
                    string titlee = reply.Content;
                    conversation.Add(await ReplyAsync("What should the candidates be? (type 'done' when done)"));
                    List<string> candidates = new List<string>();
                    while (candidates.Count < 9)
                    {
                        reply = (IUserMessage)await NextMessageAsync(timeout: TimeSpan.FromMinutes(5));
                        conversation.Add(reply);
                        if (reply.Content == "cancel")
                        {
                            await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                            return;
                        }
                        if (reply.Content == "done") break;
                        candidates.Add(reply.Content);
                        await reply.AddReactionAsync(new Emoji("✅"));
                    }
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
                        .AddField("Ends:", timeStr)
                        .AddField("Type: ", ConvertVoteTypeToString(voteType))
                        .WithColor(Color.LightGrey);
                    for (int x = 0; x < candidates.Count; x++)
                    {
                        emb.AddField("Option #" + (x + 1) + ":", candidates[x], true);
                    }
                    RestUserMessage mid = await Context.Channel.SendMessageAsync("", false, emb.Build());
                    if (!anonymouse)
                    {
                        emb.WithFooter("Message ID: " + mid.Id);
                    }
                    else
                    {
                        emb.WithFooter("Use `;;vote " + mid.Id + "` to vote.");
                    }
                    await mid.ModifyAsync((x) =>
                    {
                        x.Embed = emb.Build();
                    });
                    if (!anonymouse)
                    {
                        await mid.AddReactionsAsync(numberEmotes.AsSpan(1, candidates.Count).ToArray());
                    }
                    VoteMessage message = new VoteMessage();
                    message.MessageId = mid.Id;
                    message.CreatorId = Context.User.Id;
                    message.ChannelId = Context.Channel.Id;
                    message.Type = (int)voteType;
                    message.EndTime = DateTime.Now.AddHours(span.Hours).AddMinutes(span.Minutes).ToFileTime();
                    message.TimeSpan = span.Ticks;
                    message.Anonymous = anonymouse;
                    _context.VoteMessages.Add(message);
                    await _context.SaveChangesAsync();
                    await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(conversation);
                }
            }
            else if (time == null || text == null || anon == null)
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
                        if (anon == null)
                        {
                            await ReplyAsync("You have no specified whether the vote is anonymouse");
                        }
                        else
                        {
                            bool anonymous = (bool)anon;
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
                                    if (!anonymous)
                                    {
                                        emb.WithFooter("Message ID: " + mid.Id);
                                    }
                                    else
                                    {
                                        emb.WithFooter("Use `;;vote " + mid.Id + "` to vote.");
                                    }
                                    await mid.ModifyAsync((x) =>
                                    {
                                        x.Embed = emb.Build();
                                    });
                                    if (!anonymous)
                                    {
                                        await mid.AddReactionsAsync(new IEmote[] {
                                            new Emoji("✅"), // yay
                                            new Emoji("❌"), // nay
                                            new Emoji("🇴") // abstain
                                        });
                                    }
                                    VoteMessage message = new VoteMessage();
                                    message.MessageId = mid.Id;
                                    message.CreatorId = Context.User.Id;
                                    message.ChannelId = Context.Channel.Id;
                                    message.Type = (int)voteType;
                                    message.EndTime = DateTime.Now.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).ToFileTime();
                                    message.TimeSpan = timeSpan.Ticks;
                                    message.Anonymous = anonymous;
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
                                    if (!anonymous)
                                    {
                                        emb.WithFooter("Message ID: " + mid.Id);
                                    }
                                    else
                                    {
                                        emb.WithFooter("Use `;;vote " + mid.Id + "` to vote.");
                                    }
                                    await mid.ModifyAsync((x) =>
                                    {
                                        x.Embed = emb.Build();
                                    });
                                    if (!anonymous)
                                    {
                                        await mid.AddReactionsAsync(numberEmotes.AsSpan(1, textArgs.Length - 1).ToArray());
                                    }
                                    VoteMessage message = new VoteMessage();
                                    message.MessageId = mid.Id;
                                    message.CreatorId = Context.User.Id;
                                    message.ChannelId = Context.Channel.Id;
                                    message.Type = (int)voteType;
                                    message.EndTime = DateTime.Now.AddHours(timeSpan.Hours).AddMinutes(timeSpan.Minutes).ToFileTime();
                                    message.TimeSpan = timeSpan.Ticks;
                                    message.Anonymous = anonymous;
                                    _context.VoteMessages.Add(message);
                                    await _context.SaveChangesAsync();
                                    await Context.Message.DeleteAsync();
                                }
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

        int parseVoteReply(string text)
        {
            text = text.ToLower();
            switch (text)
            {
                case string x when new Regex(@"^(y(ay|es)?)").IsMatch(text):
                    return 0;
                case string x when new Regex(@"^(n(ay|o)?)").IsMatch(text):
                    return 1;
                case string x when new Regex(@"^(a(bstain)?)").IsMatch(text):
                    return 2;
                default:
                    return -1;
            }
        }

        bool parseVoteReply(string text, out int result)
        {
            result = parseVoteReply(text);
            if (result == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [Command("vote", RunMode = RunMode.Async)]
        [RequireProfile]
        public async Task Vote(ulong messageId)
        {
            VoteMessage vms = _context.VoteMessages.Find(messageId);
            if (vms != null)
            {
                /*
                if (vms.Votes.FirstOrDefault(x => x.UserId == Context.User.Id) != null)
                {
                    await ReplyAsync("Sorry, but you have already voted on this vote.");
                    return;
                }
                */
                ITextChannel channel = (ITextChannel)Context.Client.GetChannel(vms.ChannelId);
                IEnumerable<IGuildUser> allowedVoters = await channel.GetUsersAsync().FlattenAsync();
                if (!allowedVoters.Any(x => x.Id == Context.User.Id))
                {
                    await ReplyAsync("You are not eligible for this vote.");
                    return;
                }
                IMessage message = await channel.GetMessageAsync(messageId);
                IEmbed emb = message.Embeds.First();
                if (isMultipleOption((VoteType)vms.Type))
                {
                    int choices = emb.Fields.Count() - 2;
                    string options = "";
                    for (int x = 2; x < emb.Fields.Count(); x++)
                    {
                        options += (x - 1) + " - " + emb.Fields[x].Value + "\n";
                    }
                    try
                    {
                        IMessage og = await Context.User.SendMessageAsync("Vote " + emb.Title + "\nVote options (reply with the number corresponding to the option you want):\n" + options);
                        await Context.Message.AddReactionAsync(new Emoji("✅"));
                        int parsed;
                        IMessage reply = await NextMessageAsync(new EnsureFromChannelCriterion(og.Channel), TimeSpan.FromMinutes(5));
                        if (reply.Content == "cancel") return;
                        while (!Int32.TryParse(reply.Content, out parsed) || !(parsed <= choices && parsed > 0))
                        {
                            await Context.User.SendMessageAsync("Please select a valid option or `cancel` if you wish to cancel the vote.");
                            reply = await NextMessageAsync(new EnsureFromChannelCriterion(og.Channel), TimeSpan.FromMinutes(5));
                            if (reply.Content == "cancel") return;
                        }
                        VoteEntry vote = vms.Votes.FirstOrDefault(x => x.UserId == Context.User.Id);
                        if (vote == null)
                        {
                            vote = new VoteEntry();
                            vote.UserId = Context.User.Id;
                            vote.Vote = parsed;
                            // vote.VoteMessage = vms.MessageId;
                            vms.Votes.Add(vote);
                            _context.VoteMessages.Update(vms);
                        }
                        else
                        {
                            vote.Vote = parsed;
                            _context.VoteEntries.Update(vote);
                        }
                        await Context.User.SendMessageAsync("Vote successful!");
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        await Context.Message.AddReactionAsync(new Emoji("❌"));
                        await ReplyAsync("You seem to have disabled DMs on this server. Please enable them to vote.");
                    }

                }
                else
                {
                    try
                    {
                        IMessage og = await Context.User.SendMessageAsync("Vote " + emb.Title + ": " + emb.Description + "\nReply with `yay`, `nay` or `abstain` to vote.");
                        await Context.Message.AddReactionAsync(new Emoji("✅"));
                        int parsed;
                        IMessage reply = await NextMessageAsync(new EnsureFromChannelCriterion(og.Channel), TimeSpan.FromMinutes(5));
                        if (reply.Content == "cancel") return;
                        while (!parseVoteReply(reply.Content, out parsed))
                        {
                            await Context.User.SendMessageAsync("Please select a valid option or `cancel` if you wish to cancel the vote.");
                            reply = await NextMessageAsync(new EnsureFromChannelCriterion(og.Channel), TimeSpan.FromMinutes(5));
                            if (reply.Content == "cancel") return;
                        }
                        VoteEntry vote = vms.Votes.FirstOrDefault(x => x.UserId == Context.User.Id);
                        if (vote == null)
                        {
                            vote = new VoteEntry();
                            vote.UserId = Context.User.Id;
                            vote.Vote = parsed;
                            // vote.VoteMessage = vms.MessageId;
                            vms.Votes.Add(vote);
                            _context.VoteMessages.Update(vms);
                        }
                        else
                        {
                            vote.Vote = parsed;
                            _context.VoteEntries.Update(vote);
                        }
                        await Context.User.SendMessageAsync("Vote successful!");
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        // Console.WriteLine(e); // Uncomment this if there's an actual error, this only serves to detect that the user has their DMs off.
                        await Context.Message.AddReactionAsync(new Emoji("❌"));
                        await ReplyAsync("You seem to have disabled DMs on this server. Please enable them to vote.");
                    }
                }
            }
            else
            {
                await ReplyAsync("Invalid message id.");
            }
        }
    }
}
