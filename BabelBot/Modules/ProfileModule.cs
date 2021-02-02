using BabelBot.CustomPreconditions;
using BabelDatabase;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BabelBot.Modules
{
	public class ProfileModule : ModuleBase<SocketCommandContext>
	{
		private readonly BabelContext _context;

		public ProfileModule(BabelContext context)
		{
			_context = context;
		}

		[Command("create profile")]
		[RequireNoProfile]
		public async Task CreateProfile()
		{
			var profile = new DiscordUser()
			{
				DiscordUserId = Context.User.Id.ToString(),
				UserName = Context.User.Username,
				IsAdmin = false
			};

			_context.DiscordUsers.Add(profile);
			await _context.SaveChangesAsync();

			await ReplyAsync("Profile created.");
		}
	}
}
