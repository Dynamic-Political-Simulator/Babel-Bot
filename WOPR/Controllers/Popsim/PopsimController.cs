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

namespace WOPR.Controllers.Popsim
{
	[Route("api/[controller]")]
	[ApiController]
	public class PopsimController : ControllerBase
	{
		private readonly BabelContext _context;

		public PopsimController(BabelContext context)
		{
			_context = context;
		}

		public struct PopsimPartyOverviewReturn
		{
			public float UpperPartyPercentage { get; set; }
			public List<PopsimOverviewGlobalEthicGroup> GlobalEthicGroups { get; set; }
			public List<PopsimOverviewAlignment> Alignments { get; set; }
		}

		public struct PopsimOverviewGlobalEthicGroup
		{
			public string Name { get; set; }
			public int PartyInvolvementFactor { get; set; }
			public float Radicalisation { get; set; }
			public float PartyEnlistmentModifier { get; set; }

			public int FederalismCentralism { get; set; }
			public int DemocracyAuthority { get; set; }
			public int GlobalismIsolationism { get; set; }
			public int MilitarismPacifism { get; set; }
			public int SecurityFreedom { get; set; }
			public int CooperationCompetition { get; set; }
			public int SecularismSpiritualism { get; set; }
			public int ProgressivismTraditionalism { get; set; }
			public int MonoculturalismMulticulturalism { get; set; }

			public float PopGroupEnlistment { get; set; }
			public float UpperPartyMembership { get; set; }
			public float LowerPartyMembership { get; set; }
		}

		public struct PopsimOverviewAlignment
		{
			public string Name { get; set; }

			public int FederalismCentralism { get; set; }
			public int DemocracyAuthority { get; set; }
			public int GlobalismIsolationism { get; set; }
			public int MilitarismPacifism { get; set; }
			public int SecurityFreedom { get; set; }
			public int CooperationCompetition { get; set; }
			public int SecularismSpiritualism { get; set; }
			public int ProgressivismTraditionalism { get; set; }
			public int MonoculturalismMulticulturalism { get; set; }

			public float Establishment { get; set; }
			public float UpperPartyModifier { get; set; }
			public float LowerPartyModifier { get; set; }
			public float UpperPartyAffinity { get; set; }
			public float LowerPartyAffinity { get; set; }

			public List<string> Cliques { get; set; }
		}

		[HttpGet("overview")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetPopsimPartyOverview()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (discordUser == null || !discordUser.IsAdmin)
			{
				return Unauthorized();
			}

			var party = _context.Parties.First();

			var overview = new PopsimPartyOverviewReturn();

			overview.GlobalEthicGroups = new List<PopsimOverviewGlobalEthicGroup>();
			overview.Alignments = new List<PopsimOverviewAlignment>();

			foreach (var enlistment in party.PopGroupEnlistment)
			{
				var ppeg = enlistment.Key;
				var newEntry = new PopsimOverviewGlobalEthicGroup()
				{
					Name = ppeg.PopsimGlobalEthicGroupName,
					FederalismCentralism = ppeg.FederalismCentralism,
					DemocracyAuthority = ppeg.DemocracyAuthority,
					GlobalismIsolationism = ppeg.GlobalismIsolationism,
					MilitarismPacifism = ppeg.MilitarismPacifism,
					SecurityFreedom = ppeg.SecurityFreedom,
					CooperationCompetition = ppeg.CooperationCompetition,
					SecularismSpiritualism = ppeg.SecularismSpiritualism,
					ProgressivismTraditionalism = ppeg.ProgressivismTraditionalism,
					MonoculturalismMulticulturalism = ppeg.MonoculturalismMulticulturalism,
					PopGroupEnlistment = enlistment.Value
				};
			}

			foreach (var upperMembership in party.UpperPartyMembership)
			{
				var entry = overview.GlobalEthicGroups.SingleOrDefault(geg => geg.Name == upperMembership.Key.PopsimGlobalEthicGroupName);
				entry.UpperPartyMembership = upperMembership.Value;
			}

			foreach (var lowerMembership in party.LowerPartyMembership)
			{
				var entry = overview.GlobalEthicGroups.SingleOrDefault(geg => geg.Name == lowerMembership.Key.PopsimGlobalEthicGroupName);
				entry.LowerPartyMembership = lowerMembership.Value;
			}

			foreach (var lowerAffinity in party.LowerPartyAffinity)
			{
				var a = lowerAffinity.Key;
				var newEntry = new PopsimOverviewAlignment()
				{
					Name = a.AlignmentName,
					FederalismCentralism = a.FederalismCentralism,
					DemocracyAuthority = a.DemocracyAuthority,
					GlobalismIsolationism = a.GlobalismIsolationism,
					MilitarismPacifism = a.MilitarismPacifism,
					SecurityFreedom = a.SecurityFreedom,
					CooperationCompetition = a.CooperationCompetition,
					SecularismSpiritualism = a.SecularismSpiritualism,
					ProgressivismTraditionalism = a.ProgressivismTraditionalism,
					MonoculturalismMulticulturalism = a.MonoculturalismMulticulturalism,
					Establishment = a.Establishment,
					LowerPartyModifier = a.LowerPartyModiifer,
					UpperPartyModifier = a.UpperPartyModifier,
					LowerPartyAffinity = lowerAffinity.Value,
					Cliques = a.AlignmentClique.Select(ac => ac.Clique.CliqueName).ToList()
				};
			}

			foreach (var upperAffinity in party.UpperPartyAffinity)
			{
				var entry = overview.Alignments.SingleOrDefault(a => a.Name == upperAffinity.Key.AlignmentName);
				entry.UpperPartyAffinity = upperAffinity.Value;
			}

			return Ok(overview);
		}
	}
}
