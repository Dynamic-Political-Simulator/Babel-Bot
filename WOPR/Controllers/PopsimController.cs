using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WOPR.Services;

namespace WOPR.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PopsimController : ControllerBase
	{
		private readonly PopsimService _popsimService;

		public PopsimController(PopsimService popsimService)
		{
			_popsimService = popsimService;
		}


	}
}
