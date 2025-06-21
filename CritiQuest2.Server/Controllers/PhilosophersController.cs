using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace CritiQuest2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PhilosophersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PhilosophersController> _logger;

        public PhilosophersController(ApplicationDbContext context, ILogger<PhilosophersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all philosophers with user ownership status
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPhilosophers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var philosophers = await _context.Philosophers
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Era,
                        p.School,
                        p.Rarity,
                        BaseStats = new
                        {
                            p.Wisdom,
                            p.Logic,
                            p.Rhetoric,
                            p.Influence,
                            p.Originality
                        },
                        p.Description,
                        p.ImageUrl,
                        Quotes = JsonSerializer.Deserialize<string[]>(p.QuotesJson),
                        SpecialAbility = JsonSerializer.Deserialize<object>(p.SpecialAbilityJson),
                        IsOwned = _context.OwnedPhilosophers
                            .Any(op => op.UserId == userId && op.PhilosopherId == p.Id),
                        UserData = _context.OwnedPhilosophers
                            .Where(op => op.UserId == userId && op.PhilosopherId == p.Id)
                            .Select(op => new
                            {
                                op.Level,
                                op.Experience,
                                op.Duplicates,
                                CurrentStats = new
                                {
                                    op.CurrentWisdom,
                                    op.CurrentLogic,
                                    op.CurrentRhetoric,
                                    op.CurrentInfluence,
                                    op.CurrentOriginality
                                },
                                op.ObtainedAt
                            })
                            .FirstOrDefault()
                    })
                    .OrderBy(p => p.Era)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                return Ok(philosophers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching philosophers for user {UserId}", userId);
                return StatusCode(500, new { message = "Error fetching philosophers" });
            }
        }

        /// <summary>
        /// Get user's philosopher collection
        /// </summary>
        [HttpGet("collection")]
        public async Task<IActionResult> GetUserCollection()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var collection = await _context.OwnedPhilosophers
                    .Include(op => op.Philosopher)
                    .Where(op => op.UserId == userId)
                    .Select(op => new
                    {
                        op.Id,
                        op.Level,
                        op.Experience,
                        op.Duplicates,
                        CurrentStats = new
                        {
                            op.CurrentWisdom,
                            op.CurrentLogic,
                            op.CurrentRhetoric,
                            op.CurrentInfluence,
                            op.CurrentOriginality
                        },
                        op.ObtainedAt,
                        Philosopher = new
                        {
                            op.Philosopher.Id,
                            op.Philosopher.Name,
                            op.Philosopher.Era,
                            op.Philosopher.School,
                            op.Philosopher.Rarity,
                            BaseStats = new
                            {
                                op.Philosopher.Wisdom,
                                op.Philosopher.Logic,
                                op.Philosopher.Rhetoric,
                                op.Philosopher.Influence,
                                op.Philosopher.Originality
                            },
                            op.Philosopher.Description,
                            op.Philosopher.ImageUrl,
                            Quotes = JsonSerializer.Deserialize<string[]>(op.Philosopher.QuotesJson),
                            SpecialAbility = JsonSerializer.Deserialize<object>(op.Philosopher.SpecialAbilityJson)
                        }
                    })
                    .OrderByDescending(op => op.Philosopher.Rarity)
                    .ThenBy(op => op.Philosopher.Era)
                    .ToListAsync();

                return Ok(collection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching philosopher collection for user {UserId}", userId);
                return StatusCode(500, new { message = "Error fetching collection" });
            }
        }

        /// <summary>
        /// Get specific philosopher details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhilosopher(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var philosopher = await _context.Philosophers
                    .Where(p => p.Id == id)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Era,
                        p.School,
                        p.Rarity,
                        BaseStats = new
                        {
                            p.Wisdom,
                            p.Logic,
                            p.Rhetoric,
                            p.Influence,
                            p.Originality
                        },
                        p.Description,
                        p.ImageUrl,
                        Quotes = JsonSerializer.Deserialize<string[]>(p.QuotesJson),
                        SpecialAbility = JsonSerializer.Deserialize<object>(p.SpecialAbilityJson),
                        IsOwned = _context.OwnedPhilosophers
                            .Any(op => op.UserId == userId && op.PhilosopherId == p.Id),
                        UserData = _context.OwnedPhilosophers
                            .Where(op => op.UserId == userId && op.PhilosopherId == p.Id)
                            .Select(op => new
                            {
                                op.Level,
                                op.Experience,
                                op.Duplicates,
                                CurrentStats = new
                                {
                                    op.CurrentWisdom,
                                    op.CurrentLogic,
                                    op.CurrentRhetoric,
                                    op.CurrentInfluence,
                                    op.CurrentOriginality
                                },
                                op.ObtainedAt
                            })
                            .FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();

                if (philosopher == null)
                    return NotFound();

                return Ok(philosopher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching philosopher {PhilosopherId} for user {UserId}", id, userId);
                return StatusCode(500, new { message = "Error fetching philosopher" });
            }
        }

        /// <summary>
        /// Perform gacha summon
        /// </summary>
        [HttpPost("gacha/summon")]
        public async Task<IActionResult> PerformGachaSummon([FromBody] GachaSummonRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                // Check user has enough tickets
                var userStats = await _context.UserStats
                    .FirstOrDefaultAsync(us => us.UserId == userId);

                if (userStats == null || userStats.GachaTickets < request.TicketCount)
                    return BadRequest(new { message = "Insufficient gacha tickets" });

                var results = new List<object>();

                for (int i = 0; i < request.TicketCount; i++)
                {
                    var summonResult = await PerformSingleSummon(userId);
                    results.Add(summonResult);
                }

                // Deduct tickets
                userStats.GachaTickets -= request.TicketCount;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Results = results,
                    RemainingTickets = userStats.GachaTickets
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing gacha summon for user {UserId}", userId);
                return StatusCode(500, new { message = "Error performing summon" });
            }
        }

        private async Task<object> PerformSingleSummon(string userId)
        {
            var random = new Random();

            // Gacha rates (adjust as needed)
            var rarityRates = new Dictionary<Rarity, double>
            {
                { Rarity.Common, 0.50 },      // 50%
                { Rarity.Uncommon, 0.25 },    // 25%
                { Rarity.Rare, 0.15 },        // 15%
                { Rarity.Epic, 0.08 },        // 8%
                { Rarity.Legendary, 0.02 }    // 2%
            };

            // Determine rarity
            var roll = random.NextDouble();
            var cumulativeRate = 0.0;
            Rarity selectedRarity = Rarity.Common;

            foreach (var kvp in rarityRates.OrderByDescending(x => x.Key))
            {
                cumulativeRate += kvp.Value;
                if (roll <= cumulativeRate)
                {
                    selectedRarity = kvp.Key;
                    break;
                }
            }

            // Get random philosopher of selected rarity
            var philosophersOfRarity = await _context.Philosophers
                .Where(p => p.Rarity == selectedRarity)
                .ToListAsync();

            if (!philosophersOfRarity.Any())
            {
                // Fallback to common if no philosophers of selected rarity
                philosophersOfRarity = await _context.Philosophers
                    .Where(p => p.Rarity == Rarity.Common)
                    .ToListAsync();
            }

            var selectedPhilosopher = philosophersOfRarity[random.Next(philosophersOfRarity.Count)];

            // Check if user already owns this philosopher
            var existingOwnership = await _context.OwnedPhilosophers
                .FirstOrDefaultAsync(op => op.UserId == userId && op.PhilosopherId == selectedPhilosopher.Id);

            bool isNewPhilosopher = existingOwnership == null;

            if (isNewPhilosopher)
            {
                // Grant new philosopher
                var ownedPhilosopher = new OwnedPhilosopher
                {
                    UserId = userId,
                    PhilosopherId = selectedPhilosopher.Id,
                    Level = 1,
                    Experience = 0,
                    Duplicates = 0,
                    CurrentWisdom = selectedPhilosopher.Wisdom,
                    CurrentLogic = selectedPhilosopher.Logic,
                    CurrentRhetoric = selectedPhilosopher.Rhetoric,
                    CurrentInfluence = selectedPhilosopher.Influence,
                    CurrentOriginality = selectedPhilosopher.Originality,
                    ObtainedAt = DateTime.UtcNow
                };

                _context.OwnedPhilosophers.Add(ownedPhilosopher);

                // Update user progression
                var userProgression = await _context.UserProgressions
                    .FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProgression != null)
                {
                    var unlockedPhilosophers = JsonSerializer.Deserialize<List<string>>(userProgression.UnlockedPhilosophersJson) ?? new List<string>();
                    if (!unlockedPhilosophers.Contains(selectedPhilosopher.Id))
                    {
                        unlockedPhilosophers.Add(selectedPhilosopher.Id);
                        userProgression.UnlockedPhilosophersJson = JsonSerializer.Serialize(unlockedPhilosophers);
                    }
                }
            }
            else
            {
                // Dodaj duplikat, który dorzuci bonusy
                existingOwnership.Duplicates++;
                existingOwnership.Experience += 50; // Dodatkowe doświadczenie za duplikat
            }

            return new
            {
                Philosopher = new
                {
                    selectedPhilosopher.Id,
                    selectedPhilosopher.Name,
                    selectedPhilosopher.Era,
                    selectedPhilosopher.School,
                    selectedPhilosopher.Rarity,
                    selectedPhilosopher.ImageUrl
                },
                IsNew = isNewPhilosopher,
                IsDuplicate = !isNewPhilosopher
            };
        }
    }

    public class GachaSummonRequest
    {
        public int TicketCount { get; set; } = 1;
    }
}