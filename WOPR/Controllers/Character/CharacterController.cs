﻿using BabelDatabase;
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
		public IActionResult GetSpecies()
		{
			var allSpecies = _context.Species.ToList().OrderBy(s => s.SpeciesId).Select(s => s.SpeciesName);

			return Ok(allSpecies.ToList());
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

			if (character.DiscordUserId != userId)
			{
				return StatusCode(401);
			}

			return Ok(character);
		}

		public class CharacterSearchForm
		{
			public int Page { get; set; }
			public bool Alive { get; set; }
			public string Search { get; set; }
		}

		[HttpPost("search")]
		public IActionResult SearchCharacters([FromBody] CharacterSearchForm form)
		{
			List<BabelDatabase.Character> results = new List<BabelDatabase.Character>();

			if(form.Search != null)
			{
				if (form.Alive)
				{
					results = _context.Characters
					.Where(c => (c.CharacterName.ToLower().Contains(form.Search.ToLower()) ||
					c.CharacterBio.ToLower().Contains(form.Search.ToLower())) &&
					c.CauseOfDeath == null).ToList();
				}
				else
				{
					results = _context.Characters
					.Where(c => c.CharacterName.ToLower().Contains(form.Search.ToLower()) ||
					c.CharacterBio.ToLower().Contains(form.Search.ToLower())).ToList();
				}
			}
			else
			{
				results = _context.Characters.Where(c => c.IsDead() != form.Alive).ToList();
			}

			var paginatedListings = new PaginatedList<BabelDatabase.Character>(results, results.Count, form.Page, 20);

			return Ok(paginatedListings);
		}

		[HttpGet("my-characters")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetMyCharacters()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			Console.WriteLine("USED ID: " + userId);

			var characters = _context.Characters
				.Include(c => c.Species)
				.Where(c => c.DiscordUserId == userId);

			return Ok(characters.ToList());
		}

		public struct CharacterEditForm
		{
			public string CharacterId { get; set; }
			public string CharacterBio { get; set; }
		}

		[HttpPost("edit-character")]
		public async Task<IActionResult> EditCharacter([FromBody] CharacterEditForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var character = _context.Characters.SingleOrDefault(c => c.CharacterId == form.CharacterId);

			if (character == null)
			{
				return NotFound();
			}

			if (character.DiscordUserId != userId)
			{
				return StatusCode(401);
			}

			character.CharacterBio = form.CharacterBio;

			_context.Characters.Update(character);
			await _context.SaveChangesAsync();

			return Ok();
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

			var newCharacter = new BabelDatabase.Character()
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
