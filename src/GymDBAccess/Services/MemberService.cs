using GymDBAccess.DataAccess;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymDBAccess.Services
{
    public class MemberService : IMemberService
	{

		private readonly ApplicationDbContext _context;


		public MemberService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Member> GetMemberByIdAsync(int id)
		{
			return await _context.Members.FindAsync(id);
		}

		public async Task<IEnumerable<Member>> GetAllMembersAsync()
		{
			return await _context.Members.ToListAsync();
		}

		public async Task AddMemberAsync(Member member)
		{
			await _context.Members.AddAsync(member);
			await _context.SaveChangesAsync(); 
		}

		public async Task UpdateMemberAsync(Member member)
		{
			_context.Members.Update(member);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteMemberAsync(int id)
		{
			var memberToDelete = await _context.Members.FindAsync(id);
			if (memberToDelete != null)
			{
				_context.Members.Remove(memberToDelete);
				await _context.SaveChangesAsync();
			}
		}

	}
}
