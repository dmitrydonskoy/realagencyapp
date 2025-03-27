using Microsoft.EntityFrameworkCore;
using Moq;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq.Expressions;

namespace RealAgencyModels.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Xunit;

    public class BidServiceTests
    {
        private readonly Mock<RealAgencyDBContext> _mockDbContext;
        private readonly BidService _bidService;

        public BidServiceTests()
        {
            _mockDbContext = new Mock<RealAgencyDBContext>();
            _bidService = new BidService(_mockDbContext.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddBidAndSaveChanges()
        {
            // Arrange
            var dto = new BidDTO { Partnerid = 1, Userid = 2 };
            var mockSet = new Mock<DbSet<Bid>>();

            _mockDbContext.Setup(x => x.Bids).Returns(mockSet.Object);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Verifiable();

            // Act
            var result = await _bidService.CreateAsync(dto);

            // Assert
            mockSet.Verify(x => x.Add(It.Is<Bid>(b =>
                b.Partnerid == dto.Partnerid && b.Userid == dto.Userid)), Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(dto.Partnerid, result.Partnerid);
            Assert.Equal(dto.Userid, result.Userid);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnBid_WhenExists()
        {
            // Arrange
            var partnerId = 1;
            var userId = 2;
            var bid = new Bid { Partnerid = partnerId, Userid = userId };

            var mockSet = new Mock<DbSet<Bid>>();
            mockSet.As<IAsyncEnumerable<Bid>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Bid>(new List<Bid> { bid }.GetEnumerator()));

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Bid>(new List<Bid> { bid }.AsQueryable().Provider));

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.Expression)
                .Returns(new List<Bid> { bid }.AsQueryable().Expression);

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.ElementType)
                .Returns(new List<Bid> { bid }.AsQueryable().ElementType);

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new List<Bid> { bid }.GetEnumerator());

            _mockDbContext.Setup(x => x.Bids).Returns(mockSet.Object);

            // Act
            var result = await _bidService.GetByIdsAsync(partnerId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(partnerId, result.Partnerid);
            Assert.Equal(userId, result.Userid);
        }

       

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBids()
        {
            // Arrange
            var bids = new List<Bid>
        {
            new Bid { Partnerid = 1, Userid = 2 },
            new Bid { Partnerid = 3, Userid = 4 }
        };

            var mockSet = new Mock<DbSet<Bid>>();
            mockSet.As<IAsyncEnumerable<Bid>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Bid>(bids.GetEnumerator()));

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Bid>(bids.AsQueryable().Provider));

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.Expression)
                .Returns(bids.AsQueryable().Expression);

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.ElementType)
                .Returns(bids.AsQueryable().ElementType);

            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.GetEnumerator())
                .Returns(bids.GetEnumerator());

            _mockDbContext.Setup(x => x.Bids).Returns(mockSet.Object);

            // Act
            var result = await _bidService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Partnerid == 1 && b.Userid == 2);
            Assert.Contains(result, b => b.Partnerid == 3 && b.Userid == 4);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrueAndRemove_WhenBidExists()
        {
            // Arrange
            var partnerId = 1;
            var userId = 2;
            var bid = new Bid { Partnerid = partnerId, Userid = userId };

            // Настраиваем мок для FirstOrDefaultAsync
            var mockSet = new Mock<DbSet<Bid>>();
            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Bid>(new List<Bid> { bid }.AsQueryable().Provider));
            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.Expression)
                .Returns(new List<Bid> { bid }.AsQueryable().Expression);
            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.ElementType)
                .Returns(new List<Bid> { bid }.AsQueryable().ElementType);
            mockSet.As<IQueryable<Bid>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new List<Bid> { bid }.GetEnumerator());

            _mockDbContext.Setup(x => x.Bids).Returns(mockSet.Object);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _bidService.DeleteAsync(partnerId, userId);

            // Assert
            Assert.True(result);
            mockSet.Verify(x => x.Remove(It.Is<Bid>(b =>
                b.Partnerid == partnerId && b.Userid == userId)), Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }

   
   
}
