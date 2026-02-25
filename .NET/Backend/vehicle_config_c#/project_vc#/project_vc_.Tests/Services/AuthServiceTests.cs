using Xunit;
using project_vc_.Services;
using project_vc_.DTOs;
using project_vc_.Models;
using project_vc_.Data;
using Microsoft.EntityFrameworkCore;
using Moq;



namespace project_vc_.Tests.Services
{
    public class AuthServiceTests
    {
        // Creates fresh in-memory database for each test
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // Mocks JwtUtil so we don't use real JWT logic
        private Mock<IJwtUtil> GetJwtUtilMock()
        {
            var mock = new Mock<IJwtUtil>();

            mock.Setup(j => j.GenerateToken(It.IsAny<string>()))
                .Returns("fake-jwt-token");

            return mock;
        }


        // -------------------- TEST 1 --------------------
        [Fact]
        public void Login_InvalidUsername_ThrowsException()
        {
            var context = GetDbContext();
            var jwtMock = GetJwtUtilMock();
            var service = new AuthService(context, jwtMock.Object);

            var request = new LoginRequest
            {
                Username = "unknown",
                Password = "password"
            };

            var ex = Assert.Throws<Exception>(() => service.Login(request));

            Assert.Equal("Invalid username or password", ex.Message);
        }

        // -------------------- TEST 2 --------------------
        [Fact]
        public void Login_BlockedUser_ThrowsException()
        {
            var context = GetDbContext();

            context.Users.Add(new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("123"),
                IsBlocked = true
            });

            context.SaveChanges();

            var jwtMock = GetJwtUtilMock();
            var service = new AuthService(context, jwtMock.Object);

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "123"
            };

            var ex = Assert.Throws<Exception>(() => service.Login(request));

            Assert.Equal("User is blocked", ex.Message);
        }

        // -------------------- TEST 3 --------------------
        [Fact]
        public void Login_WrongPassword_IncreasesFailedAttempts()
        {
            var context = GetDbContext();

            var user = new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("correct"),
                FailedAttempts = 0,
                IsBlocked = false
            };

            context.Users.Add(user);
            context.SaveChanges();

            var jwtMock = GetJwtUtilMock();
            var service = new AuthService(context, jwtMock.Object);

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "wrong"
            };

            Assert.Throws<Exception>(() => service.Login(request));

            var updatedUser = context.Users.First();

            Assert.Equal(1, updatedUser.FailedAttempts);
        }

        // -------------------- TEST 4 --------------------
        [Fact]
        public void Login_ValidCredentials_ReturnsToken()
        {
            var context = GetDbContext();

            context.Users.Add(new User
            {
                Username = "testuser",
                Password = BCrypt.Net.BCrypt.HashPassword("123"),
                FailedAttempts = 2,
                IsBlocked = false
            });

            context.SaveChanges();

            var jwtMock = GetJwtUtilMock();
            var service = new AuthService(context, jwtMock.Object);

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "123"
            };

            var token = service.Login(request);

            Assert.Equal("fake-jwt-token", token);
        }
    }
}
