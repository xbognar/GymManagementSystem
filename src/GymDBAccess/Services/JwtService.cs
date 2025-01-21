using System;
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
			// Safely handle null/empty JWT_KEY to avoid ArgumentNullException
			var rawKey = Environment.GetEnvironmentVariable("JWT_KEY");
			if (string.IsNullOrEmpty(rawKey))
			{
				rawKey = "TestFallbackKey_ChangeThisInProduction";
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(rawKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, username)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature
				)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
