using BabelBot.Context;
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

			var activeCharacter = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == context.User.Id.ToString()).ActiveCharacter;

			return Task.FromResult(activeCharacter == null || activeCharacter.IsDead() ? PreconditionResult.FromError("You need a living active character to use this command.") : PreconditionResult.FromSuccess());
		}
	}
}
