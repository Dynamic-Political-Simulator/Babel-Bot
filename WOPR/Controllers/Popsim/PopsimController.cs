using BabelDatabase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using WOPR.Helpers;
using WOPR.Services;

namespace WOPR.Controllers.Popsim
{
    [Route("api/[controller]")]
    [ApiController]
    public class PopsimController : ControllerBase
    {
        private readonly BabelContext _context;
        private readonly EconPopsimServices _econ;

        public PopsimController(BabelContext context, EconPopsimServices econ)
        {
            _context = context;
            _econ = econ;
        }

        public struct PopsimPartyOverviewReturn
        {
            public float UpperPartyPercentage { get; set; }
            public List<PopsimOverviewGlobalEthicGroup> GlobalEthicGroups { get; set; }
            public List<PopsimOverviewAlignment> Alignments { get; set; }
        }

        public struct PopsimOverviewGlobalEthicGroup
        {
            public string Name { get; set; }
            public int PartyInvolvementFactor { get; set; }
            public float Radicalisation { get; set; }
            public float PartyEnlistmentModifier { get; set; }

            public int FederalismCentralism { get; set; }
            public int DemocracyAuthority { get; set; }
            public int GlobalismIsolationism { get; set; }
            public int MilitarismPacifism { get; set; }
            public int SecurityFreedom { get; set; }
            public int CooperationCompetition { get; set; }
            public int SecularismSpiritualism { get; set; }
            public int ProgressivismTraditionalism { get; set; }
            public int MonoculturalismMulticulturalism { get; set; }

            public float PopGroupEnlistment { get; set; }
            public float UpperPartyMembership { get; set; }
            public float LowerPartyMembership { get; set; }
        }

        public struct PopsimOverviewAlignment
        {
            public string Name { get; set; }

            public int FederalismCentralism { get; set; }
            public int DemocracyAuthority { get; set; }
            public int GlobalismIsolationism { get; set; }
            public int MilitarismPacifism { get; set; }
            public int SecurityFreedom { get; set; }
            public int CooperationCompetition { get; set; }
            public int SecularismSpiritualism { get; set; }
            public int ProgressivismTraditionalism { get; set; }
            public int MonoculturalismMulticulturalism { get; set; }

            public float Establishment { get; set; }
            public float UpperPartyModifier { get; set; }
            public float LowerPartyModifier { get; set; }
            public float UpperPartyAffinity { get; set; }
            public float LowerPartyAffinity { get; set; }

            public List<string> Cliques { get; set; }
        }

