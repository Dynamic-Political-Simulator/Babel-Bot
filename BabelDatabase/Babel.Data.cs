using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

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

		public virtual List<CliqueMemberCharacter> Cliques { get; set; }

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

	public class GameState
	{
		[Key]
		public int GameStateId { get; set; } = 1;
		public int CurrentYear { get; set; } = 2500;
		public int SecondsToMidnight { get; set; } = 10800;
	}

	public class Committee
	{
		[Key]
		public string CommitteeId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string CommitteeName { get; set; }
		[Required]
		public long Money { get; set; }

		public virtual List<Character> CommitteeMembers { get; set; }
	}

	public class CliqueMemberCharacter
	{
		public string CliqueId { get; set; }
		public virtual Clique Clique { get; set; }

		public string MemberId { get; set; }
		public virtual Character Member { get; set; }
	}

	public class CliqueOfficerCharacter
	{
		public string CliqueId { get; set; }
		public virtual Clique Clique { get; set; }

		public string OfficerId { get; set; }
		public virtual Character Officer { get; set; }
	}

	public class Clique
	{
		[Key]
		public string CliqueId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string CliqueName { get; set; }
		[Required]
		public ulong Money { get; set; }

		public virtual List<CliqueMemberCharacter> CliqueMembers { get; set; }
		public virtual List<CliqueOfficerCharacter> CliqueOfficers { get; set; }

		public virtual List<Alignment> Alignments { get; set; }
	}

	public class CliqueInvite
	{
		[Key]
		public string CliqueInviteId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string CliqueId { get; set; }
		public virtual Clique Clique { get; set; }

		[Required]
		public string CharacterId { get; set; }
		public virtual Character Character { get; set; }
	}

	public class Alignment
	{
		[Key]
		public string AlignmentId { get; set; } = Guid.NewGuid().ToString();

		public virtual List<Clique> Cliques { get; set; }

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

	public class CustomSpending
	{
		[Key]
		public string CustomSpendingId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string SpendingDescription { get; set; }

		[Required]
		public ulong Amount { get; set; }

		[Required]
		public string CliqueId { get; set; }
		public virtual Clique Clique { get; set; }

		[Required]
		public string CharacterId { get; set; }
		public virtual Character Character { get; set; }
	}

	public class AlignmentSpending
	{
		[Key]
		public string AlignmentSpendingId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public ulong Amount { get; set; }

		[Required]
		public string CliqueId { get; set; }
		public virtual Clique Clique { get; set; }

		[Required]
		public string CharacterId { get; set; }
		public virtual Character Character { get; set; }

		[Required]
		public string AlignmentId { get; set; }
		public virtual Alignment Alignment { get; set; }

		public string PlanetTargetId { get; set; }
		public virtual PopsimGlobalEthicGroup PlanetTarget { get; set; }

		public string GlobalTargetId { get; set; }
		public virtual PopsimGlobalEthicGroup GlobalTarget { get; set; }
	}

	public class SpendingLog
	{
		[Key]
		public string SpendingLogId { get; set; } = Guid.NewGuid().ToString();

		[Required]
		public string SpendingDescription { get; set; }
	}

	public class PopsimGlobalEthicGroup
	{
		[Key]
		public string PopsimGlobalEthicGroupId { get; set; } = Guid.NewGuid().ToString();

		public string PopsimGlobalEthicGroupName { get; set; }
		
		public virtual List<PopsimPlanetEthicGroup> PlanetaryEthicGroups { get; set; }

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

	// Planets ------------------------------------------
	public class PopsimPlanet
	{
		[Key]
		public string PopsimPlanetId { get; set; } = Guid.NewGuid().ToString();

		public string PlanetName { get; set; }
	}

	public class PopsimPlanetEthicGroup
	{
		[Key]
		public string PopsimPlanetEthicGroupId { get; set; } = Guid.NewGuid().ToString();

		public long MembersOnPlanet { get; set; }

		public string PopsimGlobalEthicGroupId { get; set; }
		public virtual PopsimGlobalEthicGroup PopsimGlobalEthicGroup { get; set; }

		public string PopsimPlanetId { get; set; }
		public virtual PopsimPlanet PopsimPlanet { get; set; }
	}

}
