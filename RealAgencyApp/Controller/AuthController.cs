using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealAgencyModels;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealAgencyApp.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly UserService _userService;
		private readonly IConfiguration _configuration;

		public AuthController(UserService userService, IConfiguration configuration)
		{
			_userService = userService;
			_configuration = configuration;
		}

		// Регистрация пользователя
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] UserDTO userDto)
		{
			var existingUser = await _userService.GetByEmailAsync(userDto.Email);
			if (existingUser != null)
			{
				return BadRequest("User with this email already exists.");
			}

			var user = await _userService.CreateAsync(userDto);
			return Ok(user);
		}

		// Авторизация пользователя
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
		{
			var user = await _userService.GetByEmailAsync(loginDto.Email);
			if (user == null || !_userService.VerifyPassword(user, loginDto.Password))
			{
				return Unauthorized("Invalid credentials.");
			}

			var token = GenerateJwtToken(user);
			return Ok(new { Token = token });
		}

		// Генерация JWT токена
		private string GenerateJwtToken(User user)
		{
			var jwtSettings = _configuration.GetSection("Jwt");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim(ClaimTypes.Role, user.Role),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["TokenLifetime"])),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
