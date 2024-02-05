using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymDBAccess.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace GymDBAccess.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class MembershipsController : ControllerBase
	{
		private readonly IMembershipService _membershipService;

		public MembershipsController(IMembershipService membershipService)
		{
			_membershipService = membershipService;
		}

		// GET: api/Memberships/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Membership>> GetMembership(int id)
		{
			var membership = await _membershipService.GetMembershipByIdAsync(id);
			if (membership == null)
			{
				return NotFound();
			}
			return Ok(membership);
		}

		// GET: api/Memberships
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Membership>>> GetAllMemberships()
		{
			var memberships = await _membershipService.GetAllMembershipsAsync();
			return Ok(memberships);
		}

		// POST: api/Memberships
		[HttpPost]
		public async Task<ActionResult<Membership>> AddMembership([FromBody] Membership membership)
		{
			await _membershipService.AddMembershipAsync(membership);
			return CreatedAtAction(nameof(GetMembership), new { id = membership.MemberID }, membership);
		}

		// PUT: api/Memberships/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateMembership(int id, [FromBody] Membership membership)
		{
			if (id != membership.MemberID)
			{
				return BadRequest();
			}

			await _membershipService.UpdateMembershipAsync(membership);
			return NoContent();
		}

		// DELETE: api/Memberships/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMembership(int id)
		{
			var membership = await _membershipService.GetMembershipByIdAsync(id);
			if (membership == null)
			{
				return NotFound();
			}

			await _membershipService.DeleteMembershipAsnyc(id);
			return NoContent();
		}

		// GET: api/Memberships/active
		[HttpGet("active")]
		public async Task<ActionResult<IEnumerable<ActiveMembershipDTO>>> GetActiveMemberships()
		{
			var activeMemberships = await _membershipService.GetActiveMembershipsAsync();
			if (activeMemberships == null)
			{
				return NotFound();
			}
			return Ok(activeMemberships);
		}

		// GET: api/Memberships/inactive
		[HttpGet("inactive")]
		public async Task<ActionResult<IEnumerable<InactiveMembershipDTO>>> GetInactiveMemberships()
		{
			var inactiveMemberships = await _membershipService.GetInactiveMembershipsAsync();
			if (inactiveMemberships == null)
			{
				return NotFound();
			}
			return Ok(inactiveMemberships);
		}
	}
}
