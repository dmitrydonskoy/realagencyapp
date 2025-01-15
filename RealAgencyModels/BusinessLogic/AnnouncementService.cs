using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
		public async Task<CreateAnnouncementDTO> CreateAsync(CreateAnnouncementDTO dto)
		{
            // 1. Создаем Announcement
            var announcement = new Announcement
            {
                Type = dto.Type,
                Description = dto.Description,
                Userid = dto.UserId
            };

            _dbContext.Announcements.Add(announcement);
            await _dbContext.SaveChangesAsync();
            if (announcement.Type == "Продажа")
            {
                // 2. Создаем RealEstate с привязкой к Announcement
                var realEstate = new Realestate
                {
                    Address = dto.RealEstate.Address,
                    Rooms = dto.RealEstate.Rooms,
                    Type = dto.RealEstate.Type,
                    Square = dto.RealEstate.Square,
                    Floor = dto.RealEstate.Floor,
                    Bathroom = dto.RealEstate.Bathroom,
                    Repair = dto.RealEstate.Repair,
                    Furniture = dto.RealEstate.Furniture,
                    TransactionType = dto.RealEstate.TransactionType,
                    Price = dto.RealEstate.Price,
                    Description = dto.RealEstate.Description,
                    Announcementid = announcement.Id
                };
                _dbContext.Realestates.Add(realEstate);
                await _dbContext.SaveChangesAsync();
                if (realEstate.Type == "Дом")
                {
                    // 3. Создаем AreaInfo с привязкой к RealEstate
                    var areaInfo = new AreaInfo
                    {
                        Description = dto.AreaInfo.Description,
                        Square = dto.AreaInfo.Square,
                        Electricity = dto.AreaInfo.Electricity,
                        Heating = dto.AreaInfo.Heating,
                        WaterSupply = dto.AreaInfo.WaterSupply,
                        Gas = dto.AreaInfo.Gas,
                        Sewerage = dto.AreaInfo.Sewerage,
                        Realestateid = realEstate.Id
                    };

                    _dbContext.AreaInfos.Add(areaInfo);
                }

            
                await _dbContext.SaveChangesAsync();
            }

            dto.Id = announcement.Id; // Возвращаем ID в DTO
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

        public async Task<RealEstateDetailsDTO?> GetRealEstatePageDataAsync(int announcementId)
        {
            // Ищем объявление по его ID
            var announcement = await _dbContext.Announcements
                .FirstOrDefaultAsync(a => a.Id == announcementId);

            if (announcement == null)
                return null;

            // Ищем недвижимость по Announcementid
            var realEstate = await _dbContext.Realestates
                .Include(re => re.RealEstatePhotos)
                .FirstOrDefaultAsync(re => re.Announcementid == announcementId);

            if (realEstate == null)
                return null;

            // Ищем информацию об области по RealEstateId
            var areaInfo = await _dbContext.AreaInfos
                .FirstOrDefaultAsync(ai => ai.Realestateid == realEstate.Id);

            if (areaInfo == null)
                return null;

            return new RealEstateDetailsDTO
            {
                RealEstateId = realEstate.Id,
                Address = realEstate.Address,
                Rooms = realEstate.Rooms,
                Type = realEstate.Type,
                Square = realEstate.Square,
                Floor = realEstate.Floor,
                Bathroom = realEstate.Bathroom,
                Repair = realEstate.Repair,
                Furniture = realEstate.Furniture,
                TransactionType = realEstate.TransactionType,
                Price = realEstate.Price,
                Photos = realEstate.RealEstatePhotos.Select(p => p.Filepath).ToList(),
                AnnouncementTitle = announcement.Type,
                AnnouncementDescription = announcement.Description,
                AreaDescription = areaInfo.Description,
                AnnouncementId = announcement.Id
                
            };
        }

    }
}

