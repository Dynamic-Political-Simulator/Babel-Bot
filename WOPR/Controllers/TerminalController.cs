using BabelDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using WOPR.TerminalCommands;

namespace WOPR.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TerminalController : ControllerBase
	{
		private readonly Dictionary<string, ITerminalCommand> _terminalCommands = new Dictionary<string, ITerminalCommand>();

		private readonly BabelContext _context;

		public TerminalController(BabelContext context)
		{
			_context = context;

			var interfaceType = typeof(ITerminalCommand);

			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => interfaceType.IsAssignableFrom(p));

			foreach(var element in types)
			{
				try
				{
					ITerminalCommand instance = (ITerminalCommand)Activator.CreateInstance(element);

					_terminalCommands.Add(instance.GetCommand(), instance);
				}
				catch(Exception e)
				{
					//insert proper exception handling here
					//(this is mostly because I'm too lazy to exclude ITerminalCommand itself)
				}
				
			}
		}

		public class TerminalCommandForm
		{
			public string InputString { get; set; }
		}

		[HttpPost("command")]
		public async Task<string> SendTerminalCommandAsync([FromBody] TerminalCommandForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userId == null)
			{
				return "Please use command 'login' to login.";
			}

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null)
			{
				var newUser = new BabelDatabase.DiscordUser
				{
					DiscordUserId = userId,
					IsAdmin = false
				};

				_context.DiscordUsers.Add(newUser);

				await _context.SaveChangesAsync();
			}

			var firstWord = form.InputString.Split(" ")[0];

			ITerminalCommand command = null;

			if (_terminalCommands.TryGetValue(firstWord, out command))
			{
				return await command.DoCommandAsync(form.InputString);
			}

			return "Check";
		}
	}
}
