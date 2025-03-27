using Microsoft.AspNetCore.Mvc;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly UserService _userService;

		public UserController(UserService userService)
		{
			_userService = userService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<UserDTO>> GetById(int id)
		{
			var user = await _userService.GetByIdAsync(id);
			if (user == null) return NotFound();
			return Ok(user);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
		{
			var users = await _userService.GetAllAsync();
			return Ok(users);
		}

		[HttpPost]
		public async Task<ActionResult<UserDTO>> Create(UserDTO dto)
		{
			var createdUser = await _userService.CreateAsync(dto);
			return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, UserDTO dto)
		{
			if (id != dto.Id) return BadRequest("ID mismatch");

			var updatedUser = await _userService.UpdateAsync(id, dto);
			if (updatedUser == null) return NotFound();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var isDeleted = await _userService.DeleteAsync(id);
			if (!isDeleted) return NotFound();

			return NoContent();
		}
	}
}
