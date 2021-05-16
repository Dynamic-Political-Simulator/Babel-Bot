using BabelDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
			var timeToMidnight = _context.TimeToMidnight.FirstOrDefault();

			if (timeToMidnight == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return Ok(timeToMidnight.SecondsToMidnight);
		}
	}
}
