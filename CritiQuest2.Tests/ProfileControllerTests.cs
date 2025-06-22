using CritiQuest2.Server.Controllers;
using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace CritiQuest2.Tests.Controllers
{
    [TestFixture]
    public class ProfileControllerTests
    {
        private ApplicationDbContext _context;
        private ProfileController _controller;
        private Mock<ILogger<ProfileController>> _mockLogger;
        private string _testUserId;

        [SetUp]
        public void Setup()
        {
            // Create in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<ProfileController>>();
            _controller = new ProfileController(_context, _mockLogger.Object);

            // Setup test user
            _testUserId = "test-user-id";
            SetupControllerContext();
            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SetupControllerContext()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
                new Claim(ClaimTypes.Email, "test@example.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        private void SeedTestData()
        {
            var user = new ApplicationUser
            {
                Id = _testUserId,
                Email = "test@example.com",
                DisplayName = "Test User",
                JoinedAt = DateTime.UtcNow.AddDays(-30),
                LastActive = DateTime.UtcNow
            };

            var progression = new UserProgression
            {
                UserId = _testUserId,
                Level = 5,
                Experience = 1250,
                CurrentStage = "Ethics",
                CompletedLessonsJson = "[\"lesson1\", \"lesson2\"]",
                UnlockedPhilosophersJson = "[\"plato\", \"aristotle\"]"
            };

            var stats = new UserStats
            {
                UserId = _testUserId,
                TotalTimeSpent = 3600, // 1 hour
                StreakDays = 7,
                LastStreakUpdate = DateTime.UtcNow,
                QuizzesCompleted = 10,
                PerfectScores = 3,
                GachaTickets = 5
            };

            _context.Users.Add(user);
            _context.UserProgressions.Add(progression);
            _context.UserStats.Add(stats);
            _context.SaveChanges();
        }

        [Test]
        public async Task GetProfile_WithValidUser_ShouldReturnProfile()
        {
            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verify the response contains expected data
            var responseData = okResult.Value;
            Assert.That(responseData, Is.Not.Null);

            // You can add more specific assertions here based on the actual response structure
        }

        [Test]
        public async Task GetProfile_WithUnauthorizedUser_ShouldReturnUnauthorized()
        {
            // Arrange - Clear the user context
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.GetProfile();

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task GetStats_WithValidUser_ShouldReturnStats()
        {
            // Act
            var result = await _controller.GetStats();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verify the response structure
            dynamic statsData = okResult.Value;
            Assert.That(statsData, Is.Not.Null);
        }

        [Test]
        public async Task GetStats_WithUnauthorizedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.GetStats();

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task UpdateProfile_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var updateRequest = new UpdateProfileRequest
            {
                DisplayName = "Updated Test User",
                AvatarUrl = "https://example.com/avatar.jpg"
            };

            // Act
            var result = await _controller.UpdateProfile(updateRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            // Verify the user was actually updated in the database
            var updatedUser = await _context.Users.FindAsync(_testUserId);
            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser.DisplayName, Is.EqualTo("Updated Test User"));
            Assert.That(updatedUser.AvatarUrl, Is.EqualTo("https://example.com/avatar.jpg"));
        }

        [Test]
        public async Task UpdateProfile_WithEmptyDisplayName_ShouldNotUpdateDisplayName()
        {
            // Arrange
            var originalDisplayName = "Test User";
            var updateRequest = new UpdateProfileRequest
            {
                DisplayName = "", // Empty string
                AvatarUrl = "https://example.com/avatar.jpg"
            };

            // Act
            var result = await _controller.UpdateProfile(updateRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            // Verify the display name wasn't changed
            var updatedUser = await _context.Users.FindAsync(_testUserId);
            Assert.That(updatedUser.DisplayName, Is.EqualTo(originalDisplayName));
            Assert.That(updatedUser.AvatarUrl, Is.EqualTo("https://example.com/avatar.jpg"));
        }

        [Test]
        public async Task UpdateProfile_WithUnauthorizedUser_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();
            var updateRequest = new UpdateProfileRequest
            {
                DisplayName = "Updated Name"
            };

            // Act
            var result = await _controller.UpdateProfile(updateRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task UpdateProfile_WithNonExistentUser_ShouldReturnNotFound()
        {
            // Arrange - Setup controller with non-existent user ID
            var nonExistentUserId = "non-existent-user";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nonExistentUserId)
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = claimsPrincipal;

            var updateRequest = new UpdateProfileRequest
            {
                DisplayName = "Updated Name"
            };

            // Act
            var result = await _controller.UpdateProfile(updateRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
