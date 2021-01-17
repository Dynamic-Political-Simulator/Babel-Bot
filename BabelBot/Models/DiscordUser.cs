using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BabelBot.Models
{
	public class DiscordUser
	{
		[Key]
		public string DiscordUserId { get; set; }

		public string ActiveCharacterId { get; set; }
		public virtual Character ActiveCharacter { get; set; }

		public virtual List<Character> Characters { get; set; }
	}
}
