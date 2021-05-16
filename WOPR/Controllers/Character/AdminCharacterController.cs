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
using WOPR.Helpers;

namespace WOPR.Controllers.Character
{
	[Route("api/admin-character")]
	[ApiController]
	public class AdminCharacterController : ControllerBase
	{
		private readonly BabelContext _context;

		public AdminCharacterController(BabelContext context)
		{
			_context = context;
		}

		[HttpPost("delete-character")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public async Task<IActionResult> DeleteCharacter(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null || discordUser.IsAdmin == false)
			{
				return Unauthorized();
			}

			var character = _context.Characters.SingleOrDefault(c => c.CharacterId == id);

			try
			{
				_context.Characters.Remove(character);
				await _context.SaveChangesAsync();
			}
			// Ideally this catch should be split up into "could not find entry" and "actual error"
			catch (Exception e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			
			return Ok();
		}

		public struct AdminCharacterEditForm
		{
			public string CharacterId { get; set; }
			public string CharacterBio { get; set; }

			public string CharacterName { get; set; }
			public int YearOfBirth { get; set; }
			public int YearOfDeath { get; set; }
			public string CauseOfDeath { get; set; }

			public string Species { get; set; }
		}
		
		[HttpPost("edit-character")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public async Task<IActionResult> EditCharacter([FromBody] AdminCharacterEditForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null || discordUser.IsAdmin == false)
			{
				return Unauthorized();
			}

			var character = _context.Characters.SingleOrDefault(c => c.CharacterId == form.CharacterId);

			if (character == null)
			{
				return NotFound();
			}

			if (character.Species.SpeciesName != form.Species)
			{
				var newSpecies = _context.Species.SingleOrDefault(s => s.SpeciesName == form.Species);
				character.Species = newSpecies;
			}

			character.CharacterBio = form.CharacterBio;
			character.CharacterName = form.CharacterName;
			character.YearOfBirth = form.YearOfBirth;
			character.YearOfDeath = form.YearOfDeath;
			if (form.CauseOfDeath == "")
			{
				character.CauseOfDeath = null;
			}
			else
			{
				character.CauseOfDeath = form.CauseOfDeath;
			}
			
			_context.Characters.Update(character);
			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("get-character")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetCharacter(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null	|| discordUser.IsAdmin == false)
			{
				return Unauthorized();
			}

			var character = _context.Characters.SingleOrDefault(c => c.CharacterId == id);

			if (character == null)
			{
				return NotFound();
			}

			var returnData = new CharacterController.CharacterDetailReturn()
			{
				CharacterId = character.CharacterId,
				CharacterName = character.CharacterName,
				CharacterBio = character.CharacterBio,
				YearOfBirth = character.YearOfBirth,
				YearOfDeath = character.YearOfDeath,
				CauseOfDeath = character.CauseOfDeath,
				Species = character.Species.SpeciesName
			};

			if (character.DiscordUserId == userId)
			{
				returnData.UserOwnsCharacter = true;
			}
			else
			{
				returnData.UserOwnsCharacter = false;
			}

			return Ok(returnData);
		}
	}
}
