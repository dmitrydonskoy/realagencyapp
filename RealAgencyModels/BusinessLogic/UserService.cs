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

		// Создание нового пользователя
		public async Task<UserDTO> CreateAsync(UserDTO dto)
		{
			var model = new User
			{
				Name = dto.Name,
				Role = dto.Role,
				Email = dto.Email,
				Password = dto.Password,
				DateOfBirth = dto.DateOfBirth
			};

			_dbContext.Users.Add(model);
			await _dbContext.SaveChangesAsync();

			dto.Id = model.Id; // Возвращаем ID в DTO
			return dto;
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
				DateOfBirth = model.DateOfBirth
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
					Password = model.Password,
					DateOfBirth = model.DateOfBirth
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
			model.DateOfBirth = dto.DateOfBirth;

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
	}
}
