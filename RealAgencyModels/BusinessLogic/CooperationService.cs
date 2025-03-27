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

		// Получение записи по ID
		public async Task<IEnumerable<CooperationDTO?>> GetCooperationsByAgentAsync(int id)
		{
			return await _dbContext.Сooperations.Where(c => c.Bidpartnerid == id).Select(model => new CooperationDTO
			{
				Id = model.Id,
				Bidpartnerid = model.Bidpartnerid,
				Biduserid = model.Biduserid,
				ClientName = ""
			}).ToListAsync(); ;

		}
		public async Task SendProposalAsync(int announcementId, int clientId)
		{
			var announcement = await _dbContext.Announcements
				.Include(a => a.Сooperations)
				.FirstOrDefaultAsync(a => a.Id == announcementId);

			if (announcement == null)
			{
				throw new Exception("Announcement not found.");
			}

			var cooperation = await _dbContext.Сooperations
				.Include(c => c.Announcements)
				.FirstOrDefaultAsync(c => c.Biduserid == clientId);

			if (cooperation == null)
			{
				throw new Exception("Cooperation not found.");
			}

			cooperation.Announcements.Add(announcement);
			await _dbContext.SaveChangesAsync();
		}
		public async Task<CooperationDTO> GetCooperationByUserIdAsync(int userId)
		{
         return await _dbContext.Сooperations
        .Where(c => c.Biduserid == userId) // Фильтрация по userId
        .Select(c => new CooperationDTO
        {
            Id = c.Id,
            Bidpartnerid = c.Bidpartnerid,
            Biduserid = c.Biduserid,
            ClientName = "" // Поле ClientName должно существовать в таблице Cooperation
        })
        .FirstOrDefaultAsync();
        }
		public async Task<List<AnnouncementDescription>> GetAnnouncementsByCooperationIdAsync(int cooperationId)
		{
            return await _dbContext.Сooperations
        .Where(c => c.Id == cooperationId) // Фильтруем по cooperationId
        .SelectMany(c => c.Announcements) // Получаем связанные объявления
        .Include(a => a.Realestates) // Подгружаем связанные RealEstate
        .ThenInclude(re => re.RealEstatePhotos) // Подгружаем фотографии RealEstate
        .Select(a => new AnnouncementDescription
        {
            Id = a.Id,
            Description = a.Description, // Берем описание из Announcement
            Price = a.Realestates.First().Price, // Цена из Announcement
            Type = a.Type, // Тип из Announcement
            Photos = a.Realestates.SelectMany(re => re.RealEstatePhotos).Select(photo => photo.Filepath).ToList() // Собираем все фотографии
        })
        .ToListAsync();
          
		}
	}
}
