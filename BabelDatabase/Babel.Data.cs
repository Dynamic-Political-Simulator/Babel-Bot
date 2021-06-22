using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using DPSSimulation.Classes;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

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

	public class PlayerStaffAction
	{
		public string PlayerId { get; set; }
		public virtual DiscordUser Player { get; set; }

		public string StaffActionId { get; set; }
		public virtual StaffAction StaffAction { get; set; }
	}

	public class StaffStaffAction
	{
		public string StaffId { get; set; }
		public virtual DiscordUser Staff { get; set; }

		public string StaffActionId { get; set; }
		public virtual StaffAction StaffAction { get; set; }
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

		public virtual List<PlayerStaffAction> Players { get; set; }
		public virtual List<StaffStaffAction> Staff { get; set; }

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
	

	public class PopsimPlanetEthicGroup
	{
		[Key]
		public string PopsimPlanetEthicGroupId { get; set; } = Guid.NewGuid().ToString();

		public long MembersOnPlanet { get; set; }

		public string PopsimGlobalEthicGroupId { get; set; }
		public virtual PopsimGlobalEthicGroup PopsimGlobalEthicGroup { get; set; }

		public string PopsimPlanetId { get; set; }
		public virtual Planet PopsimPlanet { get; set; }
	}

	// Simulation -------------------------------------------
	public class Empire
	{
		[Key]
		public string EmpireId { get; set; } = Guid.NewGuid().ToString();
		[Required]
		public string Name { get; set; }
		[Required]
		public int GameId { get; set; }
		public List<GalacticObject> GalacticObjects { get; set; } = new List<GalacticObject>();
		public List<Fleet> Fleets { get; set; } = new List<Fleet>();
		public List<Army> Armies { get; set; } = new List<Army>();
		public List<Fleet> MiningStations { get; set; } = new List<Fleet>();
		public List<Fleet> ResearchStations { get; set; } = new List<Fleet>();
		public Dictionary<string, ulong> NationalOutput { get; set; } = new Dictionary<string, ulong>();
		public InfraStructureData InfraStructureData { get; set; }
		public Dictionary<string, float> EconGmData { get; set; } = new Dictionary<string, float>();
		public Dictionary<Alignment, int> GeneralAssembly { get; set; } = new Dictionary<Alignment, int>();
		public Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>> PopsimGmData { get; set; } = new Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>>();
	}
	//celestial objects-----------------------------------
	public class GalacticObject
	{
		public string GalacticObjectId { get; set; } = Guid.NewGuid().ToString();
		public float PosX { get; set; }
		public float PosY { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public List<Planet> Planets { get; set; } = new List<Planet>();
		public List<Hyperlane> Hyperlanes { get; set; } = new List<Hyperlane>();
		public Starbase Starbase { get; set; }
	}
	public class Hyperlane
    {
		[Key]
		public string HyperlaneId { get; set; } = Guid.NewGuid().ToString();
		public string TargetId { get; set; }
		public float Distance { get; set; }
    }
	public class Planet
	{
		[Key]
		public string PlanetId { get; set; } = Guid.NewGuid().ToString();
		[Required]
		public virtual string GalacticObjectId { get; set; }
		public virtual GalacticObject GalacticObject { get; set; }
		public string PlanetName { get; set; }
		public string PlanetDescription { get; set; }
		public string PlanetClass { get; set; }
		public string OwnerId { get; set; }
		public string ControllerId { get; set; }
		public List<Pop> Pops { get; set; }
		public List<Building> Buildings { get; set; } 
		public List<District> Districts { get; set; }
		public ulong Population { get; set; }
		public Dictionary<string, ulong> Output { get; set; } = new Dictionary<string, ulong>();
		public Data Data { get; set; }
		public Dictionary<PopsimPlanetEthicGroup, float> PlanetGroups { get; set; } = new Dictionary<PopsimPlanetEthicGroup, float>();
		public Dictionary<string, float> EconGmData { get; set; } = new Dictionary<string, float>();
		public Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>> PopsimGmData { get; set; } = new Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>>();
	}
	
	public class District
    {
		[Key]
		public string DistrictId { get; set; } = Guid.NewGuid().ToString();
		public string Type { get; set; }
		[Required]
		public virtual string PlanetId { get; set; }
		public virtual Planet Planet { get; set; }
	}
	public class Building
    {
		[Key]
		public string BuildingId { get; set; } = Guid.NewGuid().ToString();
		public string Type { get; set; }
		public bool Ruined { get; set; }
		[Required]
		public virtual string PlanetId { get; set; }
		public virtual Planet Planet { get; set; }
    }

	public class Pop
    {
		public string PopId { get; set; } = Guid.NewGuid().ToString();
		[Required]
		public virtual string PlanetId { get; set; }
		public virtual Planet Planet { get; set; }

		public string Job { get; set; }
		public string Strata { get; set; }
		public float Power { get; set; }
		public float Happiness { get; set; }

	}
	//military stuff -----------------------------------------------------------
	public class Starbase
    {
		[Key]
		public string StarbaseId { get; set; } = Guid.NewGuid().ToString();
		public string Owner { get; set; }
		public string Level { get; set; }
		public List<string> Modules { get; set; }
		public List<string> Buildings { get; set; }
		[Required]
		public string FleetId { get; set; }
		public Fleet Starbasefleet { get; set; }
    }
	public class Fleet
    {
		[Key]
		public string FleetId { get; set; } = Guid.NewGuid().ToString();
		public string Name { get; set; }
		[Required]
		public string OwnerID { get; set; }
		public Empire Owner { get; set; }
		public double MilitaryPower { get; set; }
		[Required]
		public string SystemId { get; set; }
		public GalacticObject System { get; set; }
		public List<Ship> Ships { get; set; } = new List<Ship>();
    }
	public class Ship
    {
		[Key]
		public string ShipId { get; set; } = Guid.NewGuid().ToString();
		[Required]
		public string FleetId { get; set; }
		public Fleet Fleet { get; set; }
		public string ShipName { get; set; }
		public string Type { get; set; }
    }
	public class Army
    {
		[Key]
		public string ArmyId { get; set; } = Guid.NewGuid().ToString();
		public string Name { get; set; }
		public string Type { get; set; }
		[Required]
		public string OwnerId { get; set; }
		public Empire Owner { get; set; }
		public string PlanetId { get; set; }
		public Planet Planet { get; set; }
    }
	//Popsim exclusive entities-----------------------------------------
	public class Party
    {
		[Key]
		public string PartyId { get; set; } = Guid.NewGuid().ToString();
		public Dictionary<PopsimGlobalEthicGroup, float> PopGroupEnlistment { get; set; } = new Dictionary<PopsimGlobalEthicGroup, float>();
		public Dictionary<PopsimGlobalEthicGroup, float> UpperPartyMembership { get; set; } = new Dictionary<PopsimGlobalEthicGroup, float>();
		public Dictionary<PopsimGlobalEthicGroup, float> LowerPartyMembership { get; set; } = new Dictionary<PopsimGlobalEthicGroup, float>();
		public Dictionary<Alignment, float> UpperPartyAffinity { get; set; } = new Dictionary<Alignment, float>();
		public Dictionary<Alignment, float> LowerPartyAffinity { get; set; } = new Dictionary<Alignment, float>();
		public float UpperPartyPercentage { get; set; }
	}

	public class Military
    {
		[Key]
		public string RevolutionaryGuardId { get; set; } = Guid.NewGuid().ToString();
		public float MilitaryPoliticisation { get; set; }
		public Dictionary<PopsimGlobalEthicGroup, float> MilitaryGroups { get; set; }
		public Dictionary<Alignment, float> MilitaryFactions { get; set; }
	}
	//Data-------------------------------------------------------
	public class InfraStructureData
    {
		[Key]
		public string InfraStructureDataId = Guid.NewGuid().ToString();
		public ulong GdpPerInfrastructure { get; set; }
		public Dictionary<string, Infrastructure> Infrastructures { get; set; } = new Dictionary<string, Infrastructure>();
	}

	public class Data
    {
		[Key]
		public string DataId = Guid.NewGuid().ToString();
		public List<Strata> Stratas = new List<Strata>();
		public int BaseGdpPerPop;
    }
}
