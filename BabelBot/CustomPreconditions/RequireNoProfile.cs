using Discord.Commands;
using BabelBot.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BabelBot.CustomPreconditions
{
	public class RequireNoProfile : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var db = services.GetService(typeof(BabelContext)) as BabelContext;

			var hasProfile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == context.User.Id.ToString());

			return Task.FromResult(hasProfile != null ? PreconditionResult.FromError("You already have a profile.") : PreconditionResult.FromSuccess());
		}
	}
}
