using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using BabelDatabase;
using Microsoft.Extensions.Configuration;

namespace BabelBot.CustomPreconditions
{
	public class RequireProfile : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var configuration = services.GetService(typeof(IConfiguration)) as IConfiguration;

			using var db = new BabelContext(configuration);

			var hasProfile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == context.User.Id.ToString());

			return Task.FromResult(hasProfile == null ? PreconditionResult.FromError("You need a profile, create one using the `create profile` command.") : PreconditionResult.FromSuccess());
		}
	}
}