        [HttpGet("overview")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult GetPopsimPartyOverview()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || !discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            var party = _context.Parties.First();

            var overview = new PopsimPartyOverviewReturn();

            overview.GlobalEthicGroups = new List<PopsimOverviewGlobalEthicGroup>();
            overview.Alignments = new List<PopsimOverviewAlignment>();

            foreach (var enlistment in party.PopGroupEnlistment)
            {
                var ppeg = enlistment.Key;
                var newEntry = new PopsimOverviewGlobalEthicGroup()
                {
                    Name = ppeg.PopsimGlobalEthicGroupName,
                    FederalismCentralism = ppeg.FederalismCentralism,
                    DemocracyAuthority = ppeg.DemocracyAuthority,
                    GlobalismIsolationism = ppeg.GlobalismIsolationism,
                    MilitarismPacifism = ppeg.MilitarismPacifism,
                    SecurityFreedom = ppeg.SecurityFreedom,
                    CooperationCompetition = ppeg.CooperationCompetition,
                    SecularismSpiritualism = ppeg.SecularismSpiritualism,
                    ProgressivismTraditionalism = ppeg.ProgressivismTraditionalism,
                    MonoculturalismMulticulturalism = ppeg.MonoculturalismMulticulturalism,
                    PopGroupEnlistment = enlistment.Value
                };
            }

            foreach (var upperMembership in party.UpperPartyMembership)
            {
                var entry = overview.GlobalEthicGroups.SingleOrDefault(geg => geg.Name == upperMembership.Key.PopsimGlobalEthicGroupName);
                entry.UpperPartyMembership = upperMembership.Value;
            }

            foreach (var lowerMembership in party.LowerPartyMembership)
            {
                var entry = overview.GlobalEthicGroups.SingleOrDefault(geg => geg.Name == lowerMembership.Key.PopsimGlobalEthicGroupName);
                entry.LowerPartyMembership = lowerMembership.Value;
            }

            foreach (var lowerAffinity in party.LowerPartyAffinity)
            {
                var a = lowerAffinity.Key;
                var newEntry = new PopsimOverviewAlignment()
                {
                    Name = a.AlignmentName,
                    FederalismCentralism = a.FederalismCentralism,
                    DemocracyAuthority = a.DemocracyAuthority,
                    GlobalismIsolationism = a.GlobalismIsolationism,
                    MilitarismPacifism = a.MilitarismPacifism,
                    SecurityFreedom = a.SecurityFreedom,
                    CooperationCompetition = a.CooperationCompetition,
                    SecularismSpiritualism = a.SecularismSpiritualism,
                    ProgressivismTraditionalism = a.ProgressivismTraditionalism,
                    MonoculturalismMulticulturalism = a.MonoculturalismMulticulturalism,
                    Establishment = a.Establishment,
                    LowerPartyModifier = a.LowerPartyModiifer,
                    UpperPartyModifier = a.UpperPartyModifier,
                    LowerPartyAffinity = lowerAffinity.Value,
                    Cliques = a.AlignmentClique.Select(ac => ac.Clique.CliqueName).ToList()
                };
            }

            foreach (var upperAffinity in party.UpperPartyAffinity)
            {
                var entry = overview.Alignments.SingleOrDefault(a => a.Name == upperAffinity.Key.AlignmentName);
                entry.UpperPartyAffinity = upperAffinity.Value;
            }

            return Ok(overview);
        }

        [HttpGet("get-ggroups")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult GetPopsimGlobalGroups()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || !discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            PopsimOverviewGlobalEthicGroup[] greups = new PopsimOverviewGlobalEthicGroup[_context.PopsimGlobalEthicGroups.Count()];
            int x = 0;
            foreach (PopsimGlobalEthicGroup ppeg in _context.PopsimGlobalEthicGroups)
            {
                greups[x] = new PopsimOverviewGlobalEthicGroup()
                {
                    Name = ppeg.PopsimGlobalEthicGroupName,
                    FederalismCentralism = ppeg.FederalismCentralism,
                    DemocracyAuthority = ppeg.DemocracyAuthority,
                    GlobalismIsolationism = ppeg.GlobalismIsolationism,
                    MilitarismPacifism = ppeg.MilitarismPacifism,
                    SecurityFreedom = ppeg.SecurityFreedom,
                    CooperationCompetition = ppeg.CooperationCompetition,
                    SecularismSpiritualism = ppeg.SecularismSpiritualism,
                    ProgressivismTraditionalism = ppeg.ProgressivismTraditionalism,
                    MonoculturalismMulticulturalism = ppeg.MonoculturalismMulticulturalism,
                    PopGroupEnlistment = ppeg.PartyEnlistmentModifier,
                    PartyInvolvementFactor = ppeg.PartyInvolvementFactor,
                    Radicalisation = ppeg.Radicalisation
                };
                x++;
            }

            return Ok(greups);
        }

