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

namespace WOPR.Controllers.StaffAction
{
	[Route("api/staff-action")]
	[ApiController]
	public class StaffActionController : ControllerBase
	{
		private readonly BabelContext _context;

		public StaffActionController(BabelContext context)
		{
			_context = context;
		}

		public struct PlayersToAddForm
		{
			public List<string> PlayerIds { get; set; }
			public string StaffActionId { get; set; }
		}

		[HttpGet("post")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public async Task<IActionResult> AddPlayersToStaffAction([FromBody] PlayersToAddForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var staffAction = _context.StaffActions.SingleOrDefault(sa => sa.StaffActionId == form.StaffActionId);

			if (staffAction == null)
			{
				return NotFound();
			}

			if (staffAction.OwnerId != userId && !discordUser.IsAdmin)
			{
				return Unauthorized();
			}

			foreach (var id in form.PlayerIds)
			{
				var playerToAdd = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == id);

				if (playerToAdd != null && !staffAction.Players.Contains(playerToAdd))
				{
					staffAction.Players.Add(playerToAdd);
				}
			}

			_context.StaffActions.Update(staffAction);
			await _context.SaveChangesAsync();

			return Ok();
		}

		public struct NewStaffActionPostForm
		{
			public string PostContent { get; set; }
			public string StaffActionId { get; set; }
		}

		[HttpGet("post")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public async Task<IActionResult> NewStaffActionPost([FromBody] NewStaffActionPostForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var newPost = new StaffActionPost()
			{
				Content = form.PostContent,
				TimePosted = DateTime.UtcNow,
				StaffActionId = form.StaffActionId,
				AuthorId = discordUser.DiscordUserId
			};

			_context.StaffActionPosts.Add(newPost);
			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet("get-staff-action")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetStaffAction(string id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var staffAction = _context.StaffActions
				.Include(sa => sa.Players)
				.Include(sa => sa.Staff)
				.Include(sa => sa.Owner)
				.Include(sa => sa.StaffActionPosts)
				.ThenInclude(sa => sa.Author)
				.SingleOrDefault(sa => sa.StaffActionId == id);

			if (!staffAction.Players.Contains(discordUser) && !discordUser.IsAdmin)
			{
				return Unauthorized();
			}

			return Ok(staffAction);
		}
		
		public struct StaffActionCreateForm
		{
			public string Title { get; set; }
			public string FirstPost { get; set; }
			public List<string> AddedPlayerIds { get; set; }
		}

		[HttpGet("create-staff-action")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult CreateStaffAction([FromBody] StaffActionCreateForm form)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var addedDiscordUsers = new List<BabelDatabase.DiscordUser>();

			foreach (var id in form.AddedPlayerIds)
			{
				var user = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == id);

				if (user != null)
				{
					addedDiscordUsers.Add(user);
				}
			}

			var newStaffAction = new BabelDatabase.StaffAction() 
			{
				Title = form.Title,
				TimeStarted = DateTime.UtcNow,
				OwnerId = discordUser.DiscordUserId,
				Players = addedDiscordUsers,
				StaffActionPosts = new List<StaffActionPost>()
			};

			var firstPost = new StaffActionPost()
			{
				Content = form.FirstPost,
				TimePosted = DateTime.UtcNow,
				StaffActionId = newStaffAction.StaffActionId,
				AuthorId = discordUser.DiscordUserId
			};

			newStaffAction.StaffActionPosts.Add(firstPost);

			_context.StaffActions.Add(newStaffAction);
			_context.SaveChanges();

			return Ok();
		}

		public struct MyStaffActionsReturn
		{
			public string StaffActionId { get; set; }
			public string Title { get; set; }
			public List<string> Players { get; set; }
			public List<string> Staff { get; set; }
			public DateTime TimePosted { get; set; }
			public string StartedBy { get; set; }
		}

		[HttpGet("my-staff-actions")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public IActionResult GetMyStaffActions()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var discordUser = _context.DiscordUsers.SingleOrDefault(du => du.DiscordUserId == userId);

			if (userId == null || discordUser == null)
			{
				return Unauthorized();
			}

			var staffActions = _context.StaffActions.Where(sa => sa.OwnerId == userId || sa.Players.Contains(discordUser) || sa.Staff.Contains(discordUser));

			var returnList = new List<MyStaffActionsReturn>();

			foreach (var sa in staffActions)
			{
				var newEntry = new MyStaffActionsReturn
				{
					StaffActionId = sa.StaffActionId,
					Title = sa.Title,
					StartedBy = sa.Owner.UserName,
					TimePosted = sa.TimeStarted,
					Players = new List<string>(),
					Staff = new List<string>()
				};

				foreach (var player in sa.Players)
				{
					newEntry.Players.Add(player.UserName);
				}

				foreach (var staff in sa.Staff)
				{
					newEntry.Staff.Add(staff.UserName);
				}

				returnList.Add(newEntry);
			}

			return Ok(returnList);
		}
	}
}
