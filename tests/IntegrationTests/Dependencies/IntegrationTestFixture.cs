using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using GymDBAccess.DataAccess;
using GymAPI;
using System;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// WebApplicationFactory fixture for integration tests.
	/// Sets environment to "IntegrationTest" so Program.cs chooses a unique InMemory DB,
	/// then seeds the DB with test data.
	/// </summary>
	public class IntegrationTestFixture : WebApplicationFactory<Program>
	{
		protected override IHost CreateHost(IHostBuilder builder)
		{
			// Force environment to "IntegrationTest"
			builder.UseEnvironment("IntegrationTest");

			// Also set environment variables so AuthController can authenticate, etc.
			Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
			Environment.SetEnvironmentVariable("LOGIN_USERNAME", "testUser");
			Environment.SetEnvironmentVariable("LOGIN_PASSWORD", "testPass");
			Environment.SetEnvironmentVariable("JWT_KEY", "TestIntegrationJwtKeyOfAtLeast32Characters!!");
			Environment.SetEnvironmentVariable("CONNECTION_STRING", "FakeIntegrationConnString");

			// Let the base create the host
			var host = base.CreateHost(builder);

			// Now seed the brand-new InMemory DB:
			using var scope = host.Services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			SeedDataHelper.Seed(db);

			return host;
		}
	}
}
