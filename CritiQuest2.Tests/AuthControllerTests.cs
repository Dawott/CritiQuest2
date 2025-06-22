using CritiQuest2.Server.Controllers;
using CritiQuest2.Server.Model.DTOs;
using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace CritiQuest2.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthenticationService> _mockAuthService;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Test]
        public async Task Register_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                DisplayName = "Test User",
                Password = "TestPassword123"
            };

            var expectedResult = new AuthenticationReturn
            {
                Success = true,
                Token = "test-jwt-token",
                User = new UserDto
                {
                    Id = "user-id",
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                    JoinedAt = DateTime.UtcNow
                }
            };

            _mockAuthService.Setup(x => x.RegisterAsync(request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            dynamic responseData = okResult.Value;
            Assert.That(responseData.message, Is.EqualTo("Zarejestrowano"));
            Assert.That(responseData.token, Is.EqualTo("test-jwt-token"));
            Assert.That(responseData.user, Is.Not.Null);
        }

        [Test]
        public async Task Register_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "invalid-email", // Invalid email format
                DisplayName = "",
                Password = "123"
            };

            // Simulate ModelState validation failure
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Register_WithServiceFailure_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "existing@example.com",
                DisplayName = "Test User",
                Password = "TestPassword123"
            };

            var expectedResult = new AuthenticationReturn
            {
                Success = false,
                Message = "Email already registered"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            dynamic responseData = badRequestResult.Value;
            Assert.That(responseData.message, Is.EqualTo("Email already registered"));
        }

        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "TestPassword123"
            };

            var expectedResult = new AuthenticationReturn
            {
                Success = true,
                Token = "test-jwt-token",
                User = new UserDto
                {
                    Id = "user-id",
                    Email = request.Email,
                    DisplayName = "Test User",
                    JoinedAt = DateTime.UtcNow
                }
            };

            _mockAuthService.Setup(x => x.LoginAsync(request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            dynamic responseData = okResult.Value;
            Assert.That(responseData.message, Is.EqualTo("Zalogowano"));
            Assert.That(responseData.token, Is.EqualTo("test-jwt-token"));
            Assert.That(responseData.user, Is.Not.Null);
        }

        [Test]
        public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var expectedResult = new AuthenticationReturn
            {
                Success = false,
                Message = "Invalid email or password"
            };

            _mockAuthService.Setup(x => x.LoginAsync(request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

            dynamic responseData = badRequestResult.Value;
            Assert.That(responseData.message, Is.EqualTo("Invalid email or password"));
        }

        [Test]
        public async Task GetCurrentUser_WithValidUser_ShouldReturnOk()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "test@example.com",
                DisplayName = "Test User",
                JoinedAt = DateTime.UtcNow
            };

            SetupControllerContext(userId);

            _mockAuthService.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            var userData = okResult.Value as UserDto;
            Assert.That(userData, Is.Not.Null);
            Assert.That(userData.Id, Is.EqualTo(userId));
            Assert.That(userData.Email, Is.EqualTo("test@example.com"));
            Assert.That(userData.DisplayName, Is.EqualTo("Test User"));
        }

        [Test]
        public async Task GetCurrentUser_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Arrange - No user context set up

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task GetCurrentUser_WithNonExistentUser_ShouldReturnNotFound()
        {
            // Arrange
            var userId = "non-existent-user";
            SetupControllerContext(userId);

            _mockAuthService.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void Logout_ShouldReturnOk()
        {
            // Arrange
            SetupControllerContext("test-user-id");

            // Act
            var result = _controller.Logout();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            dynamic responseData = okResult.Value;
            Assert.That(responseData.message, Is.EqualTo("Wylogowano!"));
        }

        private void SetupControllerContext(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
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
    }
}
