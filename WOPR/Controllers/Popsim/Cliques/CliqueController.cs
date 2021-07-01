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

			if (discordUser.ActiveCharacterId == null)
			{
				return NotFound();
			}

			var activeCharacter = _context.Characters.SingleOrDefault(c => c.CharacterId == discordUser.ActiveCharacterId);

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
			public List<CliqueMember> Members { get; set; }
			public List<CliqueMember> Officers { get; set; }
			public IEnumerable<string> Alignments { get; set; }
			public ulong Money { get; set; }
			public bool UserIsOfficer { get; set; }
		}

		public struct CliqueMember
		{
			public string CharacterId { get; set; }
			public string CharacterName { get; set; }
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

			if (discordUser.ActiveCharacterId == null)
			{
				return NotFound();
			}

			var activeCharacter = _context.Characters.SingleOrDefault(c => c.CharacterId == discordUser.ActiveCharacterId);

			var clique = activeCharacter.Cliques.SingleOrDefault(c => c.CliqueId == id);

			if (clique == null && !discordUser.IsAdmin)
			{
				return Unauthorized();
			}

			var cliqueReturn = new CliqueReturn
			{
				CliqueName = clique.Clique.CliqueName,
				Members = clique.Clique.CliqueMemberCharacter.Select(cm =>  new CliqueMember 
				{
					CharacterId = cm.MemberId,
					CharacterName = cm.Member.CharacterName
				}).ToList(),
				Officers = clique.Clique.CliqueOfficerCharacter.Select(cm => new CliqueMember
				{
					CharacterId = cm.OfficerId,
					CharacterName = cm.Officer.CharacterName
				}).ToList(),
				Alignments = clique.Clique.Alignments.Select(a => a.Alignment.AlignmentName),
				Money = clique.Clique.Money,
				UserIsOfficer = false
			};

			if (cliqueReturn.Officers.Select(o => o.CharacterId).Contains(activeCharacter.CharacterId))
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

			if (discordUser == null || discordUser.ActiveCharacterId == null)
			{
				return Unauthorized();
			}

			var newCliqueOfficer = new CliqueOfficerCharacter
			{
				OfficerId = discordUser.ActiveCharacterId
			};

			var newCliqueMember = new CliqueMemberCharacter
			{
				MemberId = discordUser.ActiveCharacterId
			};

			var newClique = new Clique
			{
				CliqueName = form.CliqueName,
				CliqueMemberCharacter = new List<CliqueMemberCharacter>(),
				CliqueOfficerCharacter = new List<CliqueOfficerCharacter>(),
				Money = 0
			};

			newClique.CliqueOfficerCharacter.Add(newCliqueOfficer);
			newClique.CliqueMemberCharacter.Add(newCliqueMember);

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

			if (discordUser == null || discordUser.ActiveCharacterId == null)
			{
				return Unauthorized();
			}

			var activeCharacter = _context.Characters.SingleOrDefault(c => c.CharacterId == discordUser.ActiveCharacterId);

			var cliqueOfficer = _context.CliqueOfficerCharacter
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

			var officers = clique.CliqueOfficerCharacter.Select(co => co.OfficerId);

			if (!officers.Contains(discordUser.ActiveCharacterId))
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

		public struct KickForm
		{
			public string CliqueId { get; set; }
			public string CharacterId { get; set; }
		}

		[HttpPost("kick")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public async Task<IActionResult> KickMember([FromBody] KickForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null)
			{
				return Unauthorized();
			}

			var clique = _context.Cliques.SingleOrDefault(c => c.CliqueId == form.CliqueId);

			var officers = clique.CliqueOfficerCharacter.Select(co => co.OfficerId);

			if (!officers.Contains(discordUser.ActiveCharacterId))
			{
				return Unauthorized();
			}

			var member = clique.CliqueMemberCharacter.SingleOrDefault(cmc => cmc.MemberId == form.CharacterId);

			_context.CliqueMemberCharacter.Remove(member);
			await _context.SaveChangesAsync();

			return Ok();
		}
	}
}
