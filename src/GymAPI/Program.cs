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
			// Create builder
			var builder = WebApplication.CreateBuilder(args);

			// If in "IntegrationTest" environment, use a UNIQUE InMemory DB name
			if (builder.Environment.IsEnvironment("IntegrationTest"))
			{
				var uniqueDbName = $"GymIntegrationTestDB_{Guid.NewGuid()}";
				builder.Services.AddDbContext<ApplicationDbContext>(options =>
					options.UseInMemoryDatabase(uniqueDbName));
			}
			else
			{
				// Use real DB
				var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
									   ?? "FallbackOrThrow";
				builder.Services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(connectionString));
			}

			// Add Controllers & other services
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Add your usual DI for services
			builder.Services.AddScoped<IMemberService, MemberService>();
			builder.Services.AddScoped<IMembershipService, MembershipService>();
			builder.Services.AddScoped<IChipService, ChipService>();

			// JWT setup
			string jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? "TestFallbackJwtKey_ChangeMe";
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

			builder.Services.AddSingleton<IJwtService, JwtService>();

			// Build app
			var app = builder.Build();

			// If not in IntegrationTest, do DB migrate
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
