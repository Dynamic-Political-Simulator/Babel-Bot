using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.Services
{
	public class DiscordBotService
	{
		public IConfiguration Configuration { get; }

		private readonly DiscordSocketClient _client = new DiscordSocketClient();

		public DiscordBotService(IConfiguration configuration)
		{
			Configuration = configuration;
			Start().GetAwaiter().GetResult();
		}

		private async Task Start()
		{
			await _client.LoginAsync(TokenType.Bot, Configuration["BotToken"]);
			await _client.StartAsync();

			await Task.Delay(-1);
		}

		public async Task<string> SendAuthCodeToDiscordUser(string discordUserId)
		{
			try
			{
				var user = _client.GetUser(ulong.Parse(discordUserId));

				var code = DiscordUserAuthenticationTokenService.GenerateTokenForDiscordUser(discordUserId);

				var privateChannel = await user.GetOrCreateDMChannelAsync();

				await privateChannel.SendMessageAsync($"Your auth code for WOPR: {code}\nThis code will expire in 20 seconds.");

				return null;
			}
			catch (Exception e)
			{
				if (e.GetType().Equals(typeof(NullReferenceException)))
				{
					return "Couldn't send message.";
				}

				return "Something went wrong.";
			}
		}
	}
}
