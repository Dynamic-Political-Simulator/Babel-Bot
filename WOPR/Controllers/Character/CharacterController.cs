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

		public class CharacterDetailReturn
		{
			public string CharacterId { get; set; }
			public string CharacterName { get; set; }

			public string CharacterBio { get; set; }

			public int YearOfBirth { get; set; }
			public int YearOfDeath { get; set; }
			public string CauseOfDeath { get; set; }
			
			public string Species { get; set; }

			public bool UserOwnsCharacter { get; set; }
		}

		[HttpGet("get-character")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetCharacter(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var character = _context.Characters.SingleOrDefault(c => c.CharacterId == id);

			if (character == null)
			{
				return NotFound();
			}

			var returnData = new CharacterDetailReturn()
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

		public struct SimpleCharacterSearchReturn
		{
			public string CharacterId { get; set; }
			public string CharacterName { get; set; }
		}

		[HttpGet("search-for-clique")]
		public IActionResult SearchCharactersClique(string search, string id = null)
		{
			var results = _context.Characters.Where(c => EF.Functions.Like(c.CharacterName, search)).ToList();

			var returnList = new List<SimpleCharacterSearchReturn>();

			if (id != null)
			{
				var clique = _context.Cliques.SingleOrDefault(c => c.CliqueId == id);

				var members = clique.CliqueMemberCharacter.Select(cm => cm.MemberId);
				var officers = clique.CliqueOfficerCharacter.Select(co => co.OfficerId);

				foreach (var result in results)
				{
					if (!members.Contains(result.CharacterId) && !officers.Contains(result.CharacterId))
					{
						returnList.Add(new SimpleCharacterSearchReturn
						{
							CharacterId = result.CharacterId,
							CharacterName = result.CharacterName
						});
					}
				}
			}
			else
			{
				foreach (var result in results)
				{
					returnList.Add(new SimpleCharacterSearchReturn
					{
						CharacterId = result.CharacterId,
						CharacterName = result.CharacterName
					});
				}
			}

			return Ok(returnList);
		}

		[HttpGet("my-characters")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetMyCharacters()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			Console.WriteLine("USED ID: " + userId);

			var characters = _context.Characters
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
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult CreateCharacter([FromBody]CharacterCreationForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null)
			{
				return Unauthorized();
			}

			var hasActiveCharacter = _context.Characters.AsQueryable().Where(c => c.DiscordUserId == userId && c.YearOfDeath != 0).ToList();

			if(hasActiveCharacter.Count > 0)
			{
				return BadRequest();
			}

			//var currentYear = _context.Year.FirstOrDefault();
			var currentYear = _context.GameState.First().CurrentYear;

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

			discordUser.ActiveCharacterId = newCharacter.CharacterId;

			_context.Update(discordUser);
			_context.Characters.Add(newCharacter);
			_context.SaveChanges();

			return Ok();
		}
	}

	

}
