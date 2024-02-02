﻿using GymDBAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymDBAccess.Services.Interfaces
{
	public interface IChipService
	{
		
		Task<Chip> GetChipByIdAsync(int id);
		
		Task<IEnumerable<Chip>> GetAllChipsAsync();
		
		Task AddChipAsync(Chip chip);
		
		Task UpdateChipAsync(Chip chip);
		
		Task DeleteChipAsync(int id);
	}
}
