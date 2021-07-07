using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using DPSSimulation.Classes;

namespace BabelDatabase
{
    public class BabelContext : DbContext
    {
        public IConfiguration Configuration { get; }

        public BabelContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetValue<string>("Database:ConnectionString"));
            //optionsBuilder.UseInMemoryDatabase("test");
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();
        }

        private string SerializeDictInfra(Dictionary<string, Infrastructure> dict)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (string k in dict.Keys)
            {
                res.Add(k, JsonConvert.SerializeObject(dict[k]));
            }
            return JsonConvert.SerializeObject(res);
        }

        class Infr
        {
            public float InfrastructureWeight { get; set; }
            public Dictionary<string, float> InfrastructureIndustries { get; set; }
        }

        private Infrastructure DeSerializeInfra(string str)
        {
            Infr i = JsonConvert.DeserializeObject<Infr>(str);
            Infrastructure res = FormatterServices.GetUninitializedObject(typeof(Infrastructure)) as Infrastructure;
            res.InfrastructureIndustries = i.InfrastructureIndustries;
            res.InfrastructureWeight = i.InfrastructureWeight;
            return res;
        }

        private Dictionary<string, Infrastructure> DeSerializeDictInfra(string dictstr)
        {
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(dictstr);
            Dictionary<string, Infrastructure> res = new Dictionary<string, Infrastructure>();
            foreach (string k in dict.Keys)
            {
                res.Add(k, DeSerializeInfra(dict[k]));
            }
            return res;
        }

        class PseudoStrata
        {
            public int StrataWeight { get; set; }
            public Dictionary<string, float> StrataIndustries { get; set; }
            public Dictionary<string, string> StrataJobs { get; set; }
        }

        private string SerializeListStrata(List<Strata> list)
        {
            List<string> res = new List<string>();
            foreach (Strata s in list)
            {
                PseudoStrata pseudo = new PseudoStrata();
                pseudo.StrataWeight = s.StrataWeight;
                pseudo.StrataIndustries = s.StrataIndustries;
                pseudo.StrataJobs = s.StrataJobs.ToDictionary((k) => k.Key, v => JsonConvert.SerializeObject(v.Value));
                res.Add(JsonConvert.SerializeObject(pseudo));
            }

            return JsonConvert.SerializeObject(res);
        }

        class Jorb
        {
            public float JobWeight { get; set; }
            public Dictionary<string, float> JobIndustries { get; set; }
        }

        private Job DeSerializeJob(string str)
        {
            Jorb i = JsonConvert.DeserializeObject<Jorb>(str);
            Job res = FormatterServices.GetUninitializedObject(typeof(Job)) as Job;
            res.JobWeight = i.JobWeight;
            res.JobIndustries = i.JobIndustries;
            return res;
        }

        private List<Strata> DeSerializeListStrata(string str)
        {
            List<string> list = JsonConvert.DeserializeObject<List<string>>(str);
            List<Strata> res = new List<Strata>();
            foreach (string s in list)
            {
                PseudoStrata pseudo = JsonConvert.DeserializeObject<PseudoStrata>(s);
                Strata strata = FormatterServices.GetUninitializedObject(typeof(Strata)) as Strata; // Sure I could've asked to just add a blank constructor to Strata, but it's funnier this way.
                strata.StrataWeight = pseudo.StrataWeight;
                strata.StrataIndustries = pseudo.StrataIndustries;
                strata.StrataJobs = pseudo.StrataJobs.ToDictionary((k) => k.Key, v => DeSerializeJob(v.Value));
                res.Add(strata);
            }

            return res;
        }

        public DbSet<Alignment> Alignments { get; set; }
        public DbSet<AlignmentClique> AlignmentClique { get; set; }
        public DbSet<AlignmentSpending> AlignmentSpendings { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Clique> Cliques { get; set; }
        public DbSet<CliqueMemberCharacter> CliqueMemberCharacter { get; set; }
        public DbSet<CliqueOfficerCharacter> CliqueOfficerCharacter { get; set; }
        public DbSet<CliqueInvite> CliqueInvites { get; set; }
        public DbSet<CustomSpending> CustomSpendings { get; set; }
        public DbSet<DiscordUser> DiscordUsers { get; set; }
        public DbSet<Graveyard> Graveyards { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<StaffAction> StaffActions { get; set; }
        public DbSet<StaffActionPost> StaffActionPosts { get; set; }
        public DbSet<GameState> GameState { get; set; }
        public DbSet<CharacterDeathTimer> CharacterDeathTimers { get; set; }
        public DbSet<PopsimGlobalEthicGroup> PopsimGlobalEthicGroups { get; set; }

        public DbSet<Empire> Empires { get; set; }
        public DbSet<GalacticObject> GalacticObjects { get; set; }
        public DbSet<Planet> Planets { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Pop> Pops { get; set; }
        public DbSet<Starbase> Starbases { get; set; }
        public DbSet<Fleet> Fleets { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Army> Armies { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<Military> Militaries { get; set; }
        public DbSet<InfrastructureData> InfrastructureData { get; set; }
        public DbSet<Data> Data { get; set; }


        //public DbSet<PopsimReport> PopsimReports { get; set; }

        //public DbSet<PopsimGlobalEthicGroup> PopsimGlobalEthicGroups { get; set; }
        public DbSet<VoteMessage> VoteMessages { get; set; }
        public DbSet<VoteEntry> VoteEntries { get; set; }
        public DbSet<PlanetarySystem> PlanetarySystems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscordUser>().HasData(new DiscordUser()
            {
                DiscordUserId = "75968535074967552",
                UserName = "Obi",
                IsAdmin = true
            });

            modelBuilder.Entity<Species>().HasData(
                new Species
                {
                    SpeciesId = "1",
                    SpeciesName = "Human"
                },
                new Species
                {
                    SpeciesId = "2",
                    SpeciesName = "Zelvan"
                },
                new Species
                {
                    SpeciesId = "3",
                    SpeciesName = "Liaran"
                }
            );

            modelBuilder.Entity<Alignment>().HasData(
                new Alignment
                {
                    AlignmentId = "123",
                    AlignmentName = "ULTRA COMMIE LIBERTARIANS",
                    FederalismCentralism = 5,
                    DemocracyAuthority = 1,
                    GlobalismIsolationism = 0,
                    MilitarismPacifism = 8,
                    SecurityFreedom = 4,
                    CooperationCompetition = 3,
                    SecularismSpiritualism = 7,
                    ProgressivismTraditionalism = 9,
                    MonoculturalismMulticulturalism = 4
                },
                new Alignment
                {
                    AlignmentId = "124",
                    AlignmentName = "ULTRA LIBERTARIAN COMMIES",
                    FederalismCentralism = 2,
                    DemocracyAuthority = 3,
                    GlobalismIsolationism = 4,
                    MilitarismPacifism = 2,
                    SecurityFreedom = 6,
                    CooperationCompetition = 1,
                    SecularismSpiritualism = 0,
                    ProgressivismTraditionalism = 4,
                    MonoculturalismMulticulturalism = 3
                }
            );

            modelBuilder.Entity<GameState>().HasData(
                new GameState()
            );

            modelBuilder.Entity<DiscordUser>()
                .HasMany(du => du.Characters)
                .WithOne(c => c.DiscordUser)
                .HasForeignKey(c => c.DiscordUserId);

            modelBuilder.Entity<Character>()
                .HasOne(c => c.Species)
                .WithMany()
                .HasForeignKey(c => c.SpeciesId);

            modelBuilder.Entity<CharacterDeathTimer>()
                .HasOne(cdt => cdt.Character)
                .WithOne()
                .HasForeignKey<CharacterDeathTimer>(cdt => cdt.CharacterId);

            modelBuilder.Entity<CliqueMemberCharacter>().HasKey(cmc => new { cmc.CliqueId, cmc.MemberId });

            modelBuilder.Entity<CliqueMemberCharacter>()
                .HasOne(cmc => cmc.Clique)
                .WithMany(c => c.CliqueMemberCharacter);

            modelBuilder.Entity<CliqueMemberCharacter>()
                .HasOne(cmc => cmc.Member)
                .WithMany(c => c.Cliques);

            modelBuilder.Entity<CliqueOfficerCharacter>().HasKey(cmc => new { cmc.CliqueId, cmc.OfficerId });

            modelBuilder.Entity<CliqueOfficerCharacter>()
                .HasOne(cmc => cmc.Clique)
                .WithMany(c => c.CliqueOfficerCharacter);

            modelBuilder.Entity<CliqueOfficerCharacter>()
                .HasOne(cmc => cmc.Officer)
                .WithMany();

            modelBuilder.Entity<AlignmentClique>().HasKey(ac => new { ac.CliqueId, ac.AlignmentId });

            modelBuilder.Entity<AlignmentClique>()
                .HasOne(ac => ac.Alignment)
                .WithMany(a => a.AlignmentClique);

            modelBuilder.Entity<AlignmentClique>()
                .HasOne(ac => ac.Clique)
                .WithMany(c => c.Alignments);

            // Staff action stuff

            modelBuilder.Entity<PlayerStaffAction>().HasKey(psa => new { psa.PlayerId, psa.StaffActionId });

            modelBuilder.Entity<PlayerStaffAction>()
                .HasOne(psa => psa.Player)
                .WithMany()
                .HasForeignKey(psa => psa.PlayerId);

            modelBuilder.Entity<PlayerStaffAction>()
                .HasOne(psa => psa.StaffAction)
                .WithMany(sa => sa.Players)
                .HasForeignKey(psa => psa.PlayerId);

            modelBuilder.Entity<StaffStaffAction>().HasKey(ssa => new { ssa.StaffId, ssa.StaffActionId });

            modelBuilder.Entity<StaffStaffAction>()
                .HasOne(ssa => ssa.Staff)
                .WithMany()
                .HasForeignKey(ssa => ssa.StaffId);

            modelBuilder.Entity<StaffStaffAction>()
                .HasOne(ssa => ssa.StaffAction)
                .WithMany(sa => sa.Staff)
                .HasForeignKey(ssa => ssa.StaffActionId);

            modelBuilder.Entity<StaffAction>()
                .HasMany(sa => sa.StaffActionPosts)
                .WithOne(sap => sap.StaffAction)
                .HasForeignKey(sap => sap.StaffActionId);

            modelBuilder.Entity<StaffAction>()
                .HasOne(sa => sa.Owner)
                .WithMany()
                .HasForeignKey(sa => sa.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StaffActionPost>()
                .HasOne(sap => sap.Author)
                .WithMany()
                .HasForeignKey(sap => sap.AuthorId);

            modelBuilder.Entity<Committee>()
                .HasMany(c => c.CommitteeMembers);

            modelBuilder.Entity<CliqueInvite>()
                .HasOne(ci => ci.Clique)
                .WithMany()
                .HasForeignKey(ci => ci.CliqueId);

            modelBuilder.Entity<AlignmentSpending>()
                .HasOne(alsp => alsp.Clique)
                .WithMany()
                .HasForeignKey(alsp => alsp.CliqueId);

            modelBuilder.Entity<AlignmentSpending>()
                .HasOne(alsp => alsp.Character)
                .WithMany()
                .HasForeignKey(alsp => alsp.CharacterId);

            modelBuilder.Entity<AlignmentSpending>()
                .HasOne(alsp => alsp.Alignment)
                .WithMany()
                .HasForeignKey(alsp => alsp.AlignmentId);

            modelBuilder.Entity<AlignmentSpending>()
                .HasOne(alsp => alsp.PlanetTarget)
                .WithMany()
                .HasForeignKey(alsp => alsp.PlanetTargetId);

            modelBuilder.Entity<AlignmentSpending>()
                .HasOne(alsp => alsp.GlobalTarget)
                .WithMany()
                .HasForeignKey(alsp => alsp.GlobalTargetId);

            modelBuilder.Entity<PopsimGlobalEthicGroup>()
                .HasMany(pgeg => pgeg.PlanetaryEthicGroups)
                .WithOne(ppeg => ppeg.PopsimGlobalEthicGroup)
                .HasForeignKey(ppeg => ppeg.PopsimGlobalEthicGroupId);

            modelBuilder.Entity<GalacticObject>()
                .HasMany(go => go.Planets)
                .WithOne(p => p.GalacticObject)
                .HasForeignKey(p => p.GalacticObjectId);

            modelBuilder.Entity<GalacticObject>()
                .HasOne(go => go.Starbase)
                .WithOne()
                .HasForeignKey<GalacticObject>(go => go.StarbaseId);

            modelBuilder.Entity<Planet>()
                .HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Planet>()
                .HasOne(p => p.Controller)
                .WithMany()
                .HasForeignKey(p => p.ControllerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Planet>()
                .HasMany(p => p.Pops)
                .WithOne(p => p.Planet)
                .HasForeignKey(p => p.PlanetId);

            modelBuilder.Entity<Planet>()
                .HasMany(p => p.Buildings)
                .WithOne(b => b.Planet)
                .HasForeignKey(b => b.PlanetId);

            modelBuilder.Entity<Planet>()
                .HasMany(p => p.Districts)
                .WithOne(d => d.Planet)
                .HasForeignKey(d => d.PlanetId);

            modelBuilder.Entity<Planet>()
                .HasMany(p => p.PlanetGroups)
                .WithOne(ppeg => ppeg.Planet)
                .HasForeignKey(ppeg => ppeg.PlanetId);

            modelBuilder.Entity<Empire>()
                .HasMany(e => e.GalacticObjects)
                .WithOne();

            modelBuilder.Entity<Empire>()
                .HasMany(e => e.Fleets)
                .WithOne(f => f.Owner)
                .HasForeignKey(f => f.OwnerID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Empire>()
                .HasMany(e => e.Armies)
                .WithOne(a => a.Owner)
                .HasForeignKey(a => a.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Empire>()
                .HasMany(e => e.MiningStations)
                .WithOne();

            modelBuilder.Entity<Empire>()
                .HasMany(e => e.ResearchStations)
                .WithOne();

            modelBuilder.Entity<Fleet>()
                .HasMany(f => f.Ships)
                .WithOne(s => s.Fleet)
                .HasForeignKey(s => s.FleetId);

            /*modelBuilder.Entity<Fleet>()
                .HasOne(f => f.System)
                .WithMany()
                .HasForeignKey(f => f.SystemId)
                .OnDelete(DeleteBehavior.NoAction);*/

            modelBuilder.Entity<Army>()
                .HasOne(a => a.Planet)
                .WithMany()
                .HasForeignKey(a => a.PlanetId);

            modelBuilder.Entity<Starbase>()
                .HasOne(s => s.StarbaseFleet)
                .WithOne()
                .HasForeignKey<Starbase>(s => s.FleetId);

            // -------

            modelBuilder.Entity<InfrastructureData>()
                .Property(b => b.Infrastructures)
                .HasConversion(
                    v => SerializeDictInfra(v),
                    v => DeSerializeDictInfra(v));

            modelBuilder.Entity<Data>()
                .Property(b => b.Stratas)
                .HasConversion(
                    v => SerializeListStrata(v),
                    v => DeSerializeListStrata(v));

            modelBuilder.Entity<Empire>()
                .Property(b => b.NationalOutput)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, ulong>>(v));

            modelBuilder.Entity<Empire>()
                .Property(b => b.EconGmData)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, float>>(v));

            modelBuilder.Entity<Empire>()
                .Property(b => b.PopsimGmData)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, Dictionary<Alignment, float>>>(v));

            modelBuilder.Entity<Empire>()
                .Property(b => b.GeneralAssembly)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<Alignment, int>>(v));

            modelBuilder.Entity<Planet>()
                .Property(b => b.Output)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, ulong>>(v));

            modelBuilder.Entity<Planet>()
                .Property(b => b.EconGmData)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, float>>(v));

            modelBuilder.Entity<Planet>()
                .Property(b => b.PopsimGmData)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>>>(v));

            modelBuilder.Entity<Party>()
                .Property(b => b.PopGroupEnlistment)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));

            modelBuilder.Entity<Party>()
                .Property(b => b.UpperPartyMembership)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));

            modelBuilder.Entity<Party>()
                .Property(b => b.LowerPartyMembership)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));

            modelBuilder.Entity<Party>()
                .Property(b => b.UpperPartyAffinity)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<Alignment, float>>(v));

            modelBuilder.Entity<Party>()
                .Property(b => b.LowerPartyAffinity)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<Alignment, float>>(v));

            modelBuilder.Entity<Military>()
                .Property(b => b.MilitaryGroups)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));

            modelBuilder.Entity<Military>()
                .Property(b => b.MilitaryFactions)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<Alignment, float>>(v));

            modelBuilder.Entity<GalacticObject>()
                .Property(go => go.Hyperlanes)
                .HasConversion(
                    h => JsonConvert.SerializeObject(h),
                    h => JsonConvert.DeserializeObject<Dictionary<int, float>>(h));

            modelBuilder.Entity<Starbase>()
                .Property(s => s.Modules)
                .HasConversion(
                    s => JsonConvert.SerializeObject(s),
                    s => JsonConvert.DeserializeObject<List<string>>(s));

            modelBuilder.Entity<Starbase>()
                .Property(s => s.Buildings)
                .HasConversion(
                    b => JsonConvert.SerializeObject(b),
                    b => JsonConvert.DeserializeObject<List<string>>(b));

            modelBuilder.Entity<VoteEntry>()
                .HasOne(ve => ve.VoteMessage)
                .WithMany(vm => vm.Votes)
                .HasForeignKey(vm => vm.VoteMessageId);

            modelBuilder.Entity<PlanetarySystem>()
                .HasOne(ps => ps.Planet)
                .WithOne()
                .HasForeignKey<PlanetarySystem>(ps => ps.PlanetId);

            modelBuilder.Entity<Planet>()
                .HasOne(p => p.ExecutiveAlignment);

            modelBuilder.Entity<Planet>()
                .HasOne(p => p.LegislativeAlignment);

            modelBuilder.Entity<Planet>()
                .HasOne(p => p.PartyAlignment);
        }
    }
}