        [HttpPost("set-group")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult SetPopsimGlobalGroup([FromBody] PopsimOverviewGlobalEthicGroup ppeg)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || !discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            PopsimGlobalEthicGroup pp = _context.PopsimGlobalEthicGroups.FirstOrDefault(x => x.PopsimGlobalEthicGroupName == ppeg.Name);

            if (pp is null)
            {
                return BadRequest();
            }

            pp.Radicalisation = ppeg.Radicalisation;
            pp.PartyInvolvementFactor = ppeg.PartyInvolvementFactor;
            pp.PartyEnlistmentModifier = ppeg.PopGroupEnlistment;
            pp.FederalismCentralism = ppeg.FederalismCentralism;
            pp.DemocracyAuthority = ppeg.DemocracyAuthority;
            pp.GlobalismIsolationism = ppeg.GlobalismIsolationism;
            pp.MilitarismPacifism = ppeg.MilitarismPacifism;
            pp.SecurityFreedom = ppeg.SecurityFreedom;
            pp.CooperationCompetition = ppeg.CooperationCompetition;
            pp.SecularismSpiritualism = ppeg.SecularismSpiritualism;
            pp.ProgressivismTraditionalism = ppeg.ProgressivismTraditionalism;
            pp.MonoculturalismMulticulturalism = ppeg.MonoculturalismMulticulturalism;

            _context.PopsimGlobalEthicGroups.Update(pp);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("make-gov")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult MakeGovBranch()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || !discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            GovernmentBranch gb = new GovernmentBranch();
            gb.Name = "Untitled";
            gb.PerceivedAlignment = _context.Alignments.FirstOrDefault();
            gb.NationalModifier = 0;



            _context.GovernmentBranches.Add(gb);
            _context.SaveChanges();

            return Ok();
        }

        public struct GovEdit
        {
            public string Id;
            public string Name;
            public string PerceivedAlignment;
            public float NatMod;
            public Dictionary<string, float> Modifier;
        }

        [HttpGet("get-gov-edit")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult GetGovBranchEdit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || !discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            GovEdit[] gba = _context.GovernmentBranches.ToList().Select(x => new GovEdit()
            {
                Id = x.GovernmentId,
                Name = x.Name,
                PerceivedAlignment = x.PerceivedAlignment.AlignmentName,
                NatMod = x.NationalModifier,
                Modifier = x.Modifiers.ToDictionary(k => k.Key.PopsimGlobalEthicGroupName, v => v.Value)
            }).ToArray();

            return Ok(gba);
        }

        [HttpPost("set-gov")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult SetGovBranch(GovEdit ge)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || !discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            GovernmentBranch gb = _context.GovernmentBranches.FirstOrDefault(x => x.GovernmentId == ge.Id);

            if (gb is null)
            {
                return NotFound();
            }
            Alignment a = _context.Alignments.FirstOrDefault(x => x.AlignmentName == ge.PerceivedAlignment);
            if (a is null)
            {
                return BadRequest();
            }

            gb.Name = ge.Name;
            gb.PerceivedAlignment = a;
            gb.NationalModifier = ge.NatMod;
            gb.Modifiers = ge.Modifier.ToDictionary(x => _context.PopsimGlobalEthicGroups.First(y => y.PopsimGlobalEthicGroupName == x.Key), x => x.Value);

            _context.GovernmentBranches.Update(gb);
            _context.SaveChanges();

            return Ok();
        }

        struct GovBrancher
        {
            public string Name;
            public string Alignment;
            public float Popularity;
        }

        [HttpGet("get-gov")]
        public IActionResult GetGovBranches()
        {
            Empire empire = _context.Empires.Single(x => x.EmpireId == 1);
            List<GovBrancher> gbl = new List<GovBrancher>();
            List<float> popularities = _econ.CalculateBranchPopularity(empire, _context.GovernmentBranches.ToList());
            foreach (GovernmentBranch gb in _context.GovernmentBranches.ToList())
            {
                GovBrancher gbe = new GovBrancher()
                {
                    Name = gb.Name,
                    Alignment = gb.PerceivedAlignment.AlignmentName,
                    Popularity = popularities[_context.GovernmentBranches.ToList().IndexOf(gb)]
                };
                gbl.Add(gbe);
            }
            return Ok(gbl.ToArray());
        }

        struct PartySend
        {
            public float OverallPartyEnlistment;
            public float PercentageOfEmpire;
            public Dictionary<string, float> PerGroupEnlistment;
            public float UpperPartyMembership;
            public float LowerPartyMembership;
            public string UpperAlignment;
            public string LowerAlignment;
            public string UpperDominantFaction;
            public string LowerDominantFaction;
            public float? UpperPercentage;
        }

