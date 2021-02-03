using BabelDatabase;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.Services
{
	public class DiscordUserService
	{
		private readonly WoprConfig _config;
		private readonly BabelContext _context;

		public DiscordUserService(IOptions<WoprConfig> config, BabelContext context)
		{
			_config = config.Value;
			_context = context;
		}

		public DiscordUser GetUserById(string discordUserId)
		{
			return _context.DiscordUsers.FirstOrDefault(du => du.DiscordUserId == discordUserId);
		}

		public DiscordUser Authenticate(string discordUserId, string discordToken)
		{
			var user = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == discordUserId);

			// return null if user not found
			if (user == null) return null;

			var tokenIsCorrect = DiscordUserAuthenticationTokenService.CheckToken(discordUserId, discordToken);

			if (!tokenIsCorrect)
			{
				return null;
			}

			return user;
		}
	}
}
