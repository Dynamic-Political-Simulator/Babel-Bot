using BabelDatabase;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.CustomPreconditions
{
	public class RequireLivingActiveCharacter : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var db = services.GetService(typeof(BabelContext)) as BabelContext;

			var user = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == context.User.Id.ToString());

			return Task.FromResult(user.ActiveCharacterId == null ? PreconditionResult.FromError("You need a living active character to use this command.") : PreconditionResult.FromSuccess());
		}
	}
}
