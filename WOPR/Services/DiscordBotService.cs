using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.Services
{
	public class DiscordBotService
	{
		private readonly WoprConfig _config;
		private readonly DiscordSocketClient _client = new DiscordSocketClient();

		private static DiscordBotService _instance;

		private DiscordBotService()
		{
			IOptions<WoprConfig> config;

			var configBuilder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			var configuration = configBuilder.Build();

			var section = configuration.GetSection("WoprConfig");

			_config = section.Get<WoprConfig>();

			Start().GetAwaiter().GetResult();
		}

		public static DiscordBotService GetInstance()
		{
			if(_instance != null)
			{
				return _instance;
			}
			else
			{
				_instance = new DiscordBotService();
				return _instance;
			}
		}

		private async Task Start()
		{
			await _client.LoginAsync(TokenType.Bot, _config.BotToken);
			await _client.StartAsync();

			Console.WriteLine("Bot starting...");

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
