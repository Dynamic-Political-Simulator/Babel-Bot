using BabelDatabase;
using WOPR.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WOPR.Controllers.Map
{
    [Route("api/map")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly BabelContext _context;
        private readonly EconPopsimServices _econ;

        public MapController(BabelContext context, EconPopsimServices econ)
        {
            _context = context;
            _econ = econ;
        }

        class PlanetInfo
        {
            public string Name;
            public string Description;
            public string Colour;
            public float[] Location;
        }

        [HttpGet("get")]
        public IActionResult GetMap()
        {
            // Get all of the planets
            PlanetarySystem[] systems = _context.PlanetarySystems.ToArray();
            PlanetInfo[] info = new PlanetInfo[systems.Length];

            for (int x = 0; x < systems.Length; x++)
            {
                info[x] = new PlanetInfo();
                info[x].Name = systems[x].Planet.PlanetName.Replace("\"", "");
                info[x].Description = systems[x].Planet.PlanetDescription; // NOTE: Potentially optimise load speeds by fetching the desc on tooltip open
                info[x].Colour = systems[x].Colour;
                info[x].Location = new float[] { systems[x].Lat, systems[x].Lng };
            }

            return Ok(info);
        }

        public class PlanetAddForm
        {
            public string PlanetName;
            public float[] Location;
            public string Colour;
        }

        [HttpPost("add")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult AddPlanetToMap([FromBody] PlanetAddForm form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            Planet planet = _context.Planets.Where(x => x.PlanetName == "\"" + form.PlanetName + "\"" && !x.PlanetClass.Contains("star")).SingleOrDefault();

            if (planet == null || form.Location.Length != 2 || form.Colour == null)
            {
                return BadRequest();
            }

            PlanetarySystem system = new PlanetarySystem();
            system.Colour = form.Colour;
            system.Lat = form.Location[0];
            system.Lng = form.Location[1];
            system.Planet = planet;
            if (_context.PlanetarySystems.Any(x => x.Planet == planet))
                _context.PlanetarySystems.Update(system);
            else
                _context.PlanetarySystems.Add(system);
            _context.SaveChanges();

            return Ok();
        }

        public class PlanetCreateForm
        {
            public string PlanetName;
            public string PlanetDescription;
        }

        [HttpPost("edit-planet-description")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult EditPlanetDescription([FromBody] PlanetCreateForm form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            Planet planet = _context.Planets.Where(x => x.PlanetName == "\"" + form.PlanetName + "\"" && !x.PlanetClass.Contains("star")).SingleOrDefault();
            planet.PlanetDescription = form.PlanetDescription;
            _context.Planets.Update(planet);
            _context.SaveChanges();

            return Ok();
        }

        public class GroupEntry
        {
            public string Name;
            public float Size;
            public Dictionary<string, float> Modifier; // Null if not admin
        }
        public class IndustryEntry
        {
            public string Name;
            public ulong GDP;
            public float? Modifier; // Null if not admin
        }
        public class SpeciesEntry
        {
            public string Name;
            public float Amount;
        }
        public class PopularityEntry
        {
            public string Name;
            public float Popularity;
        }
        public class AlignmentMod
        {
            public string Name;
            public float Mod;
        }
        public class PlanetReturn
        {
            public string Name;
            public ulong Population;
            public SpeciesEntry[] Species;
            public string[] OfficeAlignments;
            public GroupEntry[] GroupEntries;
            public IndustryEntry[] IndustryEntries;
            public PopularityEntry[] PopularityEntries;
            public AlignmentMod[] AlignmentModifiers;
        }

        public class EmpireReturn
        {
            public string Name;
            public ulong Population;
            public ulong TotalGdp;
            public SpeciesEntry[] Species;
            public GroupEntry[] GroupEntries;
            public IndustryEntry[] IndustryEntries;
            public IndustryEntry[] SpaceIndustryEntries;
            public PopularityEntry[] PopularityEntries;
            public PopularityEntry[] ParlamentEntries;
        }

        [HttpGet("get-empire")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> GetEmpire(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            Empire empire = _context.Empires.Where(e => e.Name == "\"" + name + "\"").SingleOrDefault();
            if (empire == null)
            {
                return NotFound();
            }

            EmpireReturn replyRaw = new EmpireReturn();
            // await _econ.CalculateEmpireEcon(empire);
            replyRaw.Name = empire.Name.Replace("\"", "");
            ulong population = 0;
            List<GroupEntry> GroupEntries = new List<GroupEntry>();
            List<SpeciesEntry> speciesEntries = new List<SpeciesEntry>();

            foreach (GalacticObject system in empire.GalacticObjects)
            {
                foreach (Planet planet in system.Planets.Where(p => p.Population != 0))
                {
                    population += planet.Population;

                }
            }
            replyRaw.Population = population;

            if (population == 0)
            {
                return NotFound(); // This empire has no data so it "doesn't exist".
            }

            foreach (GalacticObject system in empire.GalacticObjects)
            {
                foreach (Planet planet in system.Planets.Where(p => p.Population != 0))
                {
                    double populationPercentage = (double)((decimal)planet.Population / (decimal)population);

                    if (planet.PlanetGroups.Count != 0)
                    {
                        foreach (PopsimPlanetEthicGroup group in planet.PlanetGroups)
                        {

                            GroupEntry ge = GroupEntries.FirstOrDefault(g => g.Name == group.PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName);
                            if (ge == null)
                            {
                                ge = new GroupEntry();
                                ge.Name = group.PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName;
                                ge.Size = (float)(group.Percentage * populationPercentage);
                                if (discordUser != null && discordUser.IsAdmin) ge.Modifier = empire.PopsimGmData.ContainsKey(group.PopsimGlobalEthicGroup) ? empire.PopsimGmData[group.PopsimGlobalEthicGroup].ToDictionary(k => ((Alignment)k.Key).AlignmentName, v => v.Value) : null;
                                GroupEntries.Add(ge);
                            }
                            else
                            {
                                GroupEntries.FirstOrDefault(g => g.Name == group.PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName).Size += (float)(group.Percentage * populationPercentage);
                            }

                        }
                    }


                    List<string> specieList = planet.Pops.ConvertAll(x => x.Species).Distinct().ToList();
                    int totalPops = planet.Pops.Count;

                    foreach (string species in specieList)
                    {
                        SpeciesEntry se = speciesEntries.FirstOrDefault(s => s.Name == species.Replace("\"", ""));
                        if (se == null)
                        {
                            se = new SpeciesEntry();
                            se.Name = species.Replace("\"", "");
                            se.Amount = (float)((planet.Pops.Count(y => y.Species == species) / (float)totalPops) * (float)populationPercentage * 100);
                            speciesEntries.Add(se);
                        }
                        else
                        {
                            var speciespop = (float)((planet.Pops.Count(y => y.Species == species) / (float)totalPops) * populationPercentage * 100);
                            speciesEntries.Where(s => s.Name == species.Replace("\"", "")).Single().Amount += speciespop;
                        }
                    }




                }
            }

            replyRaw.GroupEntries = GroupEntries.ToArray();
            replyRaw.Species = speciesEntries.ToArray();

            Dictionary<string, ulong> EconIndustries = _econ.GetGrossGdp(empire);
            List<IndustryEntry> industryEntries = new List<IndustryEntry>();
            foreach (KeyValuePair<string, ulong> industry in EconIndustries)
            {
                IndustryEntry ie = new IndustryEntry();
                ie.Name = industry.Key;
                ie.GDP = industry.Value;
                if (discordUser != null && discordUser.IsAdmin) ie.Modifier = empire.EconGmData.Keys.Contains(ie.Name) ? empire.EconGmData[ie.Name] : 1;
                industryEntries.Add(ie);
            }
            replyRaw.IndustryEntries = industryEntries.ToArray();

            replyRaw.SpaceIndustryEntries = new IndustryEntry[empire.NationalOutput.Count];
            for (int x = 0; x < empire.NationalOutput.Count; x++)
            {
                IndustryEntry ie = new IndustryEntry();
                ie.Name = empire.NationalOutput.Keys.ToList()[x];
                ie.GDP = empire.NationalOutput[ie.Name];
                if (discordUser != null && discordUser.IsAdmin) ie.Modifier = empire.EconGmData.Keys.Contains(ie.Name) ? empire.EconGmData[ie.Name] : 1;
                replyRaw.SpaceIndustryEntries[x] = ie;
            }

            replyRaw.TotalGdp = (ulong)EconIndustries.Sum(e => (decimal)e.Value);

            Dictionary<Alignment, float> popularities = _econ.CalculateNationalPopularity(empire);
            List<PopularityEntry> popularityEntries = new List<PopularityEntry>();

            foreach (Alignment k in popularities.Keys)
            {
                PopularityEntry pe = new PopularityEntry()
                {
                    Name = k.AlignmentName,
                    Popularity = popularities[k]
                };
                popularityEntries.Add(pe);
            }

            replyRaw.PopularityEntries = popularityEntries.ToArray();

            // await _econ.CalculateNationalAssembly(empire);
            List<PopularityEntry> parlamentEntries = new List<PopularityEntry>();

            foreach (Alignment k in empire.GeneralAssembly.Keys)
            {
                PopularityEntry pe = new PopularityEntry()
                {
                    Name = k.AlignmentName,
                    Popularity = empire.GeneralAssembly[k]
                };
                parlamentEntries.Add(pe);
            }
            replyRaw.ParlamentEntries = parlamentEntries.ToArray();

            return Ok(replyRaw);
        }

        [HttpPost("edit-empire")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult EditEmpire([FromBody] EmpireReturn data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            Empire p = _context.Empires.Where(x => x.Name == "\"" + data.Name + "\"").SingleOrDefault();
            if (p == null)
            {
                return BadRequest();
            }

            for (int x = 0; x < data.GroupEntries.Count(); x++)
            {
                PopsimGlobalEthicGroup group = _context.PopsimGlobalEthicGroups.FirstOrDefault(g => g.PopsimGlobalEthicGroupName == data.GroupEntries[x].Name);
                if (p.PopsimGmData.ContainsKey(group))
                {
                    p.PopsimGmData[group] = data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value);
                }
                else
                {
                    p.PopsimGmData.Add(group, data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value));
                }
            }

            for (int x = 0; x < data.IndustryEntries.Count(); x++)
            {
                if (p.EconGmData == null) p.EconGmData = new Dictionary<string, float>();
                if (p.EconGmData.Keys.Contains(data.IndustryEntries[x].Name))
                {
                    p.EconGmData[data.IndustryEntries[x].Name] = (float)data.IndustryEntries[x].Modifier;
                }
                else
                {
                    p.EconGmData.Add(data.IndustryEntries[x].Name, (float)data.IndustryEntries[x].Modifier);
                }
            }

            _context.Empires.Update(p);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("get-planet")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult GetPlanet(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);
            Planet planet = _context.Planets.Where(x => x.PlanetName == "\"" + name + "\"" && !x.PlanetClass.Contains("star")).SingleOrDefault();
            if (planet == null)
            {
                return NotFound();
            }
            PlanetReturn replyRaw = new PlanetReturn();
            replyRaw.Name = planet.PlanetName.Replace("\"", "");
            replyRaw.Population = planet.Population;
            replyRaw.OfficeAlignments = new string[3];
            replyRaw.OfficeAlignments[0] = planet.ExecutiveAlignment == null ? _context.Alignments.First().AlignmentName : planet.ExecutiveAlignment.AlignmentName;
            replyRaw.OfficeAlignments[1] = planet.LegislativeAlignment == null ? _context.Alignments.First().AlignmentName : planet.LegislativeAlignment.AlignmentName;
            replyRaw.OfficeAlignments[2] = planet.PartyAlignment == null ? _context.Alignments.First().AlignmentName : planet.PartyAlignment.AlignmentName;

            replyRaw.GroupEntries = new GroupEntry[planet.PlanetGroups.Count];
            for (int x = 0; x < planet.PlanetGroups.Count; x++)
            {
                GroupEntry ge = new GroupEntry();
                ge.Name = planet.PlanetGroups[x].PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName;
                ge.Size = planet.PlanetGroups[x].Percentage;
                if (discordUser != null && discordUser.IsAdmin) ge.Modifier = planet.PopsimGmData.Keys.Contains(planet.PlanetGroups[x]) ? planet.PopsimGmData[planet.PlanetGroups[x]].ToDictionary(k => ((Alignment)k.Key).AlignmentName, v => v.Value) : new Dictionary<string, float>();
                replyRaw.GroupEntries[x] = ge;
            }

            replyRaw.IndustryEntries = new IndustryEntry[planet.Output.Count];
            for (int x = 0; x < planet.Output.Count; x++)
            {
                IndustryEntry ie = new IndustryEntry();
                ie.Name = planet.Output.Keys.ToList()[x];
                ie.GDP = planet.Output[ie.Name];
                if (discordUser != null && discordUser.IsAdmin) ie.Modifier = planet.EconGmData.Keys.Contains(ie.Name) ? planet.EconGmData[ie.Name] : 1;
                replyRaw.IndustryEntries[x] = ie;
            }

            int totalPops = planet.Pops.Count;
            List<string> specieList = planet.Pops.ConvertAll(x => x.Species).Distinct().ToList();
            replyRaw.Species = new SpeciesEntry[specieList.Count];
            for (int x = 0; x < specieList.Count; x++)
            {
                SpeciesEntry se = new SpeciesEntry();
                se.Name = specieList[x].Replace("\"", "");
                se.Amount = (float)Math.Round((planet.Pops.Count(y => y.Species == specieList[x]) / (float)totalPops) * 1000) / 10;
                replyRaw.Species[x] = se;
            }

            Dictionary<Alignment, float> popularities = _econ.CalculatePlanetPopularity(planet);
            List<PopularityEntry> popularityEntries = new List<PopularityEntry>();

            foreach (Alignment k in popularities.Keys)
            {
                PopularityEntry pe = new PopularityEntry()
                {
                    Name = k.AlignmentName,
                    Popularity = popularities[k]
                };
                popularityEntries.Add(pe);
            }

            replyRaw.PopularityEntries = popularityEntries.ToArray();

            List<AlignmentMod> alignmentMods = new List<AlignmentMod>();
            if (planet.GlobalAlignment == null) planet.GlobalAlignment = new Dictionary<Alignment, float>();
            foreach (Alignment k in planet.GlobalAlignment.Keys)
            {
                AlignmentMod am = new AlignmentMod()
                {
                    Name = k.AlignmentName,
                    Mod = planet.GlobalAlignment[k]
                };
                alignmentMods.Add(am);
            }

            replyRaw.AlignmentModifiers = alignmentMods.ToArray();

            return Ok(replyRaw);
        }

        [HttpPost("edit-planet")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult EditPlanet([FromBody] PlanetReturn data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            Planet p = _context.Planets.Where(x => x.PlanetName == "\"" + data.Name + "\"" && !x.PlanetClass.Contains("star")).SingleOrDefault();
            if (p == null)
            {
                return BadRequest();
            }

            Alignment tempAlign = _context.Alignments.FirstOrDefault(x => x.AlignmentName == data.OfficeAlignments[0]);
            p.ExecutiveAlignment = tempAlign == null ? _context.Alignments.First() : tempAlign;

            tempAlign = _context.Alignments.FirstOrDefault(x => x.AlignmentName == data.OfficeAlignments[1]);
            p.LegislativeAlignment = tempAlign == null ? _context.Alignments.First() : tempAlign;

            tempAlign = _context.Alignments.FirstOrDefault(x => x.AlignmentName == data.OfficeAlignments[2]);
            p.PartyAlignment = tempAlign == null ? _context.Alignments.First() : tempAlign;

            for (int x = 0; x < data.GroupEntries.Count(); x++)
            {
                if (x < p.PlanetGroups.Count)
                {
                    PopsimGlobalEthicGroup global = _context.PopsimGlobalEthicGroups.First(pg => pg.PopsimGlobalEthicGroupName == data.GroupEntries[x].Name);
                    if (!p.Owner.PopsimGmData.Keys.Contains(global))
                    {
                        p.Owner.PopsimGmData.Add(global, _context.Alignments.ToList().ToDictionary(x => x, x => 1f));
                    }
                    else if (p.Owner.PopsimGmData[global].Count() < _context.Alignments.Count())
                    {
                        p.Owner.PopsimGmData[global] = _context.Alignments.ToList().ToDictionary(x => x, x => p.Owner.PopsimGmData[global].Keys.Contains(x) ? p.Owner.PopsimGmData[global][x] : 1f);
                    }
                    p.PlanetGroups[x].PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName = data.GroupEntries[x].Name;
                    p.PlanetGroups[x].Percentage = data.GroupEntries[x].Size;
                    p.PopsimGmData[p.PlanetGroups[x]] = data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value);
                }
                else
                {
                    if (_context.PopsimGlobalEthicGroups.Any(pg => pg.PopsimGlobalEthicGroupName == data.GroupEntries[x].Name))
                    {
                        PopsimGlobalEthicGroup global = _context.PopsimGlobalEthicGroups.First(pg => pg.PopsimGlobalEthicGroupName == data.GroupEntries[x].Name);
                        PopsimPlanetEthicGroup g = new PopsimPlanetEthicGroup();
                        g.Percentage = data.GroupEntries[x].Size;
                        global.PlanetaryEthicGroups.Add(g);
                        _context.PopsimGlobalEthicGroups.Update(global);
                        if (!p.Owner.PopsimGmData.Keys.Contains(global))
                        {
                            Console.WriteLine("???");
                            p.Owner.PopsimGmData.Add(global, _context.Alignments.ToList().ToDictionary(x => x, x => 1f));
                        }
                        else if (p.Owner.PopsimGmData[global].Count() < _context.Alignments.Count())
                        {
                            p.Owner.PopsimGmData[global] = _context.Alignments.ToList().ToDictionary(x => x, x => p.Owner.PopsimGmData[global].Keys.Contains(x) ? p.Owner.PopsimGmData[global][x] : 1f);
                        }
                        p.PlanetGroups.Add(g);
                        p.PopsimGmData.Add(g, data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value));
                    }
                    else
                    {
                        PopsimGlobalEthicGroup global = new PopsimGlobalEthicGroup();
                        global.PopsimGlobalEthicGroupName = data.GroupEntries[x].Name;
                        global.Radicalisation = 1;
                        global.PartyInvolvementFactor = 1;
                        global.CooperationCompetition = 5;
                        global.DemocracyAuthority = 5;
                        global.FederalismCentralism = 5;
                        global.GlobalismIsolationism = 5;
                        global.MilitarismPacifism = 5;
                        global.MonoculturalismMulticulturalism = 5;
                        global.SecurityFreedom = 5;
                        global.SecularismSpiritualism = 5;
                        global.ProgressivismTraditionalism = 5;
                        PopsimPlanetEthicGroup g = new PopsimPlanetEthicGroup();
                        g.Percentage = data.GroupEntries[x].Size;
                        global.PlanetaryEthicGroups = new List<PopsimPlanetEthicGroup>();
                        global.PlanetaryEthicGroups.Add(g);
                        _context.PopsimGlobalEthicGroups.Add(global);
                        p.Owner.PopsimGmData.Add(global, _context.Alignments.ToList().ToDictionary(x => x, x => 1f));
                        p.PlanetGroups.Add(g);
                        p.PopsimGmData.Add(g, data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value));
                    }
                }
            }

            if (data.GroupEntries.Count() < p.PlanetGroups.Count)
            {
                for (int x = data.GroupEntries.Count(); x < p.PlanetGroups.Count; x++)
                {
                    PopsimGlobalEthicGroup globe = p.PlanetGroups[x].PopsimGlobalEthicGroup;
                    globe.PlanetaryEthicGroups.RemoveAt(globe.PlanetaryEthicGroups.IndexOf(p.PlanetGroups[x]));
                    p.PopsimGmData.Remove(p.PlanetGroups[x]);
                    p.PlanetGroups.RemoveAt(x);
                }
            }

            for (int x = 0; x < data.IndustryEntries.Count(); x++)
            {
                if (p.EconGmData == null) p.EconGmData = new Dictionary<string, float>();
                if (p.Output.Keys.Contains(data.IndustryEntries[x].Name))
                {
                    p.Output[data.IndustryEntries[x].Name] = data.IndustryEntries[x].GDP;
                    p.EconGmData[data.IndustryEntries[x].Name] = (float)data.IndustryEntries[x].Modifier;
                }
                else
                {
                    p.Output.Add(data.IndustryEntries[x].Name, data.IndustryEntries[x].GDP);
                    p.EconGmData.Add(data.IndustryEntries[x].Name, (float)data.IndustryEntries[x].Modifier);
                }
            }

            for (int x = 0; x < data.AlignmentModifiers.Count(); x++)
            {
                if (p.GlobalAlignment == null) p.GlobalAlignment = new Dictionary<Alignment, float>();
                Alignment a = p.GlobalAlignment.Keys.FirstOrDefault(y => y.AlignmentName == data.AlignmentModifiers[x].Name);
                if (a != null)
                {
                    p.GlobalAlignment[a] = data.AlignmentModifiers[x].Mod;
                }
                else
                {
                    a = _context.Alignments.FirstOrDefault(y => y.AlignmentName == data.AlignmentModifiers[x].Name);
                    if (a != null)
                    {
                        p.GlobalAlignment.Add(a, data.AlignmentModifiers[x].Mod);
                    }
                }
            }

            _context.Planets.Update(p);
            _context.Empires.Update(p.Owner);
            _context.SaveChanges();

            return Ok();
        }
    }
}
