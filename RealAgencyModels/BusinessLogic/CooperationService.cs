using Microsoft.EntityFrameworkCore;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.BusinessLogic
{
	public class CooperationService
	{
		private readonly RealAgencyDBContext _dbContext;

		public CooperationService(RealAgencyDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Создание новой записи
		public async Task<CooperationDTO> CreateAsync(CooperationDTO dto)
		{
			var model = new Сooperation
			{
				Bidpartnerid = dto.Bidpartnerid,
				Biduserid = dto.Biduserid
			};

			_dbContext.Сooperations.Add(model);
			await _dbContext.SaveChangesAsync();

			dto.Id = model.Id; // Возвращаем ID в DTO
			return dto;
		}

		// Получение записи по ID
		public async Task<CooperationDTO?> GetByIdAsync(int id)
		{
			var model = await _dbContext.Сooperations.FindAsync(id);
			if (model == null) return null;

			return new CooperationDTO
			{
				Id = model.Id,
				Bidpartnerid = model.Bidpartnerid,
				Biduserid = model.Biduserid
			};
		}

		// Получение всех записей
		public async Task<IEnumerable<CooperationDTO>> GetAllAsync()
		{
			return await _dbContext.Сooperations
				.Select(model => new CooperationDTO
				{
					Id = model.Id,
					Bidpartnerid = model.Bidpartnerid,
					Biduserid = model.Biduserid
				})
				.ToListAsync();
		}

		// Обновление записи
		public async Task<CooperationDTO?> UpdateAsync(int id, CooperationDTO dto)
		{
			var model = await _dbContext.Сooperations.FindAsync(id);
			if (model == null) return null;

			model.Bidpartnerid = dto.Bidpartnerid;
			model.Biduserid = dto.Biduserid;

			await _dbContext.SaveChangesAsync();
			return dto;
		}

		// Удаление записи
		public async Task<bool> DeleteAsync(int id)
		{
			var model = await _dbContext.Сooperations.FindAsync(id);
			if (model == null) return false;

			_dbContext.Сooperations.Remove(model);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
