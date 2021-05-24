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
	public class AlignmentController : ControllerBase
	{
		private readonly BabelContext _context;

		public AlignmentController(BabelContext context)
		{
			_context = context;
		}

		public struct PopsimFactionSpendingForm
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		//[HttpGet("fund-faction")]
		//[Authorize(AuthenticationSchemes = "Discord")]
		//public IActionResult SpendFaction()
		//{

		//}

	}
}
