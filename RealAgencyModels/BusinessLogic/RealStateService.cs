using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
        private readonly IWebHostEnvironment _environment;
        public RealStateService(RealAgencyDBContext dbContext, IWebHostEnvironment environment)
		{
			_dbContext = dbContext;
			_environment = environment;
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
        public async Task<List<AnnouncementDescription>> GetAllRealEstatesAsync()
        {
			
            return await _dbContext.Realestates
                .Include(re => re.RealEstatePhotos) // Подгружаем связанные фотографии
                .Select(re => new AnnouncementDescription
                {
                    Id = re.Announcementid,
                    Description = re.Description,
                    Price = re.Price,
                    Type = re.Type,
					
                    Photos = re.RealEstatePhotos.Select(photo => photo.Filepath).ToList()
                })
                .ToListAsync();
        }
        public async Task<(bool Success, string Message, string? Filepath)> UploadPhotoAsync(int realEstateId, IFormFile photo)
        {
            if (realEstateId <= 0)
            {
                return (false, "Invalid real estate ID.", null);
            }

            if (photo == null || photo.Length == 0)
            {
                return (false, "Please select a valid photo.", null);
            }

            try
            {
                // Получаем путь для сохранения файла в физической файловой системе
                var uploadPath = Path.Combine(
                    _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
                    "uploads/realestate"
                );
                Directory.CreateDirectory(uploadPath);

                // Генерируем уникальное имя файла
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var filePath = Path.Combine(uploadPath, fileName);

                // Сохраняем файл на сервер
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Находим объект недвижимости по ID
                var realEstate = await _dbContext.Realestates.FindAsync(realEstateId);
                if (realEstate == null)
                {
                    return (false, "Real estate not found.", null);
                }

                // Сохраняем путь до фотографии в базу данных
                var fileUrl = $"/uploads/realestate/{fileName}";
                var fullUrl = $"https://localhost:7023{fileUrl}";
                realEstate.RealEstatePhotos.Add(new RealEstatePhoto { Filepath = fullUrl });
                await _dbContext.SaveChangesAsync();

                // Формируем полный URL для доступа
             

                // Возвращаем успешный результат с полным URL
                return (true, "Photo uploaded successfully.", fullUrl);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }
    }
}
