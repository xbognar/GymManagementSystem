//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using GymDBAccess.Services.Interfaces;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;

//namespace GymDBAccess.Services
//{
//	public class JwtService : IJwtService
//	{
//		private readonly IConfiguration _configuration;

//		public JwtService(IConfiguration configuration)
//		{
//			_configuration = configuration;
//		}

//		public string GenerateJwtToken(string username)
//		{
//			var tokenHandler = new JwtSecurityTokenHandler();
//			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
//			var tokenDescriptor = new SecurityTokenDescriptor
//			{
//				Subject = new ClaimsIdentity(new[]
//				{
//					new Claim(ClaimTypes.Name, username)
//				}),
//				Expires = DateTime.UtcNow.AddHours(1), 
//				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
//				Issuer = _configuration["Jwt:Issuer"],
//				Audience = _configuration["Jwt:Audience"]
//			};

//			var token = tokenHandler.CreateToken(tokenDescriptor);
//			return tokenHandler.WriteToken(token);
//		}
//	}
//}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GymDBAccess.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace GymDBAccess.Services
{
	public class JwtService : IJwtService
	{
		public string GenerateJwtToken(string username)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? "8Zz5tw0Ionm3XPZZfN0NOmAHsUBT8E8Ff6a2");
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, username)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
