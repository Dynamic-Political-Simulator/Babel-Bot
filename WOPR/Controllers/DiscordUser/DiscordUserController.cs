using BabelDatabase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WOPR.Controllers.DiscordUser
{
	[Route("api/user")]
	[ApiController]
	public class DiscordUserController : ControllerBase
	{
		private readonly BabelContext _context;
		private readonly IConfiguration _config;

		public DiscordUserController(BabelContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}
		
		[HttpGet("is-admin")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult IsAdmin()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(c => c.DiscordUserId == userId);

			return Ok(!(discordUser == null || discordUser.IsAdmin == false));
		}

		[Authorize(AuthenticationSchemes = "Discord")]
		[HttpGet("my-id")]
		public IActionResult MyId()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			return Ok(userId);
		}

		[HttpGet("get-token")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetToken()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			string key = _config.GetValue<string>("Jwt:EncryptionKey");
			string issuer = _config.GetValue<string>("Jwt:Issuer");
			string audience = _config.GetValue<string>("Jwt:Audience");

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var permClaims = new List<Claim>();
			permClaims.Add(new Claim("discordId", userId));

			var token = new JwtSecurityToken(issuer, audience, permClaims, null, DateTime.Now.AddDays(7), credentials);

			var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

			return Ok(jwtToken);
		}
	}
}
