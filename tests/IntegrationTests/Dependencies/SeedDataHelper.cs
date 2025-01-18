using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationTests.Dependencies
{
	/// <summary>
	/// Provides initial test data seeding for integration tests.
	/// </summary>
	public static class SeedDataHelper
	{
		public static void Seed(ApplicationDbContext db)
		{
			// If there's data already, skip
			if (db.Members.Any()) return;

			// 1) Seed Members
			var memberAlice = new Member
			{
				MemberID = 101,
				FirstName = "Alice",
				LastName = "Smith",
				DateOfBirth = new DateTime(1990, 1, 1),
				Email = "alice@gym.com"
			};
			var memberBob = new Member
			{
				MemberID = 102,
				FirstName = "Bob",
				LastName = "Johnson",
				DateOfBirth = new DateTime(1985, 2, 2),
				Email = "bob@gym.com"
			};
			db.Members.AddRange(memberAlice, memberBob);

			// 2) Seed Memberships
			var membership1 = new Membership
			{
				MembershipID = 201,
				MemberID = 101,
				StartDate = DateTime.UtcNow.AddDays(-10),
				EndDate = DateTime.UtcNow.AddDays(20),
				PaymentType = "Monthly",
				IsActive = true
			};
			var membership2 = new Membership
			{
				MembershipID = 202,
				MemberID = 102,
				StartDate = DateTime.UtcNow.AddDays(-30),
				EndDate = DateTime.UtcNow.AddDays(-1),
				PaymentType = "Annual",
				IsActive = false
			};
			db.Memberships.AddRange(membership1, membership2);

			// 3) Seed Chips
			var chip1 = new Chip
			{
				ChipID = 301,
				MemberID = 101,
				ChipInfo = "VIP Access",
				IsActive = true
			};
			var chip2 = new Chip
			{
				ChipID = 302,
				MemberID = 102,
				ChipInfo = "Basic Access",
				IsActive = false
			};
			db.Chips.AddRange(chip1, chip2);

			db.SaveChanges();
		}
	}
}
