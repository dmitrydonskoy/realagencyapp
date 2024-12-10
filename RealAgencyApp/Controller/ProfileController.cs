using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProfileController : ControllerBase
	{
		private readonly ProfileService _profileService;

		public ProfileController(ProfileService profileService)
		{
			_profileService = profileService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProfileDTO>> GetById(int id)
		{
			var profile = await _profileService.GetByIdAsync(id);
			if (profile == null) return NotFound();
			return Ok(profile);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProfileDTO>>> GetAll()
		{
			var profiles = await _profileService.GetAllAsync();
			return Ok(profiles);
		}

		[HttpPost]
		public async Task<ActionResult<ProfileDTO>> Create(ProfileDTO dto)
		{
			var createdProfile = await _profileService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = createdProfile.Id }, createdProfile);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, ProfileDTO dto)
		{
			if (id != dto.Id) return BadRequest("ID mismatch");

			var updatedProfile = await _profileService.UpdateAsync(id, dto);
			if (updatedProfile == null) return NotFound();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var isDeleted = await _profileService.DeleteAsync(id);
			if (!isDeleted) return NotFound();

			return NoContent();
		}
	}
}
