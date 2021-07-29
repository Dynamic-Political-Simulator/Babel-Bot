using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.TerminalCommands
{
    public class MapCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return "redirect-map";
        }

        public string GetCommand()
        {
            return "map";
        }
    }

    public class MyCharactersCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return "redirect-my-characters";
        }

        public string GetCommand()
        {
            return "characters";
        }
    }

    public class EmpireCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return "redirect-empire?name=Common and United League of Orion";
        }

        public string GetCommand()
        {
            return "nation";
        }
    }

    public class ClockCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return "redirect-clock";
        }

        public string GetCommand()
        {
            return "clock";
        }
    }

    public class HelpCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return @"Commonly Used Commands:
map - Open the Galactic Map
nation - View information on the nation
characters - View your characters
character-search - Lookup characters
clock - View the time to midnight
government - View the current popularity of government branches
options - Open the options screen
help - Returns this message";
        }

        public string GetCommand()
        {
            return "help";
        }
    }

    public class PopularityCommand : ITerminalCommand
    {
        public async Task<string> DoCommandAsync(string command)
        {
            return "redirect-popularity";
        }

        public string GetCommand()
        {
            return "government";
        }
    }
}
