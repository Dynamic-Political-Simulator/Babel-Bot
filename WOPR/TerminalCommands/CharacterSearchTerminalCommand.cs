using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.TerminalCommands
{
    public class CharacterSearchTerminalCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return "redirect-character-search";
        }

        public string GetCommand()
        {
            return "character-search";
        }
    }
}
