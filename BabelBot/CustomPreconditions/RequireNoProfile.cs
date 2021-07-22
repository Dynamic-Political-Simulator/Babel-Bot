using BabelDatabase;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BabelBot.CustomPreconditions
{
	public class RequireNoProfile : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var configuration = services.GetService(typeof(IConfiguration)) as IConfiguration;

			using var db = new BabelContext(configuration);

			var hasProfile = db.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == context.User.Id.ToString());

			return Task.FromResult(hasProfile != null ? PreconditionResult.FromError("You already have a profile.") : PreconditionResult.FromSuccess());
		}
	}
}
