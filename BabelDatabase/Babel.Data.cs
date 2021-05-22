using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BabelDatabase
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
		public virtual PopsimParty Party { get; set; }

		public string SpeciesId { get; set; }
		public virtual Species Species { get; set; }

		[Required]
		public string DiscordUserId { get; set; }
		public virtual DiscordUser DiscordUser { get; set; }

		public bool IsDead()
		{
			return CauseOfDeath != null;
		}
	}

	public class DiscordUser
	{
		[Key]
		public string DiscordUserId { get; set; }
		[Required]
		public string UserName { get; set; }

		public bool IsAdmin { get; set; }

		public string ActiveCharacterId { get; set; }
		public virtual Character ActiveCharacter { get; set; }

		public virtual List<Character> Characters { get; set; }
	}

	public class StaffAction
	{
		[Key]
		public string StaffActionId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string Title { get; set; }

		[Required]
		public DateTime TimeStarted { get; set; }

		[Required]
		public string OwnerId { get; set; }
		public virtual DiscordUser Owner { get; set; }

		public virtual List<DiscordUser> Players { get; set; }
		public virtual List<DiscordUser> Staff { get; set; }

		public virtual List<StaffActionPost> StaffActionPosts { get; set; } 
	}

	public class StaffActionPost
	{
		[Key]
		public string StaffActionPostId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string Content { get; set; }

		[Required]
		public DateTime TimePosted { get; set; }

		[Required]
		public string StaffActionId { get; set; }
		public virtual StaffAction StaffAction { get; set; }

		[Required]
		public string AuthorId { get; set; }
		public virtual DiscordUser Author { get; set; }
	}

	public class Species
	{
		[Key]
		public string SpeciesId { get; set; } = Guid.NewGuid().ToString();

		public string SpeciesName { get; set; }
	}

	public class Year
	{
		[Key]
		public int YearId { get; set; } = 1;
		public int CurrentYear { get; set; }
	}

	public class TimeToMidnight
	{
		[Key]
		public int TimeToMidnightId { get; set; } = 1;
		public int SecondsToMidnight { get; set; } = 10800;
	}

	// Popsim stuff
	public class PopsimReport
	{
		[Key]
		public string PopsimReportId { get; set; } = Guid.NewGuid().ToString();
		public string PopsimReportName { get; set; }
		public bool IsPublic { get; set; }
		public bool IsCurrent { get; set; }


	}

	public class PopsimGlobalEthicGroup
	{
		[Key]
		public string PopsimGlobalEthicGroupId { get; set; } = Guid.NewGuid().ToString();

		public string PopsimGlobalEthicGroupName { get; set; }
		
		public List<PopsimPlanetEthicGroup> planetaryEthicGroups { get; set; }

		public int FederalismCentralism { get; set; }
		public int DemocracyAuthority { get; set; }
		public int GlobalismIsolationism { get; set; }
		public int MilitarismPacifism { get; set; }
		public int SecurityFreedom { get; set; }
		public int CooperationCompetition { get; set; }
		public int SecularismSpiritualism { get; set; }
		public int ProgressivismTraditionalism { get; set; }
		public int MonoculturalismMulticulturalism { get; set; }
	}

	public class PopsimGlobalEthicGroupModifier
	{
		[Key]
		public string PopsimGlobalEthicGroupModifierId { get; set; } = Guid.NewGuid().ToString();

		public string PopsimGlobalEthicGroupModifierDescription { get; set; }

		public int FederalismCentralism { get; set; }
		public int DemocracyAuthority { get; set; }
		public int GlobalismIsolationism { get; set; }
		public int MilitarismPacifism { get; set; }
		public int SecurityFreedom { get; set; }
		public int CooperationCompetition { get; set; }
		public int SecularismSpiritualism { get; set; }
		public int ProgressivismTraditionalism { get; set; }
		public int MonoculturalismMulticulturalism { get; set; }

		public string TargetPartyId { get; set; }
		public PopsimParty TargetParty {get;set;}
	}

	// Planets ------------------------------------------
	public class PopsimPlanet
	{
		[Key]
		public string PopsimPlanetId { get; set; } = Guid.NewGuid().ToString();

		public string PlanetName { get; set; }
	}

	public class PopsimPlanetModifier
	{
		[Key]
		public string PopsimPlanetModifierId { get; set; } = Guid.NewGuid().ToString();

		public string PopsimPlanetModifierDescription { get; set; }

		public int Modifier { get; set; }

		public string TargetPartyId { get; set; }
		public PopsimParty TargetParty { get; set; }
	}

	public class PopsimPlanetEthicGroup
	{
		[Key]
		public string PopsimPlanetEthicGroupId { get; set; } = Guid.NewGuid().ToString();

		public long MembersOnPlanet { get; set; }

		public string PopsimGlobalEthicGroupId { get; set; }
		public PopsimGlobalEthicGroup PopsimGlobalEthicGroup { get; set; }

		public string PopsimPlanetId { get; set; }
		public PopsimPlanet PopsimPlanet { get; set; }
	}

	// Parties ----------------------------------------------
	public class PopsimParty
	{
		[Key]
		public string PartyId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string PartyName { get; set; }

		public virtual List<Character> Members { get; set; }
	}

	// Media ------------------------------------------------
	public class PopsimMediaEntity
	{
		[Key]
		public string PopsimMediaEntityId { get; set; } = Guid.NewGuid().ToString();

		public string PopsimMediaEntityName { get; set; }


	}

	public class PopsimMediaEntityBroadcast
	{
		[Key]
		public string PopsimMediaEntityBroadcastId { get; set; } = Guid.NewGuid().ToString();

		// -10 to 10
		public int Bias { get; set; }

		public string BiasedPartyId { get; set; }
		public PopsimParty BiasedParty { get; set; }

		public string PopsimGlobalEthicGroupModifierId { get; set; }
		public PopsimGlobalEthicGroupModifier PopsimGlobalEthicGroupModifier { get; set; }

		public string PopsimPlanetModifierId { get; set; }
		public PopsimPlanetModifier PopsimPlanetModifier { get; set; }

		public string PopsimMediaEntityId { get; set; }
		public PopsimMediaEntity PopsimMediaEntity { get; set; }
	}
}
