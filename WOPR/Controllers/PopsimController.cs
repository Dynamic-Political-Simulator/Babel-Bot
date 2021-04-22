using BabelDatabase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WOPR.Services;

namespace WOPR.Controllers
{
	[Route("api/popsim/")]
	[ApiController]
	public class PopsimController : ControllerBase
	{
		private readonly PopsimService _popsimService;

		public PopsimController(PopsimService popsimService)
		{
			_popsimService = popsimService;
		}

		[HttpGet("get-popsim-report")]
		[Authorize(AuthenticationSchemes = "Discord")]
		public PopsimReport GetPopsimReport(string guid)
		{
			var report = _popsimService.GetExistingReport(guid);

			return report;
		}


	}
}
