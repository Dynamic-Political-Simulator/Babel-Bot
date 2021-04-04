using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using BabelDatabase;

namespace BabelBot.CustomPreconditions
{
	public class RequireProfile : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var db = services.GetService(typeof(BabelContext)) as BabelContext;

			var hasProfile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == context.User.Id.ToString());

			return Task.FromResult(hasProfile == null ? PreconditionResult.FromError("You need a profile, create one using the `create profile` command.") : PreconditionResult.FromSuccess());
		}
	}
}
