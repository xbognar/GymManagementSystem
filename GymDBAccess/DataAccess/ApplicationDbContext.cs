using GymDBAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.DataAccess
{
	public class ApplicationDbContext : DbContext
	{

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		
		public DbSet<Member> Members { get; set; }
		
		public DbSet<Membership> Memberships { get; set; }
		
		public DbSet<Payment> Payments { get; set; }
		
		public DbSet<Chip> Chips { get; set; }
		
		public DbSet<GymAccessRecord> GymAccessRecords { get; set; }


	}
}
