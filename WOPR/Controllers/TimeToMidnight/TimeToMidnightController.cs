using BabelDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WOPR.Controllers.TimeToMidnight
{
    [Route("api/clock")]
    [ApiController]
    public class TimeToMidnightController : ControllerBase
    {
        private readonly BabelContext _context;

        public TimeToMidnightController(BabelContext context)
        {
            _context = context;
        }

        [HttpGet("time")]
        public IActionResult GetTimeToMidnight()
        {
            var timeToMidnight = _context.GameState.FirstOrDefault();

            if (timeToMidnight == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(timeToMidnight.SecondsToMidnight);
        }

        private readonly int[] validTimes = new int[] { 60, 60 * 2, 60 * 5, 60 * 10, 60 * 15 }; // Damn, I'd love to make this dynamic.

        public class ClockSetForm
        {
            public int NewTime { get; set; }
        }

        [HttpPost("time")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult SetTimeToMidnight([FromBody] ClockSetForm form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            var timeToMidnight = _context.GameState.FirstOrDefault();

            if (timeToMidnight == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (!validTimes.Contains(form.NewTime))
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            timeToMidnight.SecondsToMidnight = form.NewTime;
            _context.GameState.Update(timeToMidnight);
            _context.SaveChanges();

            return Ok();
        }
    }
}