        [HttpGet("get-party")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> GetParty()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            Party party = _context.Parties.FirstOrDefault(); // so long as it's a one-party state, we fine;
            Empire empire = _context.Empires.Single(x => x.EmpireId == 1);

            ulong population = 0;

            foreach (GalacticObject system in empire.GalacticObjects)
            {
                foreach (Planet planet in system.Planets.Where(p => p.Population != 0))
                {
                    population += planet.Population;

                }
            }

            if (party is null)
            {
                party = new Party();
                party.UpperPartyPercentage = 0.15f;
                _context.Parties.Add(party);
                await _context.SaveChangesAsync();
            }

            // await _econ.CalculateParty(party, empire);

            Dictionary<PopsimGlobalEthicGroup, float> grouoPop = new Dictionary<PopsimGlobalEthicGroup, float>();

            foreach (GalacticObject system in empire.GalacticObjects)
            {
                foreach (Planet planet in system.Planets.Where(p => p.Population != 0))
                {
                    double populationPercentage = (double)((decimal)planet.Population / (decimal)population);

                    if (planet.PlanetGroups.Count != 0)
                    {
                        foreach (PopsimPlanetEthicGroup group in planet.PlanetGroups)
                        {
                            float groupPerc = group.Percentage / 100f;
                            KeyValuePair<PopsimGlobalEthicGroup, float> ge = grouoPop.FirstOrDefault(x => x.Key.PopsimGlobalEthicGroupId == group.PopsimGlobalEthicGroup.PopsimGlobalEthicGroupId);
                            if (ge.Equals(default(KeyValuePair<PopsimGlobalEthicGroup, float>)))
                            {
                                grouoPop.Add(group.PopsimGlobalEthicGroup, (float)(groupPerc * populationPercentage));
                            }
                            else
                            {
                                grouoPop[grouoPop.FirstOrDefault(x => x.Key.PopsimGlobalEthicGroupId == group.PopsimGlobalEthicGroup.PopsimGlobalEthicGroupId).Key] += (float)(groupPerc * populationPercentage);
                            }

                        }
                    }
                }
            }

            // Console.WriteLine(grouoPop.First().Value);

            float overall = party.PopGroupEnlistment.Sum(x => x.Value * grouoPop[x.Key]);

            // Console.WriteLine(population);
            // Console.WriteLine(overall);

            PartySend ps = new PartySend()
            {
                OverallPartyEnlistment = overall * population,
                PercentageOfEmpire = overall,
                PerGroupEnlistment = party.PopGroupEnlistment.ToDictionary(k => k.Key.PopsimGlobalEthicGroupName, v => v.Value),
                UpperPartyMembership = overall * party.UpperPartyPercentage * population,
                LowerPartyMembership = overall * (1 - party.UpperPartyPercentage) * population,
                UpperAlignment = party.UpperPartyAffinity.Any(k => k.Value > 0.4f) ? party.UpperPartyAffinity.ToList().OrderBy(x => x.Value).Last().Key.AlignmentName : "None",
                LowerAlignment = party.LowerPartyAffinity.Any(k => k.Value > 0.4f) ? party.LowerPartyAffinity.ToList().OrderBy(x => x.Value).Last().Key.AlignmentName : "None",
                UpperDominantFaction = party.UpperPartyMembership.ToList().OrderBy(x => x.Value).Last().Key.PopsimGlobalEthicGroupName,
                LowerDominantFaction = party.LowerPartyMembership.ToList().OrderBy(x => x.Value).Last().Key.PopsimGlobalEthicGroupName,
                UpperPercentage = discordUser == null || !discordUser.IsAdmin ? (float?)null : party.UpperPartyPercentage
            };

            _context.Parties.Update(party);
            _context.SaveChanges();

            return Ok(ps);
        }

        public struct PartyReceive
        {
            public float UpperPercentage;
        }

        [HttpPost("set-party")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult SetParty([FromBody] PartyReceive pr)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || !discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            Party party = _context.Parties.FirstOrDefault(); // so long as it's a one-party state, we fine;

            if (party is null)
            {
                party = new Party();
                _context.Parties.Add(party);
            }

            party.UpperPartyPercentage = pr.UpperPercentage;

            _context.Parties.Update(party);
            _context.SaveChanges();

            return Ok();
        }
    }
}
