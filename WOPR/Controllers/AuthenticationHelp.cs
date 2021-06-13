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

namespace WOPR.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationHelp : ControllerBase
    {
        [HttpGet("auth")]
        [Authorize(AuthenticationSchemes = "Discord")]
        public IActionResult AuthHelp()
        {
            return Redirect("http://localhost:3000");
        }
    }
}
