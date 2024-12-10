using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class AnnouncementController : ControllerBase
	{
		private readonly AnnouncementService _announcementService;

		public AnnouncementController(AnnouncementService announcementService)
		{
			_announcementService = announcementService;
		}

		// Создание нового объявления
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] AnnouncementDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _announcementService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}

		// Получение объявления по Id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await _announcementService.GetByIdAsync(id);
			if (result == null)
				return NotFound();

			return Ok(result);
		}

		// Получение всех объявлений
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var results = await _announcementService.GetAllAsync();
			return Ok(results);
		}

		// Обновление объявления
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] AnnouncementDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _announcementService.UpdateAsync(id, dto);
			if (result == null)
				return NotFound();

			return Ok(result);
		}

		// Удаление объявления
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var success = await _announcementService.DeleteAsync(id);
			if (!success)
				return NotFound();

			return NoContent();
		}
	}
}
