using BabelDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CharacterCreationController : ControllerBase
	{
		private readonly BabelContext _context;

		public CharacterCreationController(BabelContext context)
		{
			_context = context;
		}

		[HttpGet("species")]
		public IEnumerable<Species> GetSpecies()
		{
			var allSpecies = _context.Species.ToList();

			return allSpecies;
		}

		[HttpPost("create")]
		public IActionResult CreateCharacter([FromBody]CharacterCreationForm form)
		{
			var hasActiveCharacter = _context.Characters.AsQueryable().Where(c => c.DiscordUserId == form.DiscordUserId && c.YearOfDeath != 0).ToList();

			if(hasActiveCharacter.Count > 0)
			{
				return BadRequest();
			}

			var currentYear = _context.Year.FirstOrDefault();

			var rand = new Random();

			var age = rand.Next(18, 26);

			var yearOfBirth = currentYear.CurrentYear - age;

			var newCharacter = new Character()
			{
				CharacterName = form.CharacterName,
				DiscordUserId = form.DiscordUserId,
				SpeciesId = form.SpeciesId,
				YearOfBirth = yearOfBirth
			};

			_context.Characters.Add(newCharacter);
			_context.SaveChanges();

			return Ok();
		}
	}

	public struct CharacterCreationForm
	{
		public string DiscordUserId { get; set; }
		public string CharacterName { get; set; }
		public string SpeciesId { get; set; }
		public int StaringAge { get; set; }
	}

}
