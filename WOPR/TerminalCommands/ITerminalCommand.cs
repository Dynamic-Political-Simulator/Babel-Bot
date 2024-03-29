﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.TerminalCommands
{
	public interface ITerminalCommand
	{
		public string GetCommand();
		public Task<string> DoCommandAsync(string command);
	}
}
