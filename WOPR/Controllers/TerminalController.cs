using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WOPR.TerminalCommands;

namespace WOPR.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TerminalController : ControllerBase
	{
		private readonly Dictionary<string, ITerminalCommand> _terminalCommands;

		public TerminalController()
		{
			var interfaceType = typeof(ITerminalCommand);

			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetTypes())
				.Where(p => interfaceType.IsAssignableFrom(p));

			foreach(var element in types)
			{
				ITerminalCommand instance = (ITerminalCommand)Activator.CreateInstance(element);

				_terminalCommands.Add(instance.GetCommand(), instance);
			}
		}

		[HttpPost("terminal")]
		public string SendTerminalCommand(string inputString)
		{
			var firstWord = inputString.Split(" ")[0];

			ITerminalCommand command = null;

			if (_terminalCommands.TryGetValue(firstWord, out command))
			{
				return command.DoCommand(inputString);
			}

			return null;
		}
	}
}
