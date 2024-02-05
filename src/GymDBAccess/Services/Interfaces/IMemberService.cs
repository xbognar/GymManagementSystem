using GymDBAccess.Models;

namespace GymDBAccess.Services.Interfaces
{
    public interface IMemberService
    {

        Task<Member> GetMemberByIdAsync(int id);

        Task<IEnumerable<Member>> GetAllMembersAsync();

        Task AddMemberAsync(Member member);

        Task UpdateMemberAsync(Member member);

        Task DeleteMemberAsync(int id);

    }
}