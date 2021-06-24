using BabelDatabase;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPSSimulation.Classes;
using System.IO;
using System.Xml;


namespace WOPR.Services
{
    public class EconPopsimServices
    {
        private readonly BabelContext _context;

        public EconPopsimServices(BabelContext context)
        {
            _context = context;
        }

        public async void CalculateEmpireEcon(BabelDatabase.Empire empire)
        {
            DPSSimulation.Classes.Empire empire1 = CreateEmpire(empire);

            empire1.EmpireEcon();

            empire.NationalOutput = empire1.NationalOutput;

            foreach(DPSSimulation.Classes.GalacticObject system in empire1.GalacticObjects)
            {
                foreach(DPSSimulation.Classes.Planet planet in system.Planets)
                {
                    if (planet.Owner == planet.Controller && planet.Pops.Count != 0)
                    {
                        BabelDatabase.Planet planet1 = _context.Planets.FirstOrDefault(p => p.PlanetId == planet.PlanetGameId);
                        if(planet1 != null)
                        {
                            planet1.Population = planet.Population;
                            planet1.Output = planet.Output;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async void CalculatePlanetEcon(BabelDatabase.Planet planet)
        {
            DPSSimulation.Classes.Planet planet1 = CreatePlanet(planet);

            planet1.CalculateEconomy(planet.Owner.EconGmData);

            planet.Population = planet1.Population;
            planet.Output = planet1.Output;

            await _context.SaveChangesAsync();
        }

        public DPSSimulation.Classes.Empire CreateEmpire (BabelDatabase.Empire empire)
        {
            DPSSimulation.Classes.Empire NewEmpire = new DPSSimulation.Classes.Empire()
            {
                Name = empire.Name,
                NationalOutput = empire.NationalOutput,
                EconGmData = empire.EconGmData
            };

            foreach(BabelDatabase.GalacticObject system in empire.GalacticObjects)
            {
                NewEmpire.GalacticObjects.Add(CreateSystem(system));
            }

            foreach (KeyValuePair<PopsimPlanetEthicGroup, Dictionary<Alignment, float>> popsimGmData in empire.PopsimGmData)
            {
                Dictionary<Faction, float> factionStuff = new Dictionary<Faction, float>();
                foreach (KeyValuePair<Alignment, float> faction in popsimGmData.Value)
                {
                    factionStuff.Add(CreateFaction(faction.Key), faction.Value);
                }
                NewEmpire.PopsimGmData.Add(CreateGroup(popsimGmData.Key.PopsimGlobalEthicGroup), factionStuff);
            }

            return NewEmpire;
        }

        public DPSSimulation.Classes.GalacticObject CreateSystem(BabelDatabase.GalacticObject system)
        {
            DPSSimulation.Classes.GalacticObject NewSystem = new DPSSimulation.Classes.GalacticObject()
            {
                Type = system.Type,
                Name = system.Name
            };

            foreach(BabelDatabase.Planet planet in system.Planets)
            {
                NewSystem.Planets.Add(CreatePlanet(planet));
            }

            return NewSystem;
        }

        public DPSSimulation.Classes.Planet CreatePlanet(BabelDatabase.Planet planet)
        {
            DPSSimulation.Classes.Planet LibraryPlanet = new DPSSimulation.Classes.Planet()
            {
                PlanetGameId = planet.PlanetId,
                Name = planet.PlanetName,
                Population = planet.Population,
                Owner = planet.OwnerId,
                Controller = planet.ControllerId,
                Output = planet.Output,
                EconGmData = planet.EconGmData
            };

            BabelDatabase.Data data = _context.Data.First();
            if (data == null)
            {
                //bad
            }
            else
            {
                LibraryPlanet.Data = CreateData(data);
            }

            foreach(BabelDatabase.Pop pop in planet.Pops)
            {
                LibraryPlanet.Pops.Add(CreatePop(pop));
            }

            foreach(BabelDatabase.District district in planet.Districts)
            {
                LibraryPlanet.Districts.Add(CreateDistrict(district));
            }

            foreach(BabelDatabase.Building building in planet.Buildings)
            {
                LibraryPlanet.Buildings.Add(CreateBuilding(building));
            }

            foreach(BabelDatabase.PopsimPlanetEthicGroup popsimPlanetEthicGroup in planet.PlanetGroups)
            {
                LibraryPlanet.PlanetGroups.Add(CreateGroup(popsimPlanetEthicGroup.PopsimGlobalEthicGroup), popsimPlanetEthicGroup.Percentage);
            }

            foreach(KeyValuePair<PopsimPlanetEthicGroup,Dictionary<Alignment,float>> popsimGmData in planet.PopsimGmData)
            {
                Dictionary<Faction, float> factionStuff = new Dictionary<Faction, float>();
                foreach(KeyValuePair<Alignment,float> faction in popsimGmData.Value)
                {
                    factionStuff.Add(CreateFaction(faction.Key), faction.Value);
                }
                LibraryPlanet.PopsimGmData.Add(CreateGroup(popsimGmData.Key.PopsimGlobalEthicGroup), factionStuff);
            }

            return LibraryPlanet;
        }

        public DPSSimulation.Classes.Pop CreatePop(BabelDatabase.Pop pop)
        {
            DPSSimulation.Classes.Pop NewPop = new DPSSimulation.Classes.Pop()
            {
                PopGameId = pop.PopId,
                Job = pop.Job,
                Strata = pop.Strata,
                Power = pop.Power,
                Hapiness = pop.Happiness
            };

            return NewPop;
        }

        public DPSSimulation.Classes.Building CreateBuilding(BabelDatabase.Building building)
        {
            DPSSimulation.Classes.Building NewBuilding = new DPSSimulation.Classes.Building()
            {
                BuildingGameId = building.BuildingId,
                Type = building.Type,
                ruined = building.Ruined
            };

            return NewBuilding;
        }

        public DPSSimulation.Classes.District CreateDistrict(BabelDatabase.District district)
        {
            DPSSimulation.Classes.District NewDistrict = new DPSSimulation.Classes.District()
            {
                DistrictId = district.DistrictId,
                Type = district.Type
            };

            return NewDistrict;
        }

        public DPSSimulation.Classes.Group CreateGroup(BabelDatabase.PopsimGlobalEthicGroup group)
        {
            Group NewGroup = new Group()
            {
                PartyInvolvementFactor = group.PartyInvolvementFactor,
                Radicalisation = group.Radicalisation
            };

            NewGroup.Alignment = CreateAlignment(group);

            return NewGroup;
        }

        public DPSSimulation.Classes.Faction CreateFaction(BabelDatabase.Alignment faction)
        {
            Faction NewFaction = new Faction()
            {
                Establishment = faction.Establishment
            };

            NewFaction.Alignment = CreateAlignment(faction);

            return NewFaction;
        }
        
        public DPSSimulation.Classes.PoliticalAlignment CreateAlignment(BabelDatabase.PopsimGlobalEthicGroup group)
        {

            DPSSimulation.Classes.PoliticalAlignment politicalAlignment = new PoliticalAlignment();

            politicalAlignment.Alignments.Add("federalism", group.FederalismCentralism);
            politicalAlignment.Alignments.Add("centralism", 10 - group.FederalismCentralism);
            politicalAlignment.Alignments.Add("democracy", group.DemocracyAuthority);
            politicalAlignment.Alignments.Add("authority", 10 - group.DemocracyAuthority);
            politicalAlignment.Alignments.Add("globalism", group.GlobalismIsolationism);
            politicalAlignment.Alignments.Add("isolationism", 10 - group.GlobalismIsolationism);
            politicalAlignment.Alignments.Add("militarism", group.MilitarismPacifism);
            politicalAlignment.Alignments.Add("pacifism", 10 - group.MilitarismPacifism);
            politicalAlignment.Alignments.Add("security", group.SecurityFreedom);
            politicalAlignment.Alignments.Add("freedom", 10 - group.SecurityFreedom);
            politicalAlignment.Alignments.Add("cooperation", group.CooperationCompetition);
            politicalAlignment.Alignments.Add("competition", 10 - group.CooperationCompetition);
            politicalAlignment.Alignments.Add("secularism", group.SecularismSpiritualism);
            politicalAlignment.Alignments.Add("spiritualism", 10 - group.SecularismSpiritualism);
            politicalAlignment.Alignments.Add("progressivism", group.ProgressivismTraditionalism);
            politicalAlignment.Alignments.Add("traditionalism", 10 - group.ProgressivismTraditionalism);
            politicalAlignment.Alignments.Add("monoculturalism", group.MonoculturalismMulticulturalism);
            politicalAlignment.Alignments.Add("multiculturalism", 10 - group.MonoculturalismMulticulturalism);

            return politicalAlignment;
        }

        public DPSSimulation.Classes.PoliticalAlignment CreateAlignment(BabelDatabase.Alignment group)
        {

            DPSSimulation.Classes.PoliticalAlignment politicalAlignment = new PoliticalAlignment();

            politicalAlignment.Alignments.Add("federalism", group.FederalismCentralism);
            politicalAlignment.Alignments.Add("centralism", 10 - group.FederalismCentralism);
            politicalAlignment.Alignments.Add("democracy", group.DemocracyAuthority);
            politicalAlignment.Alignments.Add("authority", 10 - group.DemocracyAuthority);
            politicalAlignment.Alignments.Add("globalism", group.GlobalismIsolationism);
            politicalAlignment.Alignments.Add("isolationism", 10 - group.GlobalismIsolationism);
            politicalAlignment.Alignments.Add("militarism", group.MilitarismPacifism);
            politicalAlignment.Alignments.Add("pacifism", 10 - group.MilitarismPacifism);
            politicalAlignment.Alignments.Add("security", group.SecurityFreedom);
            politicalAlignment.Alignments.Add("freedom", 10 - group.SecurityFreedom);
            politicalAlignment.Alignments.Add("cooperation", group.CooperationCompetition);
            politicalAlignment.Alignments.Add("competition", 10 - group.CooperationCompetition);
            politicalAlignment.Alignments.Add("secularism", group.SecularismSpiritualism);
            politicalAlignment.Alignments.Add("spiritualism", 10 - group.SecularismSpiritualism);
            politicalAlignment.Alignments.Add("progressivism", group.ProgressivismTraditionalism);
            politicalAlignment.Alignments.Add("traditionalism", 10 - group.ProgressivismTraditionalism);
            politicalAlignment.Alignments.Add("monoculturalism", group.MonoculturalismMulticulturalism);
            politicalAlignment.Alignments.Add("multiculturalism", 10 - group.MonoculturalismMulticulturalism);

            return politicalAlignment;
        }

        public DPSSimulation.Classes.Data CreateData(BabelDatabase.Data data)
        {
            DPSSimulation.Classes.Data NewData = new DPSSimulation.Classes.Data()
            {
                BaseGdpPerPop = data.BaseGdpPerPop,
                Stratas = data.Stratas
            };

            return NewData;
        }

        public DPSSimulation.Classes.InfraStructureData CreateInfrastructureData(BabelDatabase.InfrastructureData infrastructureData)
        {
            DPSSimulation.Classes.InfraStructureData NewData = new InfraStructureData()
            {
                GdpPerInfrastructure = infrastructureData.GdpPerInfrastructure,
                Infrastructures = infrastructureData.Infrastructures
            };

            return NewData;
        }
    }
}
