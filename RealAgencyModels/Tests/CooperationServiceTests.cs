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
    public class CooperationServiceTests
    {
        private readonly Mock<RealAgencyDBContext> _mockDbContext;
        private readonly CooperationService _cooperationService;

        public CooperationServiceTests()
        {
            _mockDbContext = new Mock<RealAgencyDBContext>();
            _cooperationService = new CooperationService(_mockDbContext.Object);
        }

        // Test for CreateAsync method
        [Fact]
        public async Task CreateAsync_ShouldCreateCooperation_WhenValidDto()
        {
            // Arrange
            var dto = new CooperationDTO
            {
                Bidpartnerid = 1,
                Biduserid = 2
            };

            _mockDbContext.Setup(x => x.Сooperations.Add(It.IsAny<Сooperation>())).Verifiable();
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _cooperationService.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Bidpartnerid, result.Bidpartnerid);
            Assert.Equal(dto.Biduserid, result.Biduserid);
            _mockDbContext.Verify(x => x.Сooperations.Add(It.IsAny<Сooperation>()), Times.Once);
        }

        // Test for GetByIdAsync method
        [Fact]
        public async Task GetByIdAsync_ShouldReturnCooperation_WhenExists()
        {
            // Arrange
            var cooperationId = 1;
            var cooperation = new Сooperation { Id = cooperationId, Bidpartnerid = 1, Biduserid = 2 };

            _mockDbContext.Setup(x => x.Сooperations.FindAsync(cooperationId)).ReturnsAsync(cooperation);

            // Act
            var result = await _cooperationService.GetByIdAsync(cooperationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cooperationId, result.Id);
            Assert.Equal(1, result.Bidpartnerid);
            Assert.Equal(2, result.Biduserid);
        }

        // Test for GetAllAsync method
       




        // Test for UpdateAsync method
        [Fact]
        public async Task UpdateAsync_ShouldUpdateCooperation_WhenExists()
        {
            // Arrange
            var cooperationId = 1;
            var dto = new CooperationDTO { Id = cooperationId, Bidpartnerid = 3, Biduserid = 4 };
            var cooperation = new Сooperation { Id = cooperationId, Bidpartnerid = 1, Biduserid = 2 };

            _mockDbContext.Setup(x => x.Сooperations.FindAsync(cooperationId)).ReturnsAsync(cooperation);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _cooperationService.UpdateAsync(cooperationId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Bidpartnerid, result.Bidpartnerid);
            Assert.Equal(dto.Biduserid, result.Biduserid);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        // Test for DeleteAsync method
        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenCooperationNotFound()
        {
            // Arrange
            var cooperationId = 1;

            _mockDbContext.Setup(x => x.Сooperations.FindAsync(cooperationId)).ReturnsAsync((Сooperation)null);

            // Act
            var result = await _cooperationService.DeleteAsync(cooperationId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenCooperationExists()
        {
            // Arrange
            var cooperationId = 1;
            var cooperation = new Сooperation { Id = cooperationId };

            _mockDbContext.Setup(x => x.Сooperations.FindAsync(cooperationId)).ReturnsAsync(cooperation);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _cooperationService.DeleteAsync(cooperationId);

            // Assert
            Assert.True(result);
        }


        // Test for GetCooperationByUserIdAsync method
        [Fact]
        public async Task GetCooperationByUserIdAsync_ShouldReturnCooperation_WhenExists()
        {
            // Arrange
            var userId = 2;
            var cooperation = new Сooperation { Biduserid = userId, Bidpartnerid = 3 };

            // Создаем список с одной кооперацией
            var cooperationList = new List<Сooperation> { cooperation };

            // Мокируем IQueryable для DbSet<Сooperation>
            var mockSet = new Mock<DbSet<Сooperation>>();
            mockSet.As<IAsyncEnumerable<Сooperation>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Сooperation>(cooperationList.GetEnumerator()));

            mockSet.As<IQueryable<Сooperation>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Сooperation>(cooperationList.AsQueryable().Provider));

            mockSet.As<IQueryable<Сooperation>>()
                .Setup(m => m.Expression)
                .Returns(cooperationList.AsQueryable().Expression);

            mockSet.As<IQueryable<Сooperation>>()
                .Setup(m => m.ElementType)
                .Returns(cooperationList.AsQueryable().ElementType);

            mockSet.As<IQueryable<Сooperation>>()
                .Setup(m => m.GetEnumerator())
                .Returns(cooperationList.GetEnumerator());

            // Мокируем контекст базы данных
            _mockDbContext.Setup(x => x.Сooperations).Returns(mockSet.Object);

            // Act
            var result = await _cooperationService.GetCooperationByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cooperation.Biduserid, result.Biduserid);
        }


      
    }
}
