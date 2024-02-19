//using Microsoft.AspNetCore.Mvc;
//using GymDBAccess.Models;
//using GymDBAccess.Services.Interfaces;

//namespace GymAPI.Controllers
//{
//	[Route("api/[controller]")]
//	[ApiController]
//	public class AuthController : ControllerBase
//	{
//		private readonly IJwtService _jwtService;
//		private readonly IConfiguration _configuration;

//		public AuthController(IJwtService jwtService, IConfiguration configuration)
//		{
//			_jwtService = jwtService;
//			_configuration = configuration;
//		}

//		[HttpPost("login")]
//		public IActionResult Login([FromBody] LoginModel login)
//		{
//			// Retrieve credentials from appsettings.json
//			var validUsername = _configuration["Credentials:Username"];
//			var validPassword = _configuration["Credentials:Password"];

//			if (login.Username == validUsername && login.Password == validPassword)
//			{
//				var token = _jwtService.GenerateJwtToken(login.Username);
//				return Ok(new { Token = token });
//			}

//			return Unauthorized("Invalid credentials");
//		}
//	}
//}


using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GymAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IJwtService _jwtService;
		private readonly IConfiguration _configuration;

		public AuthController(IJwtService jwtService, IConfiguration configuration)
		{
			_jwtService = jwtService;
			_configuration = configuration;
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginModel login)
		{
			// Retrieve credentials from environment variables
			var validUsername = Environment.GetEnvironmentVariable("LOGIN_USERNAME") ?? "RozsaTomi";
			var validPassword = Environment.GetEnvironmentVariable("LOGIN_PASSWORD") ?? "TomiFit123"; 

			if (login.Username == validUsername && login.Password == validPassword)
			{
				var token = _jwtService.GenerateJwtToken(login.Username);
				return Ok(new { Token = token });
			}

			return Unauthorized("Invalid credentials");
		}
	}
}
