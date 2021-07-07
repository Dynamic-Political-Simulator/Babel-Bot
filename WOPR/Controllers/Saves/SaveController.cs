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
using System.IO;
using WOPR.Helpers;
using WOPR.Services;

namespace WOPR.Controllers.Saves
{
    [Route("api/saves")]
    [ApiController]
    public class SaveController : ControllerBase
    {
        private readonly BabelContext _context;
        private readonly SimulationService _simulation;
        private readonly EconPopsimServices _econ;

        public SaveController(BabelContext context, SimulationService simulation, EconPopsimServices econ)
        {
            _context = context;
            _simulation = simulation;
            _econ = econ;
        }

        public struct SaveForm
        {
            public string SaveFile { get; set; }
        }

        [HttpPost("upload-save")]
        [Authorize(AuthenticationSchemes = "Discord")]
        [RequestSizeLimit(10000000)]
        public async Task<IActionResult> UploadSaveFile([FromBody] SaveForm form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (userId == null || discordUser == null)
            {
                return Unauthorized();
            }

            if (!discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            byte[] saveFileReal = Convert.FromBase64String(form.SaveFile);

            if (saveFileReal[0] != (byte)0x50 && saveFileReal[1] != (byte)0x4B) // Stellaris saves are .zip, meaning that the first 2 bytes should be 50 4B
            {
                return BadRequest();
            }

            try
            {
                System.IO.Directory.CreateDirectory("saves");
                using (var fs = new FileStream("./saves/" + DateTime.Now.ToString("u") + ".sav", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(saveFileReal, 0, saveFileReal.Length);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong while trying to write save to a file on disk.\n" + ex);
                return StatusCode(500); // Something went wrong while trying to write to a file on disk.
            }
        }

        [HttpGet("parse-save")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> ParseSaveFile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (userId == null || discordUser == null)
            {
                Console.WriteLine("user null");
                return Unauthorized();
            }

            if (!discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            await _simulation.GetDataFromSave("./saves/", null, null);

            return Ok();
        }

        [HttpGet("parse-data")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> ParseDataFiles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (userId == null || discordUser == null)
            {
                return Unauthorized();
            }

            if (!discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            await _simulation.SetData("./PopDistribution.xml", "./EmpireDistribution.xml");

            return Ok();
        }

        [HttpGet("run-test")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> Test()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (userId == null || discordUser == null)
            {
                return Unauthorized();
            }

            if (!discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            await _econ.CalculateEmpireEcon(_context.Empires.FirstOrDefault(x => x.EmpireId == 1));
            Console.WriteLine("Done!");

            return Ok();
        }
    }
}
