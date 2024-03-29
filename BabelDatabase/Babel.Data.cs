﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using DPSSimulation.Classes;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

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
            return YearOfDeath != 0;
        }

        public int GetAge(int currentYear)
        {
            var age = currentYear - YearOfBirth;
            return age;
        }
    }

    public class CharacterDeathTimer
    {
        [Key]
        public string CharacterDeathTimerId { get; set; } = Guid.NewGuid().ToString();

        public string CharacterId { get; set; }
        public virtual Character Character { get; set; }

        public int YearOfDeath { get; set; }

        [Required]
        public DateTime DeathTime { get; set; }
    }

    public class Graveyard
    {
        [Key]
        public string ChannelId { get; set; }
        public string ServerId { get; set; }
    }

    public class DiscordUser
    {
        [Key]
        public string DiscordUserId { get; set; }
        [Required]
        public string UserName { get; set; }

        public bool IsAdmin { get; set; }

        public string ActiveCharacterId { get; set; }

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
        public string SpeciesDescription { get; set; }
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

        public virtual List<CliqueMemberCharacter> CliqueMemberCharacter { get; set; }
        public virtual List<CliqueOfficerCharacter> CliqueOfficerCharacter { get; set; }

        public virtual List<AlignmentClique> Alignments { get; set; }
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

    public class AlignmentClique
    {
        public string AlignmentId { get; set; }
        public virtual Alignment Alignment { get; set; }

        public string CliqueId { get; set; }
        public virtual Clique Clique { get; set; }
    }

    public class Alignment
    {
        [Key]
        public string AlignmentId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string AlignmentName { get; set; }

        public virtual List<AlignmentClique> AlignmentClique { get; set; }

        public float Establishment { get; set; }
        public float UpperPartyModifier { get; set; }
        public float LowerPartyModiifer { get; set; }

        public int FederalismCentralism { get; set; }
        public int DemocracyAuthority { get; set; }
        public int GlobalismIsolationism { get; set; }
        public int MilitarismPacifism { get; set; }
        public int SecurityFreedom { get; set; }
        public int CooperationCompetition { get; set; }
        public int SecularismSpiritualism { get; set; }
        public int ProgressivismTraditionalism { get; set; }
        public int MonoculturalismMulticulturalism { get; set; }

        public override string ToString()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new EFResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, settings);
        }

        public static explicit operator Alignment(string m)
        {
            return JsonConvert.DeserializeObject<Alignment>(m);
        }
        public static bool operator ==(Alignment a, Alignment b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.AlignmentId == b.AlignmentId;
        }

        public static bool operator !=(Alignment a, Alignment b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            return a.AlignmentId != b.AlignmentId;
        }

        public override bool Equals(Object x)
        {
            if (x is null) return false;
            return this.AlignmentId == ((Alignment)x).AlignmentId;
        }

        public override int GetHashCode()
        {
            return this.AlignmentId.GetHashCode();
        }
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

        public int PartyInvolvementFactor { get; set; }
        public float Radicalisation { get; set; }
        public float PartyEnlistmentModifier { get; set; }

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

        public override string ToString()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new EFResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, settings);
        }

        public static explicit operator PopsimGlobalEthicGroup(string m)
        {
            return JsonConvert.DeserializeObject<PopsimGlobalEthicGroup>(m);
        }

        public static bool operator ==(PopsimGlobalEthicGroup a, PopsimGlobalEthicGroup b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.PopsimGlobalEthicGroupId == b.PopsimGlobalEthicGroupId;
        }

        public static bool operator !=(PopsimGlobalEthicGroup a, PopsimGlobalEthicGroup b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            return a.PopsimGlobalEthicGroupId != b.PopsimGlobalEthicGroupId;
        }

        public override bool Equals(Object x)
        {
            if (x is null) return false;
            return this.PopsimGlobalEthicGroupId == ((PopsimGlobalEthicGroup)x).PopsimGlobalEthicGroupId;
        }

        public override int GetHashCode()
        {
            return this.PopsimGlobalEthicGroupId.GetHashCode();
        }
    }

    // Planets ------------------------------------------

    public class PopsimPlanetEthicGroup
    {
        [Key]
        public string PopsimPlanetEthicGroupId { get; set; } = Guid.NewGuid().ToString();

        public long MembersOnPlanet { get; set; }

        public float Percentage { get; set; }

        public string PopsimGlobalEthicGroupId { get; set; }
        public virtual PopsimGlobalEthicGroup PopsimGlobalEthicGroup { get; set; }

        public int PlanetId { get; set; }
        public virtual Planet Planet { get; set; }

        public override string ToString()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new EFResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, settings);
        }

        public static explicit operator PopsimPlanetEthicGroup(string m)
        {
            return JsonConvert.DeserializeObject<PopsimPlanetEthicGroup>(m);
        }

        public static bool operator ==(PopsimPlanetEthicGroup a, PopsimPlanetEthicGroup b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.PopsimPlanetEthicGroupId == b.PopsimPlanetEthicGroupId;
        }

        public static bool operator !=(PopsimPlanetEthicGroup a, PopsimPlanetEthicGroup b)
        {
            if (a is null && b is null) return false;
            if (a is null || b is null) return true;
            return a.PopsimPlanetEthicGroupId != b.PopsimPlanetEthicGroupId;
        }

        public override bool Equals(Object x)
        {
            if (x is null) return false;
            return this.PopsimPlanetEthicGroupId == ((PopsimPlanetEthicGroup)x).PopsimPlanetEthicGroupId;
        }

        public override int GetHashCode()
        {
            return this.PopsimPlanetEthicGroupId.GetHashCode();
        }
    }

    // Simulation -------------------------------------------
    public class Empire
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmpireId { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual List<GalacticObject> GalacticObjects { get; set; } = new List<GalacticObject>();
        public virtual List<Fleet> Fleets { get; set; } = new List<Fleet>();
        public virtual List<Army> Armies { get; set; } = new List<Army>();
        public virtual List<Fleet> MiningStations { get; set; } = new List<Fleet>();
        public virtual List<Fleet> ResearchStations { get; set; } = new List<Fleet>();

        public Dictionary<string, ulong> NationalOutput { get; set; } = new Dictionary<string, ulong>();
        public Dictionary<string, float> EconGmData { get; set; } = new Dictionary<string, float>();
        public Dictionary<Alignment, int> GeneralAssembly { get; set; } = new Dictionary<Alignment, int>();
        public Dictionary<PopsimGlobalEthicGroup, Dictionary<Alignment, float>> PopsimGmData { get; set; } = new Dictionary<PopsimGlobalEthicGroup, Dictionary<Alignment, float>>();
        public Dictionary<Alignment, float> GlobalAlignment { get; set; } = new Dictionary<Alignment, float>();
    }

    //celestial objects-----------------------------------
    public class GalacticObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GalacticObjectId { get; set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public virtual List<Planet> Planets { get; set; } = new List<Planet>();
        public Dictionary<int, float> Hyperlanes { get; set; }
        //public List<Hyperlane> Hyperlanes { get; set; } = new List<Hyperlane>();

        public int StarbaseId { get; set; }
        public virtual Starbase Starbase { get; set; }
    }

    public class Planet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlanetId { get; set; }
        public string PlanetName { get; set; }
        public string PlanetDescription { get; set; }
        public string PlanetClass { get; set; }
        public ulong Population { get; set; }

        public int OwnerId { get; set; }
        public virtual Empire Owner { get; set; }

        public int? ControllerId { get; set; }
        public virtual Empire Controller { get; set; }

        [Required]
        public virtual int GalacticObjectId { get; set; }
        public virtual GalacticObject GalacticObject { get; set; }

        public virtual List<Pop> Pops { get; set; }
        public virtual List<Building> Buildings { get; set; }
        public virtual List<District> Districts { get; set; }
        public virtual List<PopsimPlanetEthicGroup> PlanetGroups { get; set; } = new List<PopsimPlanetEthicGroup>();

        public Dictionary<string, ulong> Output { get; set; } = new Dictionary<string, ulong>();
        public Dictionary<string, float> EconGmData { get; set; } = new Dictionary<string, float>();
        public Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>> PopsimGmData { get; set; } = new Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>>();
        public Dictionary<Alignment, float> GlobalAlignment { get; set; } = new Dictionary<Alignment, float>();

        public virtual Alignment ExecutiveAlignment { get; set; }
        public virtual Alignment LegislativeAlignment { get; set; }
        public virtual Alignment PartyAlignment { get; set; }
    }

    public class District
    {
        [Key]
        public string DistrictId { get; set; }
        public string Type { get; set; }
        [Required]
        public virtual int PlanetId { get; set; }
        public virtual Planet Planet { get; set; }
    }

    public class Building
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BuildingId { get; set; }
        public string Type { get; set; }
        public bool Ruined { get; set; }
        [Required]
        public virtual int PlanetId { get; set; }
        public virtual Planet Planet { get; set; }
    }

    public class Pop
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PopId { get; set; }
        [Required]
        public virtual int PlanetId { get; set; }
        public virtual Planet Planet { get; set; }

        public string Species { get; set; }
        public string Job { get; set; }
        public string Strata { get; set; }
        public float Power { get; set; }
        public float Happiness { get; set; }

    }

    //military stuff -----------------------------------------------------------
    public class Starbase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StarbaseId { get; set; }
        public int Owner { get; set; }
        public string Level { get; set; }
        public virtual List<string> Modules { get; set; }
        public virtual List<string> Buildings { get; set; }
        [Required]
        public int FleetId { get; set; }
        public virtual Fleet StarbaseFleet { get; set; }
    }

    public class Fleet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FleetId { get; set; }
        public string Name { get; set; }
        // [Required]
        public int? OwnerID { get; set; }
        public virtual Empire Owner { get; set; }
        public double MilitaryPower { get; set; }
        [Required]
        public int SystemId { get; set; }
        // public virtual GalacticObject System { get; set; }
        public virtual List<Ship> Ships { get; set; } = new List<Ship>();
    }

    public class Ship
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ShipId { get; set; }
        [Required]
        public int FleetId { get; set; }
        public virtual Fleet Fleet { get; set; }
        public string ShipName { get; set; }
        public string Type { get; set; }
    }

    public class Army
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ArmyId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public virtual Empire Owner { get; set; }
        public int PlanetId { get; set; }
        public virtual Planet Planet { get; set; }
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
    public class InfrastructureData
    {
        [Key]
        public string InfraStructureDataId { get; set; } = Guid.NewGuid().ToString();
        public ulong GdpPerInfrastructure { get; set; }
        public Dictionary<string, Infrastructure> Infrastructures { get; set; } = new Dictionary<string, Infrastructure>();
    }

    // Voting ------------------------------------------
    public enum VoteType
    {
        MAJORITY,
        TWOTHIRD,
        FPTP,
        TWOROUND,
        TWOROUNDFINAL // For the second part of the two round
    }

    public class VoteMessage
    {
        [Key]
        public ulong MessageId { get; set; }
        public ulong CreatorId { get; set; }
        public ulong ChannelId { get; set; }
        public int Type { get; set; }
        public long EndTime { get; set; } // In FileTime
        public long TimeSpan { get; set; } // In Ticks
        public bool Anonymous { get; set; }
        public virtual List<VoteEntry> Votes { get; set; }
    }

    public class VoteEntry
    {
        [Key]
        public string VoteEntryId { get; set; } = Guid.NewGuid().ToString();

        public ulong VoteMessageId { get; set; }
        public virtual VoteMessage VoteMessage { get; set; }

        public int Vote { get; set; }
        public ulong UserId { get; set; }
    }

    // Galactic Map ------------------------------------------
    public class PlanetarySystem
    {
        [Key]
        public string SystemId { get; set; } = Guid.NewGuid().ToString();

        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Colour { get; set; }

        public int PlanetId { get; set; }
        public virtual Planet Planet { get; set; }
    }

    public class Data
    {
        [Key]
        public string DataId { get; set; } = Guid.NewGuid().ToString();
        public List<Strata> Stratas { get; set; } = new List<Strata>();
        public int BaseGdpPerPop { get; set; }
    }

    public class AutoAdvance
    {
        [Key]
        public string AutoAdvanceId { get; set; } = "1";
        public bool Enabled { get; set; } = false;
        public string DayExceptions { get; set; } = "0000000";
        public int AmountOfYears { get; set; }
        public DateTime LastDayTriggered { get; set; }

        public string ChannelId { get; set; }
    }

    public class GovernmentBranch
    {
        [Key]
        public string GovernmentId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public virtual Alignment PerceivedAlignment { get; set; }
        public float NationalModifier { get; set; }
        public Dictionary<PopsimGlobalEthicGroup, float> Modifiers { get; set; } = new Dictionary<PopsimGlobalEthicGroup, float>();
    }
}
