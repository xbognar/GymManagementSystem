using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GymAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class ChipsController : ControllerBase
	{
		private readonly IChipService _chipService;

		public ChipsController(IChipService chipService)
		{
			_chipService = chipService;
		}

		// GET: api/Chips/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Chip>> GetChip(int id)
		{
			var chip = await _chipService.GetChipByIdAsync(id);
			if (chip == null)
			{
				return NotFound();
			}
			return Ok(chip);
		}

		// GET: api/Chips
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Chip>>> GetAllChips()
		{
			var chips = await _chipService.GetAllChipsAsync();
			return Ok(chips);
		}

		// POST: api/Chips
		[HttpPost]
		public async Task<ActionResult<Chip>> AddChip([FromBody] Chip chip)
		{
			await _chipService.AddChipAsync(chip);
			return CreatedAtAction(nameof(GetChip), new { id = chip.ChipID }, chip);
		}

		// PUT: api/Chips/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateChip(int id, [FromBody] ChipUpdateRequest request)
		{
			if (id != request.ChipID)
			{
				return BadRequest("Chip ID does not match the route ID.");
			}

			var chip = await _chipService.GetChipByIdAsync(id);
			if (chip == null)
			{
				return NotFound();
			}

			chip.MemberID = request.NewMemberID;
												 
			await _chipService.UpdateChipAsync(chip);
			return Ok(chip);

		}

		// DELETE: api/Chips/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteChip(int id)
		{
			var chip = await _chipService.GetChipByIdAsync(id);
			if (chip == null)
			{
				return NotFound();
			}

			await _chipService.DeleteChipAsync(id);
			return NoContent();
		}

		// GET: api/Chips/active
		[HttpGet("active")]
		public async Task<IActionResult> GetActiveChips()
		{
			var chips = await _chipService.GetActiveChipsAsync();
			return Ok(chips);
		}

		// GET: api/Chips/inactive
		[HttpGet("inactive")]
		public async Task<IActionResult> GetInactiveChips()
		{
			var chips = await _chipService.GetInactiveChipsAsync();
			return Ok(chips);
		}

		// GET: api/Chips/infoByMember/{memberId}
		[HttpGet("infoByMember/{memberId}")]
		public async Task<ActionResult<string>> GetChipInfoByMemberId(int memberId)
		{
			var chipInfo = await _chipService.GetChipInfoByMemberIdAsync(memberId);

			if (chipInfo == null)
			{
				return NotFound($"No chip found for member with ID {memberId}.");
			}

			return Ok(chipInfo);
		}


	}
}
