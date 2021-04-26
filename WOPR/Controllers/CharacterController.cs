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

namespace WOPR.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CharacterController : ControllerBase
	{
		private readonly BabelContext _context;

		public CharacterController(BabelContext context)
		{
			_context = context;
		}

		[HttpGet("get-species")]
		public List<string> GetSpecies()
		{
			var allSpecies = _context.Species.ToList().OrderBy(s => s.SpeciesId).Select(s => s.SpeciesName);

			return allSpecies.ToList();
		}

		[HttpGet("get-character")]
		public IActionResult GetCharacter(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var character = _context.Characters.SingleOrDefault(c => c.CharacterId == id);

			if (character == null)
			{
				return NotFound();
			}

			if(character.DiscordUserId != userId)
			{
				return StatusCode(401);
			}

			return Ok(character);
		}

		[HttpGet("my-characters")]
		public List<Character> GetMyCharacters()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var characters = _context.Characters
				.Include(c => c.Species)
				.Where(c => c.DiscordUserId == userId);

			return characters.ToList();
		}

		public struct CharacterCreationForm
		{
			public string CharacterName { get; set; }
			public string Species { get; set; }
			public string CharacterBio { get; set; }
		}

		[HttpPost("create-character")]
		public IActionResult CreateCharacter([FromBody]CharacterCreationForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var hasActiveCharacter = _context.Characters.AsQueryable().Where(c => c.DiscordUserId == userId && c.YearOfDeath != 0).ToList();

			if(hasActiveCharacter.Count > 0)
			{
				return BadRequest();
			}

			//var currentYear = _context.Year.FirstOrDefault();
			var currentYear = 2400;

			var rand = new Random();

			var age = rand.Next(18, 26);

			var yearOfBirth = currentYear - age;

			var species = _context.Species.SingleOrDefault(s => s.SpeciesName == form.Species);

			var newCharacter = new Character()
			{
				CharacterName = form.CharacterName,
				DiscordUserId = userId,
				SpeciesId = species.SpeciesId,
				CharacterBio = form.CharacterBio,
				YearOfBirth = yearOfBirth
			};

			_context.Characters.Add(newCharacter);
			_context.SaveChanges();

			return Ok();
		}
	}

	

}
