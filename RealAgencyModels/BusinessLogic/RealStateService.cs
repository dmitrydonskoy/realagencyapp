using Microsoft.EntityFrameworkCore;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RealAgencyModels.BusinessLogic
{
	public class RealStateService
	{
		private readonly RealAgencyDBContext _dbContext;

		public RealStateService(RealAgencyDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Создание новой записи
		public async Task<RealstateDTO> CreateAsync(RealstateDTO dto)
		{
			var model = new Realestate
			{
				Address = dto.Address,
				Rooms = dto.Rooms,
				Type = dto.Type,
				Square = dto.Square,
				Floor = dto.Floor,
				Bathroom = dto.Bathroom,
				Repair = dto.Repair,
				Furniture = dto.Furniture,
				TransactionType = dto.TransactionType,
				Price = dto.Price,
				Description = dto.Description,
				Announcementid = dto.Announcementid
			};

			_dbContext.Realestates.Add(model);
			await _dbContext.SaveChangesAsync();

			dto.Id = model.Id; // Возвращаем ID в DTO
			return dto;
		}

		// Получение записи по ID
		public async Task<RealstateDTO?> GetByIdAsync(int id)
		{
			var model = await _dbContext.Realestates.FindAsync(id);
			if (model == null) return null;

			return new RealstateDTO
			{
				Id = model.Id,
				Address = model.Address,
				Rooms = model.Rooms,
				Type = model.Type,
				Square = model.Square,
				Floor = model.Floor,
				Bathroom = model.Bathroom,
				Repair = model.Repair,
				Furniture = model.Furniture,
				TransactionType = model.TransactionType,
				Price = model.Price,
				Description = model.Description,
				Announcementid = model.Announcementid
			};
		}

		// Получение всех записей
		public async Task<IEnumerable<RealstateDTO>> GetAllAsync()
		{
			return await _dbContext.Realestates
				.Select(model => new RealstateDTO
				{
					Id = model.Id,
					Address = model.Address,
					Rooms = model.Rooms,
					Type = model.Type,
					Square = model.Square,
					Floor = model.Floor,
					Bathroom = model.Bathroom,
					Repair = model.Repair,
					Furniture = model.Furniture,
					TransactionType = model.TransactionType,
					Price = model.Price,
					Description = model.Description,
					Announcementid = model.Announcementid
				})
				.ToListAsync();
		}

		// Обновление записи
		public async Task<RealstateDTO?> UpdateAsync(int id, RealstateDTO dto)
		{
			var model = await _dbContext.Realestates.FindAsync(id);
			if (model == null) return null;

			model.Address = dto.Address;
			model.Rooms = dto.Rooms;
			model.Type = dto.Type;
			model.Square = dto.Square;
			model.Floor = dto.Floor;
			model.Bathroom = dto.Bathroom;
			model.Repair = dto.Repair;
			model.Furniture = dto.Furniture;
			model.TransactionType = dto.TransactionType;
			model.Price = dto.Price;
			model.Description = dto.Description;
			model.Announcementid = dto.Announcementid;

			await _dbContext.SaveChangesAsync();
			return dto;
		}

		// Удаление записи
		public async Task<bool> DeleteAsync(int id)
		{
			var model = await _dbContext.Realestates.FindAsync(id);
			if (model == null) return false;

			_dbContext.Realestates.Remove(model);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
