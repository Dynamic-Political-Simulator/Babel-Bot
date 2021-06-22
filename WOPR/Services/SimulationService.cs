using BabelDatabase;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DPSSimulation.Classes;

namespace WOPR.Services
{
	public class SimulationService
	{
		private readonly BabelContext _context;

		public SimulationService(BabelContext context)
		{
			_context = context;
		}

		public async void GetDataFromSave(string path)
        {
			Map map = new Map(path);

			foreach(DPSSimulation.Classes.Empire empire in map.Empires)
            {
				//set Empires here
				foreach (DPSSimulation.Classes.GalacticObject system in empire.GalacticObjects)
                {
					//system here
					foreach(DPSSimulation.Classes.Planet planet in system.Planets)
                    {
						//set planet here
                    }
                }
			}
        }
	}
}
