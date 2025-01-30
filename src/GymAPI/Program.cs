using GymDBAccess.DataAccess;
using GymDBAccess.Services;
using GymDBAccess.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GymAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			if (builder.Environment.IsEnvironment("IntegrationTest"))
			{
				// 1. Use InMemory DB for integration tests
				// 2. Do NOT run migrations in this scenario
				var uniqueDbName = $"GymIntegrationTestDB_{Guid.NewGuid()}";
				builder.Services.AddDbContext<ApplicationDbContext>(opts =>
					opts.UseInMemoryDatabase(uniqueDbName));
			}
			else
			{
				// Use the real DB in any other environment
				var connectionString =
					Environment.GetEnvironmentVariable("CONNECTION_STRING")
					?? throw new InvalidOperationException("Missing CONNECTION_STRING env variable.");

				builder.Services.AddDbContext<ApplicationDbContext>(opts =>
					opts.UseSqlServer(connectionString));
			}

			// Add other services
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Add DI for your custom services
			builder.Services.AddScoped<IMemberService, MemberService>();
			builder.Services.AddScoped<IMembershipService, MembershipService>();
			builder.Services.AddScoped<IChipService, ChipService>();

			// JWT setup
			var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "TestFallbackJwtKey_ChangeMe";
			var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

			builder.Services.AddAuthentication(opts =>
			{
				opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(opts =>
			{
				opts.RequireHttpsMetadata = false;
				opts.SaveToken = true;
				opts.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

			// Register your JwtService as a Singleton
			builder.Services.AddSingleton<IJwtService, JwtService>();

			var app = builder.Build();

			// Auto-migrate if NOT "IntegrationTest"
			if (!app.Environment.IsEnvironment("IntegrationTest"))
			{
				using var scope = app.Services.CreateScope();
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				dbContext.Database.Migrate();
			}

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			app.Run();
		}
	}
}
