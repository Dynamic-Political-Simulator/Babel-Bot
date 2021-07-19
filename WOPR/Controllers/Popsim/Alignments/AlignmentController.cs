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
    public class AlignmentController : ControllerBase
    {
        private readonly BabelContext _context;

        public AlignmentController(BabelContext context)
        {
            _context = context;
        }

        public struct AlignmentOverviewReturn
        {
            public string AlignmentId { get; set; }
            public string AlignmentName { get; set; }
            public float Establishment { get; set; }
            public int FederalismCentralism { get; set; }
            public int DemocracyAuthority { get; set; }
            public int GlobalismIsolationism { get; set; }
            public int MilitarismPacifism { get; set; }
            public int SecurityFreedom { get; set; }
            public int CooperationCompetition { get; set; }
            public int SecularismSpiritualism { get; set; }
            public int ProgressivismTraditionalism { get; set; }
            public int MonoculturalismMulticulturalism { get; set; }
        }

        [HttpGet("alignment-overview")]
        public IActionResult GetAlignmentOverview()
        {
            var alignments = _context.Alignments.ToList();

            var returnList = new List<AlignmentOverviewReturn>();

            foreach (var a in alignments)
            {
                returnList.Add(new AlignmentOverviewReturn()
                {
                    AlignmentId = a.AlignmentId,
                    AlignmentName = a.AlignmentName,
                    Establishment = a.Establishment,
                    FederalismCentralism = a.FederalismCentralism,
                    DemocracyAuthority = a.DemocracyAuthority,
                    GlobalismIsolationism = a.GlobalismIsolationism,
                    MilitarismPacifism = a.MilitarismPacifism,
                    SecurityFreedom = a.SecurityFreedom,
                    CooperationCompetition = a.CooperationCompetition,
                    SecularismSpiritualism = a.SecularismSpiritualism,
                    ProgressivismTraditionalism = a.ProgressivismTraditionalism,
                    MonoculturalismMulticulturalism = a.MonoculturalismMulticulturalism
                });
            }

            return Ok(returnList);
        }

        [HttpGet("get-alignment")]
        public IActionResult GetAlignment(string id)
        {
            var a = _context.Alignments.SingleOrDefault(a => a.AlignmentId == id);

            if (a == null)
            {
                return NotFound();
            }

            var alignmentReturn = new AlignmentOverviewReturn()
            {
                AlignmentId = a.AlignmentId,
                AlignmentName = a.AlignmentName,
                Establishment = a.Establishment,
                FederalismCentralism = a.FederalismCentralism,
                DemocracyAuthority = a.DemocracyAuthority,
                GlobalismIsolationism = a.GlobalismIsolationism,
                MilitarismPacifism = a.MilitarismPacifism,
                SecurityFreedom = a.SecurityFreedom,
                CooperationCompetition = a.CooperationCompetition,
                SecularismSpiritualism = a.SecularismSpiritualism,
                ProgressivismTraditionalism = a.ProgressivismTraditionalism,
                MonoculturalismMulticulturalism = a.MonoculturalismMulticulturalism
            };

            return Ok(alignmentReturn);
        }

        public struct AlignmentEditForm
        {
            public string AlignmentId { get; set; }
            public string AlignmentName { get; set; }
            public float Establishment { get; set; }
            public int FederalismCentralism { get; set; }
            public int DemocracyAuthority { get; set; }
            public int GlobalismIsolationism { get; set; }
            public int MilitarismPacifism { get; set; }
            public int SecurityFreedom { get; set; }
            public int CooperationCompetition { get; set; }
            public int SecularismSpiritualism { get; set; }
            public int ProgressivismTraditionalism { get; set; }
            public int MonoculturalismMulticulturalism { get; set; }
        }

        [HttpPost("edit-alignment")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult EditAlignment([FromBody] AlignmentEditForm form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            var alignment = _context.Alignments.SingleOrDefault(a => a.AlignmentId == form.AlignmentId);

            if (alignment == null)
            {
                return NotFound();
            }

            alignment.AlignmentName = form.AlignmentName;
            alignment.Establishment = form.Establishment;
            alignment.FederalismCentralism = form.FederalismCentralism;
            alignment.DemocracyAuthority = form.DemocracyAuthority;
            alignment.GlobalismIsolationism = form.GlobalismIsolationism;
            alignment.MilitarismPacifism = form.MilitarismPacifism;
            alignment.SecurityFreedom = form.SecurityFreedom;
            alignment.CooperationCompetition = form.CooperationCompetition;
            alignment.SecularismSpiritualism = form.SecularismSpiritualism;
            alignment.ProgressivismTraditionalism = form.ProgressivismTraditionalism;
            alignment.MonoculturalismMulticulturalism = form.MonoculturalismMulticulturalism;

            _context.Update(alignment);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("create-alignment")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> CreateAlignment()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            var newAlignment = new Alignment()
            {
                AlignmentName = "New Alignment",
                Establishment = 0,
                FederalismCentralism = 0,
                DemocracyAuthority = 0,
                GlobalismIsolationism = 0,
                MilitarismPacifism = 0,
                SecurityFreedom = 0,
                CooperationCompetition = 0,
                SecularismSpiritualism = 0,
                ProgressivismTraditionalism = 0,
                MonoculturalismMulticulturalism = 0
            };

            _context.Alignments.Add(newAlignment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("delete-alignment")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public async Task<IActionResult> DeleteAlignment(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

            if (discordUser == null || discordUser.IsAdmin == false)
            {
                return Unauthorized();
            }

            var alignment = _context.Alignments.SingleOrDefault(a => a.AlignmentId == id);

            _context.Alignments.Remove(alignment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public struct AlignmentSearchReturn
        {
            public string AlignmentId { get; set; }
            public string AlignmentName { get; set; }
        }

        [HttpGet("search-alignment")]
        public IActionResult SearchAlignment(string search)
        {
            var returnList = new List<AlignmentSearchReturn>();

            if (search == null)
            {
                return Ok(returnList);
            }

            var alignments = _context.Alignments.Where(a => a.AlignmentName.ToLower().Contains(search.ToLower())).ToList();

            foreach (var a in alignments)
            {
                returnList.Add(new AlignmentSearchReturn
                {
                    AlignmentId = a.AlignmentId,
                    AlignmentName = a.AlignmentName
                });
            }

            return Ok(returnList);
        }
    }
}
