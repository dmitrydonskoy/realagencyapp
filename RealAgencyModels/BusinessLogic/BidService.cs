using Microsoft.EntityFrameworkCore;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealAgencyModels.BusinessLogic
{
	public class BidService
	{
		private readonly RealAgencyDBContext _dbContext;

		public BidService(RealAgencyDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		// Создание новой записи
		public async Task<BidDTO> CreateAsync(BidDTO dto)
		{
			var model = new Bid
			{
				Partnerid = dto.Partnerid,
				Userid = dto.Userid
			};

			_dbContext.Bids.Add(model);
			await _dbContext.SaveChangesAsync();

			return dto; // ID не генерируется, так как ключ составной
		}

		// Получение записи по составному ключу
		public async Task<BidDTO?> GetByIdsAsync(int partnerId, int userId)
		{
			var model = await _dbContext.Bids
				.FirstOrDefaultAsync(b => b.Partnerid == partnerId && b.Userid == userId);

			if (model == null) return null;

			return new BidDTO
			{
				Partnerid = model.Partnerid,
				Userid = model.Userid
			};
		}

		// Получение всех записей
		public async Task<IEnumerable<BidDTO>> GetAllAsync()
		{
			return await _dbContext.Bids
				.Select(model => new BidDTO
				{
					Partnerid = model.Partnerid,
					Userid = model.Userid
				})
				.ToListAsync();
		}

		// Удаление записи по составному ключу
		public async Task<bool> DeleteAsync(int partnerId, int userId)
		{
			var model = await _dbContext.Bids
				.FirstOrDefaultAsync(b => b.Partnerid == partnerId && b.Userid == userId);

			if (model == null) return false;

			_dbContext.Bids.Remove(model);
			await _dbContext.SaveChangesAsync();
			return true;
		}
	}
}
