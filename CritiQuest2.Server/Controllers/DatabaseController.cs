using CritiQuest2.Server.Data;
using CritiQuest2.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CritiQuest2.Server.Controllers
{
   
        [ApiController]
        [Route("api/[controller]")]
        public class DatabaseController : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            private readonly IDatabaseSeedingService _seedingService;
            private readonly ILogger<DatabaseController> _logger;
            private readonly IWebHostEnvironment _environment;

            public DatabaseController(
                ApplicationDbContext context,
                IDatabaseSeedingService seedingService,
                ILogger<DatabaseController> logger,
                IWebHostEnvironment environment)
            {
                _context = context;
                _seedingService = seedingService;
                _logger = logger;
                _environment = environment;
            }

            /// <summary>
            /// Pobiesz status danych i seedu
            /// </summary>
            [HttpGet("status")]
            public async Task<IActionResult> GetDatabaseStatus()
            {
                try
                {
                    var canConnect = await _context.Database.CanConnectAsync();

                    if (!canConnect)
                    {
                        return Ok(new
                        {
                            connected = false,
                            message = "Nie moge polaczyc z baza"
                        });
                    }

                    var philosopherCount = await _context.Philosophers.CountAsync();
                    var achievementCount = await _context.Achievements.CountAsync();
                    var lessonCount = await _context.Lessons.CountAsync();
                    var quizCount = await _context.Quizzes.CountAsync();
                    var questionCount = await _context.Questions.CountAsync();
                    var debateArgumentCount = await _context.DebateArguments.CountAsync();
                    var userCount = await _context.Users.CountAsync();

                    return Ok(new
                    {
                        connected = true,
                        environment = _environment.EnvironmentName,
                        counts = new
                        {
                            philosophers = philosopherCount,
                            achievements = achievementCount,
                            lessons = lessonCount,
                            quizzes = quizCount,
                            questions = questionCount,
                            debateArguments = debateArgumentCount,
                            users = userCount
                        },
                        seeded = new
                        {
                            philosophers = philosopherCount > 0,
                            achievements = achievementCount > 0,
                            lessons = lessonCount > 0,
                            quizzes = quizCount > 0,
                            questions = questionCount > 0,
                            debateArguments = debateArgumentCount > 0
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Blad sprawdzania statusu bazy");
                    return StatusCode(500, new { message = "Blad sprawdzania statusu bazy", error = ex.Message });
                }
            }

            /// <summary>
            /// Manualny trigger seeda (na dev)
            /// </summary>
            [HttpPost("seed")]
            public async Task<IActionResult> SeedDatabase()
            {
                if (!_environment.IsDevelopment())
                {
                    return Forbid("Seeding dostępny jest tylko na środowisku dev");
                }

                try
                {
                    _logger.LogInformation("Rozpoczęto manualny seed");
                    await _seedingService.SeedDatabaseAsync();

                    return Ok(new
                    {
                        message = "Seed bazy zakonczono z sukcesem",
                        timestamp = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Blad podczas manualnego seedu");
                    return StatusCode(500, new
                    {
                        message = "Blad podczas manualnego seedu",
                        error = ex.Message
                    });
                }
            }

            /// <summary>
            /// Seed specific entity types (Development only)
            /// </summary>
            [HttpPost("seed/{entityType}")]
            public async Task<IActionResult> SeedSpecificEntity(string entityType)
            {
                if (!_environment.IsDevelopment())
                {
                    return Forbid("Seeding zezwolony jest tylko na dev");
                }

                try
                {
                    switch (entityType.ToLower())
                    {
                        case "philosophers":
                            await _seedingService.SeedPhilosophersAsync();
                            break;
                        case "achievements":
                            await _seedingService.SeedAchievementsAsync();
                            break;
                        case "lessons":
                            await _seedingService.SeedLessonsAsync();
                            break;
                        case "quizzes":
                            await _seedingService.SeedQuizzesAsync();
                            break;
                        case "debates":
                            await _seedingService.SeedDebateArgumentsAsync();
                            break;
                        default:
                            return BadRequest(new { message = $"Nieznany typ jednostki: {entityType}" });
                    }

                    return Ok(new
                    {
                        message = $"{entityType} seed zakonczony",
                        timestamp = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Blad seed {EntityType}", entityType);
                    return StatusCode(500, new
                    {
                        message = $"Blad Seed {entityType}",
                        error = ex.Message
                    });
                }
            }

            /// <summary>
            /// Reset database (Development)
            /// </summary>
            [HttpPost("reset")]
            public async Task<IActionResult> ResetDatabase()
            {
                if (!_environment.IsDevelopment())
                {
                    return Forbid("Resetowac baze mozna tylko na developmencie");
                }

                try
                {
                    _logger.LogWarning("Request resetu bazy - dane zostana utracone!");

                    await _context.Database.EnsureDeletedAsync();
                    await _context.Database.EnsureCreatedAsync();
                    await _seedingService.SeedDatabaseAsync();

                    return Ok(new
                    {
                        message = "Reset zakonczony",
                        warning = "Poprzednie dane utracone",
                        timestamp = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during database reset");
                    return StatusCode(500, new
                    {
                        message = "Error during database reset",
                        error = ex.Message
                    });
                }
            }

            /// <summary>
            /// Get sample philosophers for testing
            /// </summary>
            [HttpGet("sample/philosophers")]
            public async Task<IActionResult> GetSamplePhilosophers()
            {
                try
                {
                    var philosophers = await _context.Philosophers
                        .Take(5)
                        .Select(p => new
                        {
                            p.Id,
                            p.Name,
                            p.Era,
                            p.School,
                            p.Rarity,
                            Stats = new
                            {
                                p.Wisdom,
                                p.Logic,
                                p.Rhetoric,
                                p.Influence,
                                p.Originality
                            }
                        })
                        .ToListAsync();

                    return Ok(philosophers);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Blad pobierania sampli filozofow");
                    return StatusCode(500, new { message = "Error retrieving philosophers", error = ex.Message });
                }
            }
        }
    }

