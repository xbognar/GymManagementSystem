﻿using GymDBAccess.Models;

namespace GymDBAccess.Services.Interfaces
{
    public interface IMembershipService
    {

		Task<Membership> GetMembershipByIdAsync(int id);

		Task<IEnumerable<Membership>> GetAllMembershipsAsync();

		Task AddMembershipAsync(Membership membership);

		Task UpdateMembershipAsync(Membership membership);

		Task DeleteMembershipAsnyc(int id);

	}
}