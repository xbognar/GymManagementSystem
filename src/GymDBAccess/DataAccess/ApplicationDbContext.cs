using GymDBAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace GymDBAccess.DataAccess
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// Make DbSet<T> properties virtual for Moq
		public virtual DbSet<Member> Members { get; set; }
		
		public virtual DbSet<Membership> Memberships { get; set; }
		
		public virtual DbSet<Chip> Chips { get; set; }
	}
}
