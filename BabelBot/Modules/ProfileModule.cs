using BabelBot.CustomPreconditions;
using BabelDatabase;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Modules
{
	public class ProfileModule : ModuleBase<SocketCommandContext>
	{
		private IConfiguration Configuration;

		public ProfileModule(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		[Command("create profile")]
		[RequireNoProfile]
		public async Task CreateProfile()
		{
			using var db = new BabelContext(Configuration);
			var profile = new DiscordUser()
			{
				DiscordUserId = Context.User.Id.ToString(),
				UserName = Context.User.Username,
				IsAdmin = false
			};

			db.DiscordUsers.Add(profile);
			await db.SaveChangesAsync();

			await ReplyAsync("Profile created.");
		}
	}
}
