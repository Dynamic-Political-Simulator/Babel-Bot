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
		private readonly BabelContext _context;

		public DiscordUserService(BabelContext context)
		{
			_context = context;
		}

		public DiscordUser GetUserById(string discordUserId)
		{
			return _context.DiscordUsers.FirstOrDefault(du => du.DiscordUserId == discordUserId);
		}
	}
}
