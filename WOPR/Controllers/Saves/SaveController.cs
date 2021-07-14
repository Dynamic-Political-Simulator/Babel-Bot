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
using System.IO.Compression;
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
        public IActionResult UploadSaveFile([FromBody] SaveForm form)
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
                string name = DateTime.Now.ToString("u");
                string zipLoc = "./saves/" + name + ".sav";
                using (var fs = new FileStream(zipLoc, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(saveFileReal, 0, saveFileReal.Length);
                    System.IO.Directory.CreateDirectory("saves/" + name);
                    ZipFile.ExtractToDirectory(zipLoc, "./saves/" + name + "/");
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong while trying to write save to a file on disk.\n" + ex);
                return StatusCode(500); // Something went wrong while trying to write to a file on disk.
            }
        }

        class SaveEntry
        {
            public string name;
            public string ingameDate;
        }

        string GetLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                for (int i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }

        [HttpGet("list-saves")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult ListSaveFiles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (userId == null || discordUser == null)
            {
                // Console.WriteLine("user null");
                return Unauthorized();
            }

            if (!discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            string[] dirs = System.IO.Directory.GetDirectories("./saves/");
            List<SaveEntry> entires = new List<SaveEntry>();

            foreach (string dir in dirs)
            {
                if (!System.IO.File.Exists(dir + "/meta")) continue;
                string dateRaw = GetLine(dir + "/meta", 4);
                string[] split = dateRaw.Split("=");
                if (split.Length < 2) continue;
                string date = split[1].Replace("\"", "");
                SaveEntry e = new SaveEntry()
                {
                    name = dir,
                    ingameDate = date
                };
                entires.Add(e);
            }

            return Ok(entires);
        }

        [HttpGet("parse-save")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> ParseSaveFile(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (userId == null || discordUser == null)
            {
                // Console.WriteLine("user null");
                return Unauthorized();
            }

            if (!discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            if (!System.IO.Directory.Exists(name) || !System.IO.File.Exists(name + "/gamestate")) return BadRequest();

            await _simulation.GetDataFromSave(name, null, null);

            return Ok();
        }

        [HttpPost("upload-pop")]
        [Authorize(AuthenticationSchemes = "Discord")]
        [RequestSizeLimit(10000000)]
        public IActionResult UploadPopFile([FromBody] SaveForm form)
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

            try
            {
                using (var fs = new FileStream("./PopDistribution.xml", FileMode.OpenOrCreate, FileAccess.Write))
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

        [HttpPost("upload-empire")]
        [Authorize(AuthenticationSchemes = "Discord")]
        [RequestSizeLimit(10000000)]
        public IActionResult UploadEmpireFile([FromBody] SaveForm form)
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

            try
            {
                using (var fs = new FileStream("./EmpireDistribution.xml", FileMode.OpenOrCreate, FileAccess.Write))
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

        [HttpPost("parse-data")]
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

        [HttpGet("calculate")]
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
