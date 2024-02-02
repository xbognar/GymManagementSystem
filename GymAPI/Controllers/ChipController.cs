using Microsoft.AspNetCore.Mvc;
using GymDBAccess.Models;
using GymDBAccess.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymDBAccess.Controllers
{
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
		public async Task<IActionResult> UpdateChip(int id, [FromBody] Chip chip)
		{
			if (id != chip.ChipID)
			{
				return BadRequest();
			}

			await _chipService.UpdateChipAsync(chip);
			return NoContent();
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
	}
}
