using GymDBAccess.DataAccess;
using GymDBAccess.DTOs;
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
    public class ChipService : IChipService 
	{
		private readonly ApplicationDbContext _context;

		public ChipService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Chip> GetChipByIdAsync(int id)
		{
			return await _context.Chips.FindAsync(id);
		}

		public async Task<IEnumerable<Chip>> GetAllChipsAsync()
		{
			return await _context.Chips.ToListAsync();
		}

		public async Task AddChipAsync(Chip chip)
		{
			await _context.Chips.AddAsync(chip);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateChipAsync(Chip chip)
		{
			_context.Chips.Update(chip);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteChipAsync(int id)
		{
			var chipToDelete = await _context.Chips.FindAsync(id);
			if (chipToDelete != null)
			{
				_context.Chips.Remove(chipToDelete);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<ActiveChipDTO>> GetActiveChipsAsync()
		{
			return await _context.Chips
				.Where(c => c.IsActive)
				.Join(_context.Members,
					chip => chip.MemberID,
					member => member.MemberID,
					(chip, member) => new ActiveChipDTO
					{
						ChipID = chip.ChipID,
						OwnerFullName = $"{member.FirstName} {member.LastName}",
						ChipInfo = chip.ChipInfo
					})
				.ToListAsync();
		}

		public async Task<IEnumerable<InactiveChipDTO>> GetInactiveChipsAsync()
		{
			return await _context.Chips
				.Where(c => !c.IsActive)
				.Join(_context.Members,
					chip => chip.MemberID,
					member => member.MemberID,
					(chip, member) => new InactiveChipDTO
					{
						ChipID = chip.ChipID,
						OwnerFullName = $"{member.FirstName} {member.LastName}",
						ChipInfo = chip.ChipInfo
					})
				.ToListAsync();
		}

		public async Task<string> GetChipInfoByMemberIdAsync(int memberId)
		{
			var chip = await _context.Chips
									 .FirstOrDefaultAsync(c => c.MemberID == memberId);

			return chip?.ChipInfo;
		}

	}
}
