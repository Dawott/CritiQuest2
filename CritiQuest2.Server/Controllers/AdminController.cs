using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CritiQuest2.Server.Data;

namespace CritiQuest2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize] // DO DEMA
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Add gacha tickets to the current user (for testing purposes)
        /// </summary>
        [HttpPost("add-tickets")]
        public async Task<ActionResult> AddGachaTickets([FromBody] AddTicketsRequest request)
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                if (request.Amount <= 0 || request.Amount > 100)
                {
                    return BadRequest("Invalid ticket amount. Must be between 1 and 100.");
                }

                user.Stats.GachaTickets += request.Amount;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added {Amount} gacha tickets to user {UserId}", request.Amount, userId);

                return Ok(new
                {
                    message = $"Added {request.Amount} gacha tickets",
                    totalTickets = user.Stats.GachaTickets
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding gacha tickets");
                return StatusCode(500, "An error occurred while adding tickets");
            }
        }

        /// <summary>
        /// Reset user's philosopher collection (for testing purposes)
        /// </summary>
        [HttpPost("reset-collection")]
        public async Task<ActionResult> ResetPhilosopherCollection()
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var ownedPhilosophers = await _context.OwnedPhilosophers
                    .Where(op => op.UserId == userId)
                    .ToListAsync();

                _context.OwnedPhilosophers.RemoveRange(ownedPhilosophers);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reset philosopher collection for user {UserId}", userId);

                return Ok(new
                {
                    message = $"Removed {ownedPhilosophers.Count} philosophers from collection"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting philosopher collection");
                return StatusCode(500, "An error occurred while resetting collection");
            }
        }

        /// <summary>
        /// Get user stats and collection summary
        /// </summary>
        [HttpGet("user-summary")]
        public async Task<ActionResult> GetUserSummary()
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var ownedCount = await _context.OwnedPhilosophers.CountAsync(op => op.UserId == userId);
                var totalPhilosophers = await _context.Philosophers.CountAsync();

                var rarityBreakdown = await _context.OwnedPhilosophers
                    .Where(op => op.UserId == userId)
                    .Include(op => op.Philosopher)
                    .GroupBy(op => op.Philosopher.Rarity)
                    .Select(g => new { Rarity = g.Key.ToString(), Count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    userId = userId,
                    gachaTickets = user.Stats?.GachaTickets ?? 0,
                    ownedPhilosophers = ownedCount,
                    totalPhilosophers = totalPhilosophers,
                    collectionPercentage = totalPhilosophers > 0 ? (double)ownedCount / totalPhilosophers * 100 : 0,
                    rarityBreakdown = rarityBreakdown
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user summary");
                return StatusCode(500, "An error occurred while getting user summary");
            }
        }
    }

    public class AddTicketsRequest
    {
        public int Amount { get; set; }
    }
}