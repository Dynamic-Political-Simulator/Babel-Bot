using BabelDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
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

        public MapController(BabelContext context)
        {
            _context = context;
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
                info[x].Name = systems[x].Planet.PlanetName;
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

            Planet planet = _context.Planets.FirstOrDefault(x => x.PlanetName == form.PlanetName);

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

        [HttpPost("create-planet")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult CreatePlanet([FromBody] PlanetCreateForm form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            Planet planet = new Planet();
            planet.PlanetName = form.PlanetName;
            planet.PlanetDescription = form.PlanetDescription;
            _context.Planets.Add(planet);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("get-planet")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult GetPlanet([FromBody] string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {

                return Unauthorized();
            }
            else
            {

            }

            return Ok();
        }
    }
}
