using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Model.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CritiQuest2.Server.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationReturn> RegisterAsync(RegisterRequest request);
        Task<AuthenticationReturn> LoginAsync(LoginRequest request);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationReturn> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthenticationReturn
                {
                    Success = false,
                    Message = "Email already registered"
                };
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                DisplayName = request.DisplayName,
                JoinedAt = DateTime.UtcNow,
                LastActive = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new AuthenticationReturn
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            var token = await GenerateJwtToken(user);

            return new AuthenticationReturn
            {
                Success = true,
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    DisplayName = user.DisplayName!,
                    JoinedAt = user.JoinedAt
                }
            };
        }

        public async Task<AuthenticationReturn> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthenticationReturn
                {
                    Success = false,
                    Message = "Błędny mail lub hasło"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return new AuthenticationReturn
                {
                    Success = false,
                    Message = "Błędny mail lub hasło"
                };
            }

            // Update last active
            user.LastActive = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtToken(user);

            return new AuthenticationReturn
            {
                Success = true,
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    DisplayName = user.DisplayName!,
                    JoinedAt = user.JoinedAt
                }
            };
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, user.DisplayName ?? user.Email!)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["ExpiryInDays"])),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
