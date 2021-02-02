using BabelDatabase;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.Services
{
	public class UserService
	{
		private readonly WoprConfig _config;
		private readonly BabelContext _context;

		public UserService(IOptions<WoprConfig> config, BabelContext context)
		{
			_config = config.Value;
			_context = context;
		}

		public AuthenticateResponse Authenticate(string discordUserId, string discordToken)
		{
			//var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);
			var user = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == discordUserId);

			// return null if user not found
			if (user == null) return null;

			// authentication successful so generate jwt token
			var token = GenerateJwtToken(user);

			return new AuthenticateResponse(user, token);
		}
	}
}
