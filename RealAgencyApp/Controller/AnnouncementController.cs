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
        private readonly RealStateService _realestateService;
        public AnnouncementController(AnnouncementService announcementService, RealStateService realestateService)
        {
            _announcementService = announcementService;
            _realestateService = realestateService;
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
        [HttpGet]
        [Route("get-all")]
        public async Task<IActionResult> GetAllAnnouncement()
        {
            var realEstates = await _realestateService.GetAllRealEstatesAsync();
            return Ok(realEstates);
        }

        [HttpGet("page/{announcementId}")]
        public async Task<IActionResult> GetRealEstatePageData(int announcementId)
        {
            var data = await _announcementService.GetRealEstatePageDataAsync(announcementId);

            if (data == null)
                return NotFound(new { message = "Data not found" });

            return Ok(data);
        }
    }
}
