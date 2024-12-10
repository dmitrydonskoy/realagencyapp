using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class CooperationController : ControllerBase
	{
		private readonly CooperationService _cooperationService;

		public CooperationController(CooperationService cooperationService)
		{
			_cooperationService = cooperationService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CooperationDTO>> GetById(int id)
		{
			var cooperation = await _cooperationService.GetByIdAsync(id);
			if (cooperation == null) return NotFound();
			return Ok(cooperation);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<CooperationDTO>>> GetAll()
		{
			var cooperations = await _cooperationService.GetAllAsync();
			return Ok(cooperations);
		}

		[HttpPost]
		public async Task<ActionResult<CooperationDTO>> Create(CooperationDTO dto)
		{
			var createdCooperation = await _cooperationService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = createdCooperation.Id }, createdCooperation);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, CooperationDTO dto)
		{
			if (id != dto.Id) return BadRequest("ID mismatch");

			var updatedCooperation = await _cooperationService.UpdateAsync(id, dto);
			if (updatedCooperation == null) return NotFound();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var isDeleted = await _cooperationService.DeleteAsync(id);
			if (!isDeleted) return NotFound();

			return NoContent();
		}
	}
}
