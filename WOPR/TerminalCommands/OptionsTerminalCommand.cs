using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.TerminalCommands
{
    public class OptionsTerminalCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return "redirect-options";
        }

        public string GetCommand()
        {
            return "options";
        }
    }
}
