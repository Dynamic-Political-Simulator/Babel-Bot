using BabelBot.CustomPreconditions;
using BabelDatabase;
using Discord;
using Discord.Commands;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace BabelBot.Modules
{
    public class VoteModule : ModuleBase<SocketCommandContext>
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

        string[] formats = new[] { "h'h'", "h'h 'm'm'", "m'm'", "%m" }; // should ensure that XhMmin format works
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

        [Command("makevote")]
        [RequireNoProfile]
        public async Task CreateProfile(string type = null, string time = null, string link = null, [Remainder] string title = null)
        {
            if (type == null)
            {
                // Do the wizard
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
