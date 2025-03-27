using Microsoft.EntityFrameworkCore;
using Moq;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace RealAgencyModels.Tests
{

    public class AnnouncementServiceTests
    {
        private readonly Mock<RealAgencyDBContext> _mockDbContext;
        private readonly AnnouncementService _announcementService;

        public AnnouncementServiceTests()
        {
            _mockDbContext = new Mock<RealAgencyDBContext>();
            _announcementService = new AnnouncementService(_mockDbContext.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAnnouncement_WhenValidDto()
        {
            
                // Arrange
                var createDto = new CreateAnnouncementDTO
                {
                    Type = "Продажа",
                    Description = "Продаю дом",
                    UserId = 1,
                    RealEstate = new RealstateDTO
                    {
                        Address = "ул. Ленина, 10",
                        Rooms = "3",
                        Type = "Дом",
                        Square = "100",
                        Floor = "1",
                        Bathroom = "1",
                        Repair = "Ремонт",
                        Furniture = "Есть",
                        TransactionType = "Продажа",
                        Price = 1000000,
                        Description = "Новый дом"
                    },
                    AreaInfo = new AreaInfoDTO
                    {
                        Description = "Хорошая зона",
                        Square = "200",
                        Electricity = "Есть",
                        Heating = "Есть",
                        WaterSupply = "Есть",
                        Gas = "Есть",
                        Sewerage = "Есть"
                    }
                };

                var announcement = new Announcement
                {
                    Id = 1,  // Устанавливаем ID вручную
                    Type = createDto.Type,
                    Description = createDto.Description,
                    Userid = createDto.UserId
                };

                var realEstate = new Realestate
                {
                    Id = 1,  // Устанавливаем ID вручную
                    Announcementid = announcement.Id,
                    Address = createDto.RealEstate.Address,
                    Rooms = createDto.RealEstate.Rooms,
                    Type = createDto.RealEstate.Type,
                    Square = createDto.RealEstate.Square,
                    Floor = createDto.RealEstate.Floor,
                    Bathroom = createDto.RealEstate.Bathroom,
                    Repair = createDto.RealEstate.Repair,
                    Furniture = createDto.RealEstate.Furniture,
                    TransactionType = createDto.RealEstate.TransactionType,
                    Price = createDto.RealEstate.Price,
                    Description = createDto.RealEstate.Description
                };

                var areaInfo = new AreaInfo
                {
                    Id = 1,  // Устанавливаем ID вручную
                    Realestateid = realEstate.Id,
                    Description = createDto.AreaInfo.Description,
                    Square = createDto.AreaInfo.Square,
                    Electricity = createDto.AreaInfo.Electricity,
                    Heating = createDto.AreaInfo.Heating,
                    WaterSupply = createDto.AreaInfo.WaterSupply,
                    Gas = createDto.AreaInfo.Gas,
                    Sewerage = createDto.AreaInfo.Sewerage
                };

                _mockDbContext.Setup(x => x.Announcements.Add(It.IsAny<Announcement>())).Verifiable();
                _mockDbContext.Setup(x => x.Realestates.Add(It.IsAny<Realestate>())).Verifiable();
                _mockDbContext.Setup(x => x.AreaInfos.Add(It.IsAny<AreaInfo>())).Verifiable();

              
                _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

               
                var createdResult = await _announcementService.CreateAsync(createDto);

                Assert.NotNull(createdResult);
                Assert.Equal(0, createdResult.Id);  
                Assert.Equal("Продажа", createdResult.Type);
                Assert.Equal("Продаю дом", createdResult.Description);
            }

        

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAnnouncement_WhenExists()
        {
            
            var announcement = new Announcement
            {
                Id = 7,
                Type = "Продажа",
                Description = "Продаю дом",
                Userid = 2
            };

            _mockDbContext.Setup(x => x.Announcements.FindAsync(7)).ReturnsAsync(announcement);

          
            var result = await _announcementService.GetByIdAsync(7);

            Assert.NotNull(result);
            Assert.Equal(7, result.Id);
            Assert.Equal("Продажа", result.Type);
            Assert.Equal("Продаю дом", result.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _mockDbContext.Setup(x => x.Announcements.FindAsync(7)).ReturnsAsync((Announcement)null);

            // Act
            var result = await _announcementService.GetByIdAsync(7);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAnnouncement_WhenExists()
        {
            // Arrange
            var announcement = new Announcement
            {
                Id = 7,
                Type = "Продажа",
                Description = "Продаю дом",
                Userid = 2
            };

            var updateDto = new AnnouncementDTO
            {
                Id = 7,
                Type = "Продажа",
                Description = "Снова продаю дом",
                Userid = 2
            };

            _mockDbContext.Setup(x => x.Announcements.FindAsync(7)).ReturnsAsync(announcement);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _announcementService.UpdateAsync(7, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Снова продаю дом", result.Description);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenAnnouncementNotFound()
        {
            // Arrange
            var updateDto = new AnnouncementDTO
            {
                Id = 7,
                Type = "Продажа",
                Description = "Снова продаю дом",
                Userid = 2
            };

            _mockDbContext.Setup(x => x.Announcements.FindAsync(7)).ReturnsAsync((Announcement)null);

            // Act
            var result = await _announcementService.UpdateAsync(7, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenAnnouncementExists()
        {
            // Arrange
            var announcement = new Announcement
            {
                Id = 7,
                Type = "Продажа",
                Description = "Продаю дом",
                Userid = 2
            };

            _mockDbContext.Setup(x => x.Announcements.FindAsync(7)).ReturnsAsync(announcement);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _announcementService.DeleteAsync(7);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenAnnouncementNotFound()
        {
            // Arrange
            _mockDbContext.Setup(x => x.Announcements.FindAsync(7)).ReturnsAsync((Announcement)null);

            // Act
            var result = await _announcementService.DeleteAsync(7);

            // Assert
            Assert.False(result);
        }
      


    }
}
