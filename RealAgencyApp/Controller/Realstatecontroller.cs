using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class RealstateController : ControllerBase
	{
		private readonly RealStateService _realstateService;

		public RealstateController(RealStateService realstateService)
		{
			_realstateService = realstateService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<RealstateDTO>> GetById(int id)
		{
			var realstate = await _realstateService.GetByIdAsync(id);
			if (realstate == null) return NotFound();
			return Ok(realstate);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<RealstateDTO>>> GetAll()
		{
			var realstates = await _realstateService.GetAllAsync();
			return Ok(realstates);
		}

		[HttpPost]
		public async Task<ActionResult<RealstateDTO>> Create(RealstateDTO dto)
		{
			var createdRealstate = await _realstateService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = createdRealstate.Id }, createdRealstate);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, RealstateDTO dto)
		{
			if (id != dto.Id) return BadRequest("ID mismatch");

			var updatedRealstate = await _realstateService.UpdateAsync(id, dto);
			if (updatedRealstate == null) return NotFound();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var isDeleted = await _realstateService.DeleteAsync(id);
			if (!isDeleted) return NotFound();

			return NoContent();
		}
	}
}
