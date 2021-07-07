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
        public class PlanetReturn
        {
            public string Name;
            public ulong Population;
            public SpeciesEntry[] Species;
            public string[] OfficeAlignments;
            public GroupEntry[] GroupEntries;
            public IndustryEntry[] IndustryEntries;
            public PopularityEntry[] PopularityEntries;
        }

        public class EmpireReturn
        {
            public string Name;
            public ulong Population;
            public SpeciesEntry[] Species;
            public GroupEntry[] GroupEntries;
            public IndustryEntry[] IndustryEntries;
        }

        [HttpGet("get-empire")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult GetEmpire(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            Empire empire = _context.Empires.Where(e => e.Name == "\"" + name + "\"").SingleOrDefault();
            if (empire == null)
            {
                return NotFound();
            }

            EmpireReturn replyRaw = new EmpireReturn();

            replyRaw.Name = empire.Name.Replace("\"", "");
            ulong population = 0;
            List<GroupEntry> GroupEntries = new List<GroupEntry>(); 
            foreach(GalacticObject system in empire.GalacticObjects)
            {
                foreach (Planet planet in system.Planets.Where(p => p.Population != 0))
                {
                    population += planet.Population;
                    
                }
            }

            foreach (GalacticObject system in empire.GalacticObjects)
            {
                foreach (Planet planet in system.Planets.Where(p => p.Population != 0 && p.PlanetGroups.Count != 0))
                {
                    float populationPercentage = (float)(planet.Population / population);
                    foreach(PopsimPlanetEthicGroup group in planet.PlanetGroups)
                    {

                        GroupEntry ge = GroupEntries.FirstOrDefault(g => g.Name == group.PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName);
                        if(ge == null)
                        {
                            ge = new GroupEntry();
                            ge.Name = group.PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName;
                            ge.Size = group.Percentage * populationPercentage;
                        }
                        
                        
                    }

                }
            }

            replyRaw.Population = population;

            
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
                if (discordUser != null && discordUser.IsAdmin) ie.Modifier = planet.EconGmData.Keys.Contains(ie.Name) ? planet.EconGmData[ie.Name] : 0;
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
                    p.PlanetGroups[x].PopsimGlobalEthicGroup.PopsimGlobalEthicGroupName = data.GroupEntries[x].Name;
                    p.PlanetGroups[x].Percentage = data.GroupEntries[x].Size;
                    p.PopsimGmData[p.PlanetGroups[x]] = data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value);
                    p.Controller.PopsimGmData[p.PlanetGroups[x]] = data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value);
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
                        p.PlanetGroups.Add(g);
                        p.PopsimGmData.Add(g, data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value));
                        p.Controller.PopsimGmData.Add(g, data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value));
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
                        PopsimPlanetEthicGroup g = new PopsimPlanetEthicGroup();
                        g.Percentage = data.GroupEntries[x].Size;
                        global.PlanetaryEthicGroups = new List<PopsimPlanetEthicGroup>();
                        global.PlanetaryEthicGroups.Add(g);
                        _context.PopsimGlobalEthicGroups.Add(global);
                        p.PlanetGroups.Add(g);
                        p.PopsimGmData.Add(g, data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value));
                        p.Controller.PopsimGmData.Add(g, data.GroupEntries[x].Modifier.ToDictionary(k => _context.Alignments.FirstOrDefault(x => x.AlignmentName == k.Key), v => v.Value));
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

            _context.Planets.Update(p);
            _context.Empires.Update(p.Controller);
            _context.SaveChanges();

            return Ok();
        }
    }
}
