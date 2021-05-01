using BabelDatabase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WOPR.Controllers.Character
{
	[Route("api/[controller]")]
	[ApiController]
	public class AdminCharacterController : ControllerBase
	{
		private readonly BabelContext _context;

		public AdminCharacterController(BabelContext context)
		{
			_context = context;
		}

		
	}
}
