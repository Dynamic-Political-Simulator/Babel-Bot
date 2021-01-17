using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using BabelBot.Context;

namespace BabelBot
{
	public class CommandHandler
	{
		private readonly CommandService _commandService;
		private readonly DiscordSocketClient _discordSocketClient;
		private readonly BabelContext _context;
		private readonly IServiceProvider _serviceProvider;

		public CommandHandler(CommandService commandService, DiscordSocketClient discordSocketClient, IServiceProvider serviceProvider)
		{
			_commandService = commandService;
			_discordSocketClient = discordSocketClient;
			_serviceProvider = serviceProvider;

			_context = serviceProvider.GetService<BabelContext>();

			_discordSocketClient.MessageReceived += HandleCommandAsync;
		}

		private async Task HandleCommandAsync(SocketMessage messageParam)
		{
			var message = messageParam as SocketUserMessage;
			if (message == null) return;

			int argPos = 0;

			if (!(message.HasStringPrefix(";;", ref argPos) ||
				  message.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos)) ||
				message.Author.IsBot)
				return;

			var context = new SocketCommandContext(_discordSocketClient, message);

			var result = await _commandService.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: _serviceProvider);

			if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ErrorReason);
		}
	}
}
