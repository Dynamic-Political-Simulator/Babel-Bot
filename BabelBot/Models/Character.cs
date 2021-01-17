using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BabelBot.Models
{
	public class Character
	{
		[Key]
		public string CharacterId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string CharacterName { get; set; }
		public int YearOfBirth { get; set; } = 2500;
		public int YearOfDeath { get; set; } = 0;
		public string CauseOfDeath { get; set; } = null;
		public string CharacterBio { get; set; } = "Character bio.";

		public string PartyId { get; set; }
		public virtual Party Party { get; set; }

		public string SpeciesId { get; set; }
		public virtual Species Species { get; set; }

		public string DiscordUserId { get; set; }
		public virtual DiscordUser DiscordUser { get; set; }

		public bool IsDead()
		{
			return YearOfDeath != 0;
		}
	}
}
