using Microsoft.EntityFrameworkCore;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.BusinessLogic
{
	public class AreaInfoService
	{
		private readonly RealAgencyDBContext _dbContext;

		public AreaInfoService(RealAgencyDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Создание новой записи
		public async Task<AreaInfoDTO> CreateAsync(AreaInfoDTO dto)
		{
			var model = new AreaInfo
			{
				Description = dto.Description,
				Square = dto.Square,
				Electricity = dto.Electricity,
				Heating = dto.Heating,
				WaterSupply = dto.WaterSupply,
				Gas = dto.Gas,
				Sewerage = dto.Sewerage,
				Realestateid = dto.Realestateid
			};

			_dbContext.AreaInfos.Add(model);
			await _dbContext.SaveChangesAsync();

			dto.Id = model.Id; // Возвращаем ID в DTO
			return dto;
		}

		// Получение записи по ID
		public async Task<AreaInfoDTO?> GetByIdAsync(int id)
		{
			var model = await _dbContext.AreaInfos.FindAsync(id);
			if (model == null) return null;

			return new AreaInfoDTO
			{
				Id = model.Id,
				Description = model.Description,
				Square = model.Square,
				Electricity = model.Electricity,
				Heating = model.Heating,
				WaterSupply = model.WaterSupply,
				Gas = model.Gas,
				Sewerage = model.Sewerage,
				Realestateid = model.Realestateid
			};
		}

		// Получение всех записей
		public async Task<IEnumerable<AreaInfoDTO>> GetAllAsync()
		{
			return await _dbContext.AreaInfos
				.Select(model => new AreaInfoDTO
				{
					Id = model.Id,
					Description = model.Description,
					Square = model.Square,
					Electricity = model.Electricity,
					Heating = model.Heating,
					WaterSupply = model.WaterSupply,
					Gas = model.Gas,
					Sewerage = model.Sewerage,
					Realestateid = model.Realestateid
				})
				.ToListAsync();
		}

		// Обновление записи
		public async Task<AreaInfoDTO?> UpdateAsync(int id, AreaInfoDTO dto)
		{
			var model = await _dbContext.AreaInfos.FindAsync(id);
			if (model == null) return null;

			model.Description = dto.Description;
			model.Square = dto.Square;
			model.Electricity = dto.Electricity;
			model.Heating = dto.Heating;
			model.WaterSupply = dto.WaterSupply;
			model.Gas = dto.Gas;
			model.Sewerage = dto.Sewerage;
			model.Realestateid = dto.Realestateid;

			await _dbContext.SaveChangesAsync();
			return dto;
		}

		// Удаление записи
		public async Task<bool> DeleteAsync(int id)
		{
			var model = await _dbContext.AreaInfos.FindAsync(id);
			if (model == null) return false;

			_dbContext.AreaInfos.Remove(model);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
