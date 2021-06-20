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

namespace WOPR.Controllers.Popsim.Cliques
{
	[Route("api/[controller]")]
	[ApiController]
	public class CliqueController : ControllerBase
	{
		private readonly BabelContext _context;

		public CliqueController(BabelContext context)
		{
			_context = context;
		}

		public struct CliqueOverviewReturn
		{
			public IEnumerable<CliqueListingReturn> Cliques { get; set; }
			public IEnumerable<CliqueListingReturn> CliqueInvites { get; set; }
		}

		public struct CliqueListingReturn
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}

		[HttpGet("get-overview")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetCliqueOverview()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null)
			{
				return Unauthorized();
			}

			var activeCharacter = discordUser.ActiveCharacter;

			if (activeCharacter == null)
			{
				return NotFound();
			}

			var cliques = activeCharacter.Cliques;

			var invites = _context.CliqueInvites.Where(ci => ci.CharacterId == activeCharacter.CharacterId);

			var returnCliques = new List<CliqueListingReturn>();

			foreach (var c in cliques)
			{
				returnCliques.Add(new CliqueListingReturn
				{
					Id = c.CliqueId,
					Name = c.Clique.CliqueName
				});
			}

			var returnInvites = new List<CliqueListingReturn>();

			foreach (var i in invites)
			{
				returnInvites.Add(new CliqueListingReturn
				{
					Id = i.CliqueInviteId,
					Name = i.Clique.CliqueName
				});
			}

			var returnData = new CliqueOverviewReturn
			{
				Cliques = returnCliques,
				CliqueInvites = returnInvites
			};

			return Ok(returnData);
		}

		public struct CliqueReturn
		{
			public string CliqueName { get; set; }
			public IEnumerable<string> Members { get; set; }
			public IEnumerable<string> Officers { get; set; }
			public IEnumerable<string> Alignments { get; set; }
			public ulong Money { get; set; }
			public bool UserIsOfficer { get; set; }
		}

		[HttpGet("get-clique")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetClique(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null)
			{
				return Unauthorized();
			}

			var activeCharacter = discordUser.ActiveCharacter;

			if (activeCharacter == null && !discordUser.IsAdmin)
			{
				return Unauthorized();
			}

			var clique = activeCharacter.Cliques.SingleOrDefault(c => c.CliqueId == id);

			if (clique == null && !discordUser.IsAdmin)
			{
				return Unauthorized();
			}

			var cliqueReturn = new CliqueReturn
			{
				CliqueName = clique.Clique.CliqueName,
				Members = clique.Clique.CliqueMembers.Select(cm => cm.Member.CharacterName),
				Officers = clique.Clique.CliqueOfficers.Select(co => co.Officer.CharacterName),
				Alignments = clique.Clique.Alignments.Select(a => a.Alignment.AlignmentName),
				Money = clique.Clique.Money,
				UserIsOfficer = false
			};

			if (cliqueReturn.Officers.Contains(activeCharacter.CharacterName))
			{
				cliqueReturn.UserIsOfficer = true;
			}

			return Ok(cliqueReturn);
		}

		public struct CreateCliqueForm
		{
			public string CliqueName { get; set; }
		}

		[HttpGet("create-clique")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public async Task<IActionResult> CreateCliqueAsync([FromBody] CreateCliqueForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null || discordUser.ActiveCharacter == null)
			{
				return Unauthorized();
			}

			var newCliqueOfficer = new CliqueOfficerCharacter
			{
				Officer = discordUser.ActiveCharacter
			};

			var newCliqueMember = new CliqueMemberCharacter
			{
				Member = discordUser.ActiveCharacter
			};

			var newClique = new Clique
			{
				CliqueName = form.CliqueName,
				CliqueMembers = new List<CliqueMemberCharacter>(),
				CliqueOfficers = new List<CliqueOfficerCharacter>(),
				Money = 0
			};

			newClique.CliqueOfficers.Add(newCliqueOfficer);
			newClique.CliqueMembers.Add(newCliqueMember);

			newCliqueOfficer.Clique = newClique;
			newCliqueMember.Clique = newClique;

			_context.Cliques.Add(newClique);
			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("user-is-clique-officer")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult UserIsCliqueOfficer(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null)
			{
				return Unauthorized();
			}

			var activeCharacter = discordUser.ActiveCharacter;

			var cliqueOfficer = _context.CliqueOfficers
				.SingleOrDefault(co => co.OfficerId == activeCharacter.CharacterId && co.CliqueId == id);

			if (cliqueOfficer == null)
			{
				return Unauthorized();
			}

			return Ok();
		}

		public struct InviteForm
		{
			public string CliqueId { get; set; }
			public string[] AddedCharacterIds { get; set; }
		}

		[HttpPost("send-invites")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public async Task<IActionResult> InviteCharactersToCliqueAsync([FromBody] InviteForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null)
			{
				return Unauthorized();
			}

			var clique = _context.Cliques.SingleOrDefault(c => c.CliqueId == form.CliqueId);

			var officers = clique.CliqueOfficers.Select(co => co.OfficerId);

			if (!officers.Contains(userId))
			{
				return Unauthorized();
			}

			foreach (var id in form.AddedCharacterIds)
			{
				var newInvite = new CliqueInvite
				{
					CliqueId = clique.CliqueId,
					CharacterId = id
				};

				_context.CliqueInvites.Add(newInvite);
			}

			await _context.SaveChangesAsync();

			return Ok();
		}
	}
}
