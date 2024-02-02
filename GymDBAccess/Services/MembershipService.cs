using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymDBAccess.Services
{
    public class MembershipService : IMembershipService
	{
		
		private readonly ApplicationDbContext _context;

		public MembershipService(ApplicationDbContext context) { 
			_context = context;

		}

		public async Task<Membership> GetMembershipByIdAsync(int id)
		{
			return await _context.Memberships.FindAsync(id);
		}

		public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
		{
			return await _context.Memberships.ToListAsync();
		}

		public async Task AddMembershipAsync(Membership membership)
		{
			await _context.Memberships.AddAsync(membership);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateMembershipAsync(Membership membership)
		{
			_context.Memberships.Update(membership);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteMembershipAsnyc(int id)
		{
			var membershipToChange = await _context.Memberships.FindAsync(id);

            if (membershipToChange != null)
            {
				_context.Memberships.Remove(membershipToChange);
				await _context.SaveChangesAsync();
            }
        }

	}
}
