﻿using Microsoft.EntityFrameworkCore;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealAgencyModels.BusinessLogic.AnnouncementService;

namespace RealAgencyModels.BusinessLogic
{
	public class AnnouncementService
	{
		private readonly RealAgencyDBContext _dbContext;

		public AnnouncementService(RealAgencyDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Создание нового объявления
		public async Task<AnnouncementDTO> CreateAsync(AnnouncementDTO dto)
		{
			var model = new Announcement
			{
				Type = dto.Type,
				Description = dto.Description,
				Userid = dto.Userid
			};

			_dbContext.Announcements.Add(model);
			await _dbContext.SaveChangesAsync();

			dto.Id = model.Id; // Возвращаем сгенерированный ID в DTO
			return dto;
		}

		// Получение объявления по Id
		public async Task<AnnouncementDTO?> GetByIdAsync(int id)
		{
			var model = await _dbContext.Announcements.FindAsync(id);
			if (model == null) return null;

			return new AnnouncementDTO
			{
				Id = model.Id,
				Type = model.Type,
				Description = model.Description,
				Userid = model.Userid
			};
		}

		// Получение всех объявлений
		public async Task<IEnumerable<AnnouncementDTO>> GetAllAsync()
		{
			return await _dbContext.Announcements
				.Select(model => new AnnouncementDTO
				{
					Id = model.Id,
					Type = model.Type,
					Description = model.Description,
					Userid = model.Userid
				})
				.ToListAsync();
		}

		// Обновление объявления
		public async Task<AnnouncementDTO?> UpdateAsync(int id, AnnouncementDTO dto)
		{
			var model = await _dbContext.Announcements.FindAsync(id);
			if (model == null) return null;

			model.Type = dto.Type;
			model.Description = dto.Description;
			model.Userid = dto.Userid;

			await _dbContext.SaveChangesAsync();
			return dto;
		}

		// Удаление объявления
		public async Task<bool> DeleteAsync(int id)
		{
			var model = await _dbContext.Announcements.FindAsync(id);
			if (model == null) return false;

			_dbContext.Announcements.Remove(model);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
