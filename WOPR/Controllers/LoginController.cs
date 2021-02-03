using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WOPR.Services;

namespace WOPR.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly DiscordBotService _discordBotService;
		private readonly DiscordUserService _discordUserService;
		private readonly WoprConfig _config;

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

		public class LoginForm
		{
			public string Id { get; set; }
			public string Token { get; set; }
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginForm form)
		{
			var user = _discordUserService.Authenticate(form.Id, form.Token);

			if (user == null)
			{
				return BadRequest("Token is not valid.");
			}

			var jwtTokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_config.JwtSecret);

			var tokenDescriptor = new SecurityTokenDescriptor()
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.DiscordUserId)
				}),
				Expires = DateTime.UtcNow.AddDays(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			var tokenString = jwtTokenHandler.WriteToken(token);

			return Ok(new { AccessToken = tokenString });
		}
	}
}
