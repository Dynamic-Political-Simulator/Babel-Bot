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

namespace WOPR.Controllers.Spending
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

		public struct CliqueInviteReplyForm
		{
			public string ClinqueInviteId { get; set; }
			public bool AcceptInvite { get; set; }
		}

		[HttpPost("reply-to-invite")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult CliqueInviteReply([FromBody] CliqueInviteReplyForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var invite = _context.CliqueInvites.SingleOrDefault(ci => ci.CliqueInviteId == form.ClinqueInviteId);

			if (form.AcceptInvite)
			{
				invite.Clique.CliqueMembers.Add(discordUser.ActiveCharacter);
				_context.Cliques.Update(invite.Clique);
			}

			_context.CliqueInvites.Remove(invite);

			_context.SaveChanges();

			return Ok();
		}

		public struct LeaveCliqueForm
		{
			public string CliqueId { get; set; }
		}

		[HttpPost("leave-clique")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult LeaveClique([FromBody] LeaveCliqueForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var clique = _context.Cliques.SingleOrDefault(c => c.CliqueId == form.CliqueId);

			if (clique != null && clique.CliqueMembers.Contains(discordUser.ActiveCharacter))
			{
				clique.CliqueMembers.Remove(discordUser.ActiveCharacter);
				clique.CliqueOfficers.Remove(discordUser.ActiveCharacter);
			}

			_context.SaveChanges();

			return Ok();
		}

		public struct InviteCharactersToCliqueForm
		{
			public string CliqueId { get; set; }
			public List<string> CharacterIds { get; set; }
		}

		[HttpPost("invite-to-clique")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult InviteCharactersToClique([FromBody] InviteCharactersToCliqueForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var clique = _context.Cliques.SingleOrDefault(c => c.CliqueId == form.CliqueId);

			if (!discordUser.ActiveCharacter.Cliques.Contains(clique) || !clique.CliqueOfficers.Contains(discordUser.ActiveCharacter))
			{
				return Unauthorized();
			}

			foreach (var characterid in form.CharacterIds)
			{
				var characterToInvite = _context.Characters.SingleOrDefault(c => c.CharacterId == characterid);

				if (characterToInvite != null)
				{
					var newCliqueInvite = new CliqueInvite()
					{
						CliqueId = clique.CliqueId,
						CharacterId = characterToInvite.CharacterId
					};

					_context.CliqueInvites.Add(newCliqueInvite);
				}
			}

			_context.SaveChanges();

			return Ok();
		}

		public struct CreateCliqueForm
		{
			public string CharacterId { get; set; }
			public string CliqueName { get; set; }
			public List<string> AlignmentIds { get; set; }
		}

		[HttpPost("create-clique")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult CreateClique([FromBody] CreateCliqueForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var alignmentsToJoin = new List<Alignment>();

			foreach (var align in form.AlignmentIds)
			{
				var alignmentToJoin = _context.Alignments.SingleOrDefault(a => a.AlignmentId == align);

				if (alignmentsToJoin != null)
				{
					alignmentsToJoin.Add(alignmentToJoin);
				}
			}

			if (alignmentsToJoin.Count == 0)
			{
				return BadRequest();
			}

			var newClique = new Clique()
			{
				CliqueName = form.CliqueName,
				Money = 0,
				CliqueMembers = new List<BabelDatabase.Character>(),
				CliqueOfficers = new List<BabelDatabase.Character>(),
				Alignments = new List<Alignment>()
			};

			newClique.CliqueMembers.Add(discordUser.ActiveCharacter);
			newClique.CliqueOfficers.Add(discordUser.ActiveCharacter);

			_context.Cliques.Add(newClique);

			_context.SaveChanges();

			return Ok();
		}

		public struct CustomSpendingForm
		{
			public string CliqueId { get; set; }
			public string CharacterId { get; set; }
			public ulong Amount { get; set; }

			public string Description { get; set; }
		}

		[HttpPost("custom-spending")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult CustomSpending([FromBody] CustomSpendingForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var clique = _context.Cliques.SingleOrDefault(c => c.CliqueId == form.CliqueId);

			if (!discordUser.ActiveCharacter.Cliques.Contains(clique) || !clique.CliqueOfficers.Contains(discordUser.ActiveCharacter))
			{
				return Unauthorized();
			}

			if (clique.Money < form.Amount)
			{
				return BadRequest();
			}

			clique.Money -= form.Amount;

			var newCustomSpending = new CustomSpending()
			{
				SpendingDescription = form.Description,
				Amount = form.Amount,
				CliqueId = clique.CliqueId,
				CharacterId = form.CharacterId
			};

			_context.Cliques.Update(clique);

			_context.CustomSpendings.Add(newCustomSpending);
			_context.SaveChanges();

			return Ok();
		}

		public struct AlignmentSpendingForm
		{
			public string CliqueId { get; set; }
			// Doesn't have to be own alignment, can be negative for rival alignments
			public string AlignmentId { get; set; }
			public string CharacterId { get; set; }

			public ulong Amount { get; set; }

			public string TargetPlanetEthicGroupId { get; set; }
			public string TargetGlobalEthicGroupId { get; set; }
		}

		[HttpGet("fund-alignment")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult SpendMoneyForAlignment([FromBody] AlignmentSpendingForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var clique = _context.Cliques.SingleOrDefault(c => c.CliqueId == form.CliqueId);

			if (!discordUser.ActiveCharacter.Cliques.Contains(clique) || !clique.CliqueOfficers.Contains(discordUser.ActiveCharacter))
			{
				return Unauthorized();
			}

			if (clique.Money < form.Amount)
			{
				return BadRequest();
			}

			clique.Money -= form.Amount;

			var newSpending = new AlignmentSpending()
			{
				Amount = form.Amount,
				CliqueId = form.CliqueId,
				CharacterId = form.CharacterId,
				AlignmentId = form.AlignmentId
			};

			if (form.TargetGlobalEthicGroupId != null)
			{
				newSpending.GlobalTargetId = form.TargetGlobalEthicGroupId;
			}
			else
			{
				newSpending.PlanetTargetId = form.TargetPlanetEthicGroupId;
			}

			_context.Cliques.Update(clique);

			_context.AlignmentSpendings.Add(newSpending);
			_context.SaveChanges();

			return Ok();
		}
	}
}
