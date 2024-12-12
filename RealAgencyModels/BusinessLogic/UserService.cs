using Microsoft.EntityFrameworkCore;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.BusinessLogic
{
	public class UserService
	{
		private readonly RealAgencyDBContext _dbContext;

		public UserService(RealAgencyDBContext dbContext)
		{
			_dbContext = dbContext;
		}


		// Получение пользователя по ID
		public async Task<UserDTO?> GetByIdAsync(int id)
		{
			var model = await _dbContext.Users.FindAsync(id);
			if (model == null) return null;

			return new UserDTO
			{
				Id = model.Id,
				Name = model.Name,
				Role = model.Role,
				Email = model.Email,
				Password = model.Password,
				
			};
		}

		// Получение всех пользователей
		public async Task<IEnumerable<UserDTO>> GetAllAsync()
		{
			return await _dbContext.Users
				.Select(model => new UserDTO
				{
					Id = model.Id,
					Name = model.Name,
					Role = model.Role,
					Email = model.Email,
					Password = model.Password
					
				})
				.ToListAsync();
		}

		// Обновление пользователя
		public async Task<UserDTO?> UpdateAsync(int id, UserDTO dto)
		{
			var model = await _dbContext.Users.FindAsync(id);
			if (model == null) return null;

			model.Name = dto.Name;
			model.Role = dto.Role;
			model.Email = dto.Email;
			model.Password = dto.Password;
		

			await _dbContext.SaveChangesAsync();
			return dto;
		}

		// Удаление пользователя
		public async Task<bool> DeleteAsync(int id)
		{
			var model = await _dbContext.Users.FindAsync(id);
			if (model == null) return false;

			_dbContext.Users.Remove(model);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		public async Task<User?> GetByEmailAsync(string email)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task<User> CreateAsync(UserDTO dto)
		{
			var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password); // Хеширование пароля

			var user = new User
			{
				Name = dto.Name,
				Email = dto.Email,
				Role = "User", // Назначьте роль по умолчанию
				Password = passwordHash,
				DateOfBirth = new DateOnly(2001, 10, 11)
			};

			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();
			return user;
		}

		public bool VerifyPassword(User user, string password)
		{
			return BCrypt.Net.BCrypt.Verify(password, user.Password);
		}
	}
}
