using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class AreaInfoController : ControllerBase
	{
		private readonly AreaInfoService _areaInfoService;

		public AreaInfoController(AreaInfoService areaInfoService)
		{
			_areaInfoService = areaInfoService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<AreaInfoDTO>> GetById(int id)
		{
			var areaInfo = await _areaInfoService.GetByIdAsync(id);
			if (areaInfo == null) return NotFound();
			return Ok(areaInfo);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<AreaInfoDTO>>> GetAll()
		{
			var areaInfos = await _areaInfoService.GetAllAsync();
			return Ok(areaInfos);
		}

		[HttpPost]
		public async Task<ActionResult<AreaInfoDTO>> Create(AreaInfoDTO dto)
		{
			var createdAreaInfo = await _areaInfoService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = createdAreaInfo.Id }, createdAreaInfo);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, AreaInfoDTO dto)
		{
			if (id != dto.Id) return BadRequest("ID mismatch");

			var updatedAreaInfo = await _areaInfoService.UpdateAsync(id, dto);
			if (updatedAreaInfo == null) return NotFound();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var isDeleted = await _areaInfoService.DeleteAsync(id);
			if (!isDeleted) return NotFound();

			return NoContent();
		}
	}
}
