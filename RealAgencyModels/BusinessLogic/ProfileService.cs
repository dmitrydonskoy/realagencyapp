using Microsoft.EntityFrameworkCore;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.BusinessLogic
{
	public class ProfileService
	{
		private readonly RealAgencyDBContext _dbContext;

		public ProfileService(RealAgencyDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Создание новой записи
		public async Task<ProfileDTO> CreateAsync(ProfileDTO dto)
		{
			var model = new Profile
			{
				Userid = dto.Userid,
				Experience = dto.Experience,
				Transactions = dto.Transactions,
				Percent = dto.Percent,
				Description = dto.Description
			};

			_dbContext.Profiles.Add(model);
			await _dbContext.SaveChangesAsync();

			dto.Id = model.Id; // Возвращаем ID в DTO
			return dto;
		}

		// Получение записи по ID
		public async Task<ProfileDTO?> GetByIdAsync(int id)
		{
			var model = await _dbContext.Profiles.FindAsync(id);
			if (model == null) return null;

			return new ProfileDTO
			{
				Id = model.Id,
				Userid = model.Userid,
				Experience = model.Experience,
				Transactions = model.Transactions,
				Percent = model.Percent,
				Description = model.Description
			};
		}

		// Получение всех записей
		public async Task<IEnumerable<ProfileDTO>> GetAllAsync()
		{
			return await _dbContext.Profiles
				.Select(model => new ProfileDTO
				{
					Id = model.Id,
					Userid = model.Userid,
					Experience = model.Experience,
					Transactions = model.Transactions,
					Percent = model.Percent,
					Description = model.Description
				})
				.ToListAsync();
		}

		// Обновление записи
		public async Task<ProfileDTO?> UpdateAsync(int id, ProfileDTO dto)
		{
			var model = await _dbContext.Profiles.FindAsync(id);
			if (model == null) return null;

			model.Userid = dto.Userid;
			model.Experience = dto.Experience;
			model.Transactions = dto.Transactions;
			model.Percent = dto.Percent;
			model.Description = dto.Description;

			await _dbContext.SaveChangesAsync();
			return dto;
		}

		// Удаление записи
		public async Task<bool> DeleteAsync(int id)
		{
			var model = await _dbContext.Profiles.FindAsync(id);
			if (model == null) return false;

			_dbContext.Profiles.Remove(model);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
