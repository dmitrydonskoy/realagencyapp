using Microsoft.EntityFrameworkCore;
using RealAgencyModels.BusinessLogic;
using RealAgencyModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RealAgencyModels.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class UserServiceTests
    {
        private readonly Mock<RealAgencyDBContext> _mockDbContext;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockDbContext = new Mock<RealAgencyDBContext>();
            _userService = new UserService(_mockDbContext.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Name = "John Doe", Email = "john@example.com", Role = "Agent", Password = "hashedpassword" };

            _mockDbContext.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _mockDbContext.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com", Role = "Agent", Password = "hashedpassword" },
            new User { Id = 2, Name = "Jane Doe", Email = "jane@example.com", Role = "Admin", Password = "hashedpassword" }
        };

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(users.AsQueryable().Provider));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(users.AsQueryable().Expression);

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(users.AsQueryable().ElementType);

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(users.GetEnumerator());

            _mockDbContext.Setup(x => x.Users).Returns(mockSet.Object);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Name = "John Doe", Email = "john@example.com", Role = "Agent", Password = "hashedpassword" };
            var updatedUserDto = new UserDTO { Id = userId, Name = "John Updated", Email = "john.updated@example.com", Role = "Admin", Password = "newpassword" };

            _mockDbContext.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(user);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateAsync(userId, updatedUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedUserDto.Name, user.Name);
            Assert.Equal(updatedUserDto.Email, user.Email);
            Assert.Equal(updatedUserDto.Role, user.Role);
            Assert.True(BCrypt.Net.BCrypt.Verify(updatedUserDto.Password, user.Password));
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var updatedUserDto = new UserDTO { Id = userId, Name = "John Updated", Email = "john.updated@example.com", Role = "Admin", Password = "newpassword" };

            _mockDbContext.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateAsync(userId, updatedUserDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Name = "John Doe", Email = "john@example.com", Role = "Agent", Password = "hashedpassword" };

            _mockDbContext.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync(user);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _mockDbContext.Setup(x => x.Users.FindAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = "john@example.com";
            var user = new User { Id = 1, Name = "John Doe", Email = email, Role = "Agent", Password = "hashedpassword" };

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(new List<User> { user }.GetEnumerator()));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(new List<User> { user }.AsQueryable().Provider));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(new List<User> { user }.AsQueryable().Expression);

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(new List<User> { user }.AsQueryable().ElementType);

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new List<User> { user }.GetEnumerator());

            _mockDbContext.Setup(x => x.Users).Returns(mockSet.Object);

            // Act
            var result = await _userService.GetByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<User>(new List<User>().GetEnumerator()));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(new List<User>().AsQueryable().Provider));

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(new List<User>().AsQueryable().Expression);

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(new List<User>().AsQueryable().ElementType);

            mockSet.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new List<User>().GetEnumerator());

            _mockDbContext.Setup(x => x.Users).Returns(mockSet.Object);

            // Act
            var result = await _userService.GetByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUser()
        {
            // Arrange
            var userDto = new UserDTO
            {
                Name = "John Doe",
                Email = "john@example.com",
                Role = "Agent",
                Password = "password123"
            };

            var createdUser = new User();
            var mockSet = new Mock<DbSet<User>>();

            _mockDbContext.Setup(x => x.Users).Returns(mockSet.Object);
            _mockDbContext.Setup(x => x.Users.Add(It.IsAny<User>()))
                .Callback<User>(u => createdUser = u);

            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1)
                .Verifiable();

            // Act
            var result = await _userService.CreateAsync(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.Name, result.Name);
            Assert.Equal(userDto.Email, result.Email);
           

            // Verify that Add and SaveChangesAsync were called
            mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            // Arrange
            var password = "password123";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Password = passwordHash };

            // Act
            var result = _userService.VerifyPassword(user, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            var password = "password123";
            var wrongPassword = "wrongpassword";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Password = passwordHash };

            // Act
            var result = _userService.VerifyPassword(user, wrongPassword);

            // Assert
            Assert.False(result);
        }
    }
}
