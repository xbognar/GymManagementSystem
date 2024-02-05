using GymDBAccess.Models;
using GymDBAccess.DTOs;

namespace GymDBAccess.Services.Interfaces
{
	public interface IMembershipService
	{
		Task<Membership> GetMembershipByIdAsync(int id);
		
		Task<IEnumerable<Membership>> GetAllMembershipsAsync();
		
		Task AddMembershipAsync(Membership membership);
		
		Task UpdateMembershipAsync(Membership membership);
		
		Task DeleteMembershipAsnyc(int id); 
		
		Task<IEnumerable<ActiveMembershipDTO>> GetActiveMembershipsAsync();
		
		Task<IEnumerable<InactiveMembershipDTO>> GetInactiveMembershipsAsync();
	}
}