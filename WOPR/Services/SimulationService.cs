using BabelDatabase;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPSSimulation.Classes;
using System.IO;
using System.Xml;
using WOPR.Services;

namespace WOPR.Services
{
    public class SimulationService
    {
        private readonly BabelContext _context;

        public SimulationService(BabelContext context)
        {
            _context = context;
        }

        public async void SetData(string DataPath, string InfrastructureDataPath)
        {
            XmlDocument Xmldata = new XmlDocument();
            Xmldata.Load(DataPath);
            XmlDocument XmlInfradata = new XmlDocument();
            XmlInfradata.Load(InfrastructureDataPath);
            var data = new DPSSimulation.Classes.Data(Xmldata);
            var infradata = new InfraStructureData(XmlInfradata);

            if (_context.Data.ToList().Count() == 0)
            {
                _context.Data.Add(new BabelDatabase.Data()
                {
                    Stratas = data.Stratas,
                    BaseGdpPerPop = data.BaseGdpPerPop
                });
            }
            else
            {
                var Data = _context.Data.First();
                Data.BaseGdpPerPop = data.BaseGdpPerPop;
                Data.Stratas = data.Stratas;
            }

            if (_context.InfrastructureData.ToList().Count() == 0)
            {
                _context.InfrastructureData.Add(new BabelDatabase.InfrastructureData()
                {
                    GdpPerInfrastructure = infradata.GdpPerInfrastructure,
                    Infrastructures = infradata.Infrastructures
                });
            }
            else
            {
                var InfraData = _context.InfrastructureData.First();
                InfraData.GdpPerInfrastructure = infradata.GdpPerInfrastructure;
                InfraData.Infrastructures = infradata.Infrastructures;
            }

            await _context.SaveChangesAsync();

        }

        public async Task GetDataFromSave(string SaveFolderPath, string DataPath, string InfrastructureDataPath)
        {
            var directory = new DirectoryInfo(SaveFolderPath);

            var saveFile = directory.GetFiles()
             .OrderByDescending(f => f.LastWriteTime)
             .First();

            Map map = new Map(saveFile.FullName);

            List<BabelDatabase.Empire> empires = new List<BabelDatabase.Empire>();
            foreach (DPSSimulation.Classes.Empire empire in map.Empires)
            {
                empire.OrganiseFleets();
                empires.Add(GetEmpire(empire));
            }

            await _context.SaveChangesAsync();

            //Another Pass over Fleets to do locations
            foreach (BabelDatabase.Fleet fleet in _context.Fleets)
            {
                BabelDatabase.GalacticObject location = _context.GalacticObjects.FirstOrDefault(g => g.GalacticObjectId == fleet.SystemId);
                if (location != null)
                {
                    fleet.SystemId = location.GalacticObjectId;
                }
            }

            //another pass over planets to do controllers
            foreach (BabelDatabase.Planet planet in _context.Planets)
            {
                BabelDatabase.Empire controller = _context.Empires.FirstOrDefault(e => e.EmpireId == planet.ControllerId);
                if (controller != null)
                {
                    planet.Controller = controller;
                }
            }

            await _context.SaveChangesAsync();
        }

        public BabelDatabase.Empire GetEmpire(DPSSimulation.Classes.Empire empire)
        {
            //set Empires here
            var Empire = _context.Empires.FirstOrDefault(e => e.EmpireId == empire.EmpireID);
            bool newEmpire = false;
            if (Empire == null)
            {
                Empire = new BabelDatabase.Empire()
                {
                    Name = empire.Name,
                    EmpireId = empire.EmpireID
                };
                newEmpire = true;
            }
            else
            {
                Empire.Name = empire.Name;
            }

            List<BabelDatabase.GalacticObject> Systems = new List<BabelDatabase.GalacticObject>();
            foreach (DPSSimulation.Classes.GalacticObject system in empire.GalacticObjects)
            {
                Systems.Add(GetGalacticObject(system, Empire));
            }
            Empire.GalacticObjects = Systems;

            //Military Fleet
            List<BabelDatabase.Fleet> MilFleets = new List<BabelDatabase.Fleet>();
            foreach (DPSSimulation.Classes.Fleet fleet in empire.Fleets)
            {
                MilFleets.Add(GetFleet(fleet, Empire));
            }
            Empire.Fleets = MilFleets;

            //Mining Stations
            List<BabelDatabase.Fleet> MiningFleets = new List<BabelDatabase.Fleet>();
            foreach (DPSSimulation.Classes.Fleet fleet in empire.MiningStations)
            {
                MiningFleets.Add(GetFleet(fleet, Empire));
            }
            Empire.MiningStations = MiningFleets;

            //Research Stations
            List<BabelDatabase.Fleet> ResearchFleets = new List<BabelDatabase.Fleet>();
            foreach (DPSSimulation.Classes.Fleet fleet in empire.ResearchStations)
            {
                ResearchFleets.Add(GetFleet(fleet, Empire));
            }
            Empire.ResearchStations = ResearchFleets;

            if (newEmpire == true)
            {
                _context.Empires.Add(Empire);
            }

            return Empire;
        }

        public BabelDatabase.GalacticObject GetGalacticObject(DPSSimulation.Classes.GalacticObject system, BabelDatabase.Empire empire)
        {
            //system here
            BabelDatabase.GalacticObject GalacticObject = _context.GalacticObjects.FirstOrDefault(g => g.GalacticObjectId == system.GalacticObjectGameId);
            bool newObject = false;
            if (GalacticObject == null)
            {
                GalacticObject = new BabelDatabase.GalacticObject()
                {
                    GalacticObjectId = system.GalacticObjectGameId,
                    PosX = system.PosX,
                    PosY = system.PosY,
                    Type = system.Type,
                    Name = system.Name,
                    Hyperlanes = new Dictionary<int, float>(),
                    Planets = new List<BabelDatabase.Planet>()
                };
                newObject = true;
            }
            else
            {
                GalacticObject.PosX = system.PosX;
                GalacticObject.PosY = system.PosY;
                GalacticObject.Name = system.Name;
                GalacticObject.Type = system.Type;
                GalacticObject.Hyperlanes = new Dictionary<int, float>();
                GalacticObject.Planets = new List<BabelDatabase.Planet>();
            }

            //set planet here
            foreach (DPSSimulation.Classes.Planet planet in system.Planets)
            {
                GalacticObject.Planets.Add(GetPlanet(planet, empire, GalacticObject));
            }

            //Starbase
            BabelDatabase.Starbase starbase = GetStarbase(system.Starbase, empire, GalacticObject);
            GalacticObject.StarbaseId = starbase.StarbaseId;
            GalacticObject.Starbase = starbase;

            //Hyperlane
            foreach (Hyperlane hyperlane in system.Hyperlanes)
            {
                GalacticObject.Hyperlanes.Add(hyperlane.TargetId, hyperlane.Distance);
            }

            if (newObject == true)
            {
                _context.GalacticObjects.Add(GalacticObject);
            }

            return GalacticObject;
        }
        public BabelDatabase.Starbase GetStarbase(DPSSimulation.Classes.Starbase starbase, BabelDatabase.Empire empire, BabelDatabase.GalacticObject galacticObject)
        {
            BabelDatabase.Starbase NewStarbase = _context.Starbases.FirstOrDefault(s => s.StarbaseId == starbase.StarbaseId);
            if (NewStarbase == null)
            { //New Starbase
                NewStarbase = new BabelDatabase.Starbase()
                {
                    StarbaseId = starbase.StarbaseId,
                    Owner = empire.EmpireId,
                    Level = starbase.Level,
                    Modules = starbase.Modules,
                    Buildings = starbase.Buildings
                };

                //starbase fleet
                BabelDatabase.Fleet fleet = GetFleet(starbase.StarbaseFleet, empire);
                NewStarbase.StarbaseFleet = fleet;
                NewStarbase.FleetId = fleet.FleetId;
                _context.Starbases.Add(NewStarbase);
            }
            else
            { //Update Existing Starbase
                NewStarbase.Owner = empire.EmpireId;
                NewStarbase.Level = starbase.Level;
                NewStarbase.Modules = starbase.Modules;
                NewStarbase.Buildings = starbase.Buildings;

                //starbase fleet
                BabelDatabase.Fleet fleet = GetFleet(starbase.StarbaseFleet, empire);
                NewStarbase.StarbaseFleet = fleet;
                NewStarbase.FleetId = fleet.FleetId;
            }
            return NewStarbase;
        }

        public BabelDatabase.Ship GetShip(DPSSimulation.Classes.Ship ship, BabelDatabase.Fleet fleet)
        {
            BabelDatabase.Ship NewShip = _context.Ships.FirstOrDefault(s => s.ShipId == ship.ShipId);
            if (NewShip == null)
            {
                NewShip = new BabelDatabase.Ship()
                {
                    ShipId = ship.ShipId,
                    FleetId = fleet.FleetId,
                    Fleet = fleet,
                    ShipName = ship.ShipName,
                    Type = ship.Type
                };
                _context.Ships.Add(NewShip);
            }
            else
            {
                NewShip.FleetId = fleet.FleetId;
                NewShip.Fleet = fleet;
                NewShip.ShipName = ship.ShipName;
                NewShip.Type = ship.Type;

            }
            return NewShip;
        }

        public BabelDatabase.Fleet GetFleet(DPSSimulation.Classes.Fleet fleet, BabelDatabase.Empire empire)
        {
            BabelDatabase.Fleet NewFleet = _context.Fleets.FirstOrDefault(f => f.FleetId == fleet.FleetId);
            if (NewFleet == null)
            {
                NewFleet = new BabelDatabase.Fleet()
                {
                    FleetId = fleet.FleetId,
                    Name = fleet.Name,
                    OwnerID = empire.EmpireId,
                    Owner = empire,
                    MilitaryPower = fleet.MilitaryPower,
                    SystemId = fleet.System
                };
                //starbase fleet ships
                List<BabelDatabase.Ship> Ships = new List<BabelDatabase.Ship>();
                foreach (DPSSimulation.Classes.Ship ship in fleet.Ships)
                {
                    Ships.Add(GetShip(ship, NewFleet));
                }
                NewFleet.Ships = Ships;
                _context.Fleets.Add(NewFleet);
            }
            else
            {
                NewFleet.Name = fleet.Name;
                NewFleet.OwnerID = empire.EmpireId;
                NewFleet.Owner = empire;
                NewFleet.MilitaryPower = fleet.MilitaryPower;
                NewFleet.SystemId = fleet.System;


                List<BabelDatabase.Ship> Ships = new List<BabelDatabase.Ship>();
                foreach (DPSSimulation.Classes.Ship ship in fleet.Ships)
                {
                    Ships.Add(GetShip(ship, NewFleet));
                }
                NewFleet.Ships = Ships;
            }
            return NewFleet;
        }

        public BabelDatabase.Planet GetPlanet(DPSSimulation.Classes.Planet planet, BabelDatabase.Empire empire, BabelDatabase.GalacticObject galacticObject)
        {
            BabelDatabase.Planet NewPlanet = _context.Planets.FirstOrDefault(p => p.PlanetId == planet.PlanetGameId);
            bool IsPlanetNew = false;

            if (NewPlanet == null)
            {
                NewPlanet = new BabelDatabase.Planet()
                {
                    PlanetId = planet.PlanetGameId,
                    PlanetName = planet.Name,
                    PlanetClass = planet.Planet_class,
                    OwnerId = empire.EmpireId,
                    Owner = empire,
                    //ControllerId = planet.Controller, // TODO: Fix the race(?) condition where the ControllerId FK constraint seems to be inserted before the Empires are.
                    GalacticObjectId = galacticObject.GalacticObjectId,
                    GalacticObject = galacticObject,
                    Pops = new List<BabelDatabase.Pop>(),
                    Buildings = new List<BabelDatabase.Building>(),
                    Districts = new List<BabelDatabase.District>()
                };
                IsPlanetNew = true;
            }
            else
            {
                NewPlanet.PlanetName = planet.Name;
                NewPlanet.PlanetClass = planet.Planet_class;
                NewPlanet.OwnerId = empire.EmpireId;
                NewPlanet.Owner = empire;
                NewPlanet.ControllerId = planet.Controller;
            }

            foreach (DPSSimulation.Classes.Pop pop in planet.Pops)
            {
                NewPlanet.Pops.Add(GetPop(pop, NewPlanet));
            }

            foreach (DPSSimulation.Classes.Building building in planet.Buildings)
            {
                NewPlanet.Buildings.Add(GetBuilding(building, NewPlanet));
            }

            int x = 0;
            foreach (DPSSimulation.Classes.District district in planet.Districts)
            {
                NewPlanet.Districts.Add(GetDistrict(district, NewPlanet, x));
                x++;
            }

            if (IsPlanetNew == true)
            {
                _context.Planets.Add(NewPlanet);
            }

            return NewPlanet;
        }

        public BabelDatabase.Pop GetPop(DPSSimulation.Classes.Pop pop, BabelDatabase.Planet planet)
        {
            BabelDatabase.Pop NewPop = _context.Pops.FirstOrDefault(p => p.PopId == pop.PopGameId);
            if (NewPop == null)
            {
                NewPop = new BabelDatabase.Pop()
                {
                    PopId = pop.PopGameId,
                    PlanetId = planet.PlanetId,
                    Planet = planet,
                    Job = pop.Job,
                    Strata = pop.Strata,
                    Power = pop.Power,
                    Happiness = pop.Hapiness
                };
                _context.Pops.Add(NewPop);
            }
            else
            {
                NewPop.PlanetId = planet.PlanetId;
                NewPop.Planet = planet;
                NewPop.Job = pop.Job;
                NewPop.Strata = pop.Strata;
                NewPop.Power = pop.Power;
                NewPop.Happiness = pop.Hapiness;
            }

            return NewPop;
        }

        public BabelDatabase.District GetDistrict(DPSSimulation.Classes.District district, BabelDatabase.Planet planet, int x)
        {
            // Since DistrictId is an unused field, I replaced it with PlanetId_Pos
            BabelDatabase.District NewDistrict = _context.Districts.FirstOrDefault(d => d.DistrictId == planet.PlanetId.ToString() + x);
            if (NewDistrict == null)
            {
                NewDistrict = new BabelDatabase.District()
                {
                    DistrictId = planet.PlanetId.ToString() + x,
                    Type = district.Type,
                    PlanetId = planet.PlanetId,
                    Planet = planet
                };
                _context.Districts.Add(NewDistrict);
            }
            else
            {
                NewDistrict.Type = district.Type;
                NewDistrict.PlanetId = planet.PlanetId;
                NewDistrict.Planet = planet;
            }

            return NewDistrict;
        }

        public BabelDatabase.Building GetBuilding(DPSSimulation.Classes.Building building, BabelDatabase.Planet planet)
        {
            BabelDatabase.Building NewBuilding = _context.Buildings.FirstOrDefault(b => b.BuildingId == building.BuildingGameId);
            if (NewBuilding == null)
            {
                NewBuilding = new BabelDatabase.Building()
                {
                    BuildingId = building.BuildingGameId,
                    Type = building.Type,
                    Ruined = building.ruined,
                    PlanetId = planet.PlanetId,
                    Planet = planet
                };
                _context.Buildings.Add(NewBuilding);
            }
            else
            {
                NewBuilding.Type = building.Type;
                NewBuilding.Ruined = building.ruined;
                NewBuilding.PlanetId = planet.PlanetId;
                NewBuilding.Planet = planet;
            }

            return NewBuilding;
        }
    }
}
