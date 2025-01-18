using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GymDBAccess.DataAccess;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// Custom WebApplicationFactory that overrides the real DB with an in-memory DB
	/// and seeds test data on startup.
	/// </summary>
	public class IntegrationTestFixture : WebApplicationFactory<Program>
	{
		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// Remove existing DbContext registration (SQL)
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
				if (descriptor != null) services.Remove(descriptor);

				// Register InMemory DB instead
				services.AddDbContext<ApplicationDbContext>(options =>
				{
					options.UseInMemoryDatabase("GymIntegrationTestDb");
				});

				// Build the service provider so we can seed data
				var sp = services.BuildServiceProvider();

				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var db = scopedServices.GetRequiredService<ApplicationDbContext>();

					// Ensure the database is created
					db.Database.EnsureCreated();

					// Seed test data
					SeedDataHelper.Seed(db);
				}
			});

			return base.CreateHost(builder);
		}
	}
}
