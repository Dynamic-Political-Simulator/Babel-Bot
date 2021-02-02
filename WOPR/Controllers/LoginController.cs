using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WOPR.Services;

namespace WOPR.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly DiscordBotService _discordBotService;

		public LoginController(DiscordBotService discordBotService)
		{
			_discordBotService = discordBotService;
		}

		[HttpPost("send-auth")]
		public IActionResult SendAuthenticationCode(string discordUserId)
		{
			var result = _discordBotService.SendAuthCodeToDiscordUser(discordUserId);

			if(result == null)
			{
				return Ok();
			}

			return StatusCode(500);
		}

		[HttpPost("verify-auth")]
		public IActionResult VerifyAuth(string discordUserId, string authCode)
		{
			var result = DiscordUserAuthenticationTokenService.VerifyLogin(discordUserId, authCode);
		}
	}
}
