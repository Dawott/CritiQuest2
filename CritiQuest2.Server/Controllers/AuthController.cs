using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CritiQuest2.Server.Model.DTOs;
using CritiQuest2.Server.Services;
using System.Security.Claims;

namespace CritiQuest2.Server.Controllers
{
  
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly IAuthenticationService _authService;

            public AuthController(IAuthenticationService authService)
            {
                _authService = authService;
            }

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] RegisterRequest request)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    message = "Zarejestrowano",
                    token = result.Token,
                    user = result.User
                });
            }

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] LoginRequest request)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    message = "Zalogowano",
                    token = result.Token,
                    user = result.User
                });
            }

            [HttpGet("me")]
            [Authorize]
            public async Task<IActionResult> GetCurrentUser()
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    DisplayName = user.DisplayName!,
                    JoinedAt = user.JoinedAt
                });
            }

            [HttpPost("logout")]
            [Authorize]
            public IActionResult Logout()
            {
                return Ok(new { message = "Wylogowano!" });
            }
        }
    }
