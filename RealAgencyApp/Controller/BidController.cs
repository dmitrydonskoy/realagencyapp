using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class BidController : ControllerBase
	{
		private readonly BidService _bidService;

		public BidController(BidService bidService)
		{
			_bidService = bidService;
		}

		// Создание новой записи
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] BidDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _bidService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetByIds), new { partnerId = result.Partnerid, userId = result.Userid }, result);
		}

		// Получение записи по составному ключу
		[HttpGet("{partnerId}/{userId}")]
		public async Task<IActionResult> GetByIds(int partnerId, int userId)
		{
			var result = await _bidService.GetByIdsAsync(partnerId, userId);
			if (result == null)
				return NotFound();

			return Ok(result);
		}

		// Получение всех записей
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var results = await _bidService.GetAllAsync();
			return Ok(results);
		}

		// Удаление записи по составному ключу
		[HttpDelete("{partnerId}/{userId}")]
		public async Task<IActionResult> Delete(int partnerId, int userId)
		{
			var success = await _bidService.DeleteAsync(partnerId, userId);
			if (!success)
				return NotFound();

			return NoContent();
		}
	}
}
