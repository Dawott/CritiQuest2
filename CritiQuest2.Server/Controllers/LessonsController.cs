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
    public class LessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LessonsController> _logger;

        public LessonsController(ApplicationDbContext context, ILogger<LessonsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all lessons with user progress
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLessons([FromQuery] string? stage = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var query = _context.Lessons.AsQueryable();

                if (!string.IsNullOrEmpty(stage))
                {
                    query = query.Where(l => l.Stage == stage);
                }

                var lessons = await query
                    .OrderBy(l => l.Order)
                    .Select(l => new
                    {
                        l.Id,
                        l.Title,
                        l.Description,
                        l.Stage,
                        l.Order,
                        l.Difficulty,
                        l.EstimatedTime,
                        PhilosophicalConcepts = JsonSerializer.Deserialize<string[]>(l.PhilosophicalConceptsJson),
                        l.RequiredPhilosopher,
                        l.QuizId,
                        Rewards = new
                        {
                            Xp = l.RewardXp,
                            Coins = l.RewardCoins,
                            UnlockedContent = JsonSerializer.Deserialize<string[]>(l.RewardContentJson)
                        },
                        UserProgress = _context.LessonProgress
                            .Where(lp => lp.UserId == userId && lp.LessonId == l.Id)
                            .Select(lp => new
                            {
                                lp.CompletedAt,
                                lp.Score,
                                lp.TimeSpent,
                                lp.Attempts,
                                lp.BestScore
                            })
                            .FirstOrDefault(),
                        IsCompleted = _context.LessonProgress
                            .Any(lp => lp.UserId == userId && lp.LessonId == l.Id)
                    })
                    .ToListAsync();

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lessons for user {UserId}", userId);
                return StatusCode(500, new { message = "Error fetching lessons" });
            }
        }

        /// <summary>
        /// Get lesson details with content
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLesson(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var lesson = await _context.Lessons
                    .Where(l => l.Id == id)
                    .Select(l => new
                    {
                        l.Id,
                        l.Title,
                        l.Description,
                        l.Stage,
                        l.Order,
                        l.Difficulty,
                        l.EstimatedTime,
                        PhilosophicalConcepts = JsonSerializer.Deserialize<string[]>(l.PhilosophicalConceptsJson),
                        l.RequiredPhilosopher,
                        Content = JsonSerializer.Deserialize<object>(l.ContentJson),
                        l.QuizId,
                        Rewards = new
                        {
                            Xp = l.RewardXp,
                            Coins = l.RewardCoins,
                            UnlockedContent = JsonSerializer.Deserialize<string[]>(l.RewardContentJson)
                        },
                        UserProgress = _context.LessonProgress
                            .Where(lp => lp.UserId == userId && lp.LessonId == l.Id)
                            .Select(lp => new
                            {
                                lp.CompletedAt,
                                lp.Score,
                                lp.TimeSpent,
                                lp.Attempts,
                                lp.BestScore,
                                lp.Notes
                            })
                            .FirstOrDefault(),
                        IsCompleted = _context.LessonProgress
                            .Any(lp => lp.UserId == userId && lp.LessonId == l.Id)
                    })
                    .FirstOrDefaultAsync();

                if (lesson == null)
                    return NotFound();

                return Ok(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lesson {LessonId} for user {UserId}", id, userId);
                return StatusCode(500, new { message = "Error fetching lesson" });
            }
        }

        /// <summary>
        /// Complete a lesson and update progress
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteLesson(string id, [FromBody] LessonCompletionRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var lesson = await _context.Lessons.FindAsync(id);
                if (lesson == null)
                    return NotFound();

                // Check if user already has progress for this lesson
                var existingProgress = await _context.LessonProgress
                    .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == id);

                if (existingProgress != null)
                {
                    // Update existing progress
                    existingProgress.CompletedAt = DateTime.UtcNow;
                    existingProgress.Score = request.Score;
                    existingProgress.TimeSpent = request.TimeSpent;
                    existingProgress.Notes = request.Notes;
                    existingProgress.Attempts++;
                    existingProgress.BestScore = Math.Max(existingProgress.BestScore, request.Score);
                }
                else
                {
                    // Create new progress
                    var progress = new LessonProgress
                    {
                        UserId = userId,
                        LessonId = id,
                        CompletedAt = DateTime.UtcNow,
                        Score = request.Score,
                        TimeSpent = request.TimeSpent,
                        Notes = request.Notes,
                        Attempts = 1,
                        BestScore = request.Score
                    };
                    _context.LessonProgress.Add(progress);

                    // Update user progression
                    var userProgression = await _context.UserProgressions
                        .FirstOrDefaultAsync(up => up.UserId == userId);

                    if (userProgression != null)
                    {
                        var completedLessons = JsonSerializer.Deserialize<List<string>>(userProgression.CompletedLessonsJson) ?? new List<string>();
                        if (!completedLessons.Contains(id))
                        {
                            completedLessons.Add(id);
                            userProgression.CompletedLessonsJson = JsonSerializer.Serialize(completedLessons);
                            userProgression.Experience += lesson.RewardXp;
                        }
                    }

                    // Update user stats
                    var userStats = await _context.UserStats
                        .FirstOrDefaultAsync(us => us.UserId == userId);

                    if (userStats != null)
                    {
                        userStats.TotalTimeSpent += request.TimeSpent;
                        userStats.GachaTickets += lesson.RewardCoins; // Assuming coins = tickets for now
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Lesson completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing lesson {LessonId} for user {UserId}", id, userId);
                return StatusCode(500, new { message = "Error completing lesson" });
            }
        }
    }

    public class LessonCompletionRequest
    {
        public int Score { get; set; }
        public int TimeSpent { get; set; } // in minutes
        public string? Notes { get; set; }
    }
}