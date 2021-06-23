using BabelDatabase;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.CustomPreconditions
{
	public class RequiresAdmin : PreconditionAttribute
	{

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var db = services.GetService(typeof(BabelContext)) as BabelContext;

			var discordUser = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == context.User.Id.ToString());

			if (discordUser == null || !discordUser.IsAdmin)
			{
				return Task.FromResult(
					PreconditionResult.FromError("You don't have the required permissions to use this command."));
			}

			return Task.FromResult(PreconditionResult.FromSuccess());
		}
	}
}
