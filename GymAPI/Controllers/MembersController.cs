using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GymDBAccess.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class MembersController : ControllerBase
	{
		private readonly IMemberService _memberService;

		public MembersController(IMemberService memberService)
		{
			_memberService = memberService;
		}

		// GET: api/Members/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Member>> GetMember(int id)
		{
			var member = await _memberService.GetMemberByIdAsync(id);
			if (member == null)
			{
				return NotFound();
			}
			return member;
		}

		// GET: api/Members
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Member>>> GetAllMembers()
		{
			var members = await _memberService.GetAllMembersAsync();
			return Ok(members);
		}

		// POST: api/Members
		[HttpPost]
		public async Task<ActionResult<Member>> AddMember([FromBody] Member member)
		{
			await _memberService.AddMemberAsync(member);
			return CreatedAtAction(nameof(GetMember), new { id = member.MemberID }, member);
		}

		// PUT: api/Members/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateMember(int id, [FromBody] Member member)
		{
			if (id != member.MemberID)
			{
				return BadRequest();
			}

			await _memberService.UpdateMemberAsync(member);
			return NoContent();
		}

		// DELETE: api/Members/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteMember(int id)
		{
			var memberToDelete = await _memberService.GetMemberByIdAsync(id);
			if (memberToDelete == null)
			{
				return NotFound();
			}

			await _memberService.DeleteMemberAsync(id);
			return NoContent();
		}
	}
}
