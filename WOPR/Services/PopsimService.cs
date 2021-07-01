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

namespace WOPR.Services
{
	public class PopsimService
	{
		private readonly BabelContext _context;

		public PopsimService(BabelContext context)
		{
			_context = context;
		}

		
	}
}
