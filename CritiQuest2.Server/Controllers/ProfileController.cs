using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Services;
using System.Security.Claims;
using System.Text.Json;

namespace CritiQuest2.Server.Controllers
{
    public class UpdateProfileRequest
    {
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        private readonly IProgressionService _progressionService;

        public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get user profile with complete progression data
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var user = await _context.Users
                    .Include(u => u.Progression)
                    .Include(u => u.Stats)
                    .Include(u => u.PhilosopherCollection)
                        .ThenInclude(pc => pc.Philosopher)
                    .Include(u => u.LessonProgress)
                        .ThenInclude(lp => lp.Lesson)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return NotFound();

                // Initialize progression and stats if they don't exist
                await EnsureUserProgressionExists(userId);
                await EnsureUserStatsExists(userId);

                // Refresh user data after potential creation
                user = await _context.Users
                    .Include(u => u.Progression)
                    .Include(u => u.Stats)
                    .Include(u => u.PhilosopherCollection)
                        .ThenInclude(pc => pc.Philosopher)
                    .Include(u => u.LessonProgress)
                        .ThenInclude(lp => lp.Lesson)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                // Get achievement progress
                var achievementProgress = await _context.AchievementProgress
                    .Include(ap => ap.Achievement)
                    .Where(ap => ap.UserId == userId)
                    .Select(ap => new
                    {
                        ap.AchievementId,
                        Achievement = new
                        {
                            ap.Achievement.Name,
                            ap.Achievement.Description,
                            RewardExperience = ap.Achievement.RewardExperience,
                            RewardGachaTickets = ap.Achievement.RewardGachaTickets
                        },
                        ap.CurrentValue,
                        ap.TargetValue,
                        ap.Completed,
                        ap.UnlockedAt,
                        ap.Viewed,
                        Progress = ap.TargetValue > 0 ? (double)ap.CurrentValue / ap.TargetValue * 100 : 0
                    })
                    .ToListAsync();

                // Get recent lesson activity
                var recentLessons = await _context.LessonProgress
                    .Include(lp => lp.Lesson)
                    .Where(lp => lp.UserId == userId && lp.CompletedAt != null)
                    .OrderByDescending(lp => lp.CompletedAt)
                    .Take(5)
                    .Select(lp => new
                    {
                        LessonId = lp.LessonId,
                        LessonTitle = lp.Lesson.Title,
                        Score = lp.BestScore,
                        CompletedAt = lp.CompletedAt!,
                        TimeSpent = lp.TimeSpent
                    })
                    .ToListAsync();

                // Calculate experience for next level (simple formula)
                var currentLevel = user.Progression?.Level ?? 1;
                var currentExp = user.Progression?.Experience ?? 0;
                var expForCurrentLevel = CalculateExperienceForLevel(currentLevel - 1);
                var expForNextLevel = CalculateExperienceForLevel(currentLevel);

                var profile = new
                {
                    User = new
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        DisplayName = user.DisplayName ?? user.Email!,
                        AvatarUrl = user.AvatarUrl,
                        JoinedAt = user.JoinedAt,
                        LastActive = user.LastActive
                    },
                    Progression = new
                    {
                        Level = user.Progression?.Level ?? 1,
                        Experience = user.Progression?.Experience ?? 0,
                        ExperienceForNextLevel = expForNextLevel,
                        ExperienceForCurrentLevel = expForCurrentLevel,
                        ExperienceToNextLevel = Math.Max(0, expForNextLevel - currentExp),
                        CurrentStage = user.Progression?.CurrentStage ?? "ancient-philosophy",
                        CompletedLessons = DeserializeStringArray(user.Progression?.CompletedLessonsJson ?? "[]"),
                        UnlockedPhilosophers = DeserializeStringArray(user.Progression?.UnlockedPhilosophersJson ?? "[]")
                    },
                    Stats = new
                    {
                        TotalTimeSpent = user.Stats?.TotalTimeSpent ?? 0,
                        StreakDays = user.Stats?.StreakDays ?? 0,
                        LastStreakUpdate = user.Stats?.LastStreakUpdate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        QuizzesCompleted = user.Stats?.QuizzesCompleted ?? 0,
                        PerfectScores = user.Stats?.PerfectScores ?? 0,
                        GachaTickets = user.Stats?.GachaTickets ?? 3 // Start with some tickets
                    },
                    PhilosopherCollection = user.PhilosopherCollection.Select(pc => new
                    {
                        Id = pc.Id,
                        PhilosopherId = pc.PhilosopherId,
                        PhilosopherName = pc.Philosopher.Name,
                        PhilosopherRarity = pc.Philosopher.Rarity,
                        Level = pc.Level,
                        Experience = pc.Experience,
                        Duplicates = pc.Duplicates,
                        ObtainedAt = pc.ObtainedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    }).ToList(),
                    Achievements = achievementProgress,
                    RecentActivity = recentLessons
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching profile for user {UserId}", userId);
                return StatusCode(500, new { message = "Error fetching profile", error = ex.Message });
            }
        }

        [HttpGet("progression-summary")]
        public async Task<ActionResult> GetProgressionSummary()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var summary = await _progressionService.GetProgressionSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting progression summary for user");
                return StatusCode(500, new { message = "Error retrieving progression summary" });
            }
        }

        [HttpPost("recalculate-level")]
        public async Task<ActionResult> RecalculateLevel()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var userProgression = await _progressionService.RecalculateUserLevelAsync(userId);

                return Ok(new
                {
                    message = "Level recalculated successfully",
                    currentLevel = userProgression.Level,
                    currentExperience = userProgression.Experience
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating level for user");
                return StatusCode(500, new { message = "Error recalculating level" });
            }
        }

        /// <summary>
        /// Get user stats summary
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await EnsureUserStatsExists(userId);

                var stats = await _context.UserStats
                    .Where(us => us.UserId == userId)
                    .FirstOrDefaultAsync();

                // Get additional calculated stats
                var completedLessonsCount = await _context.LessonProgress
                    .CountAsync(lp => lp.UserId == userId && lp.CompletedAt != null);

                var philosopherCount = await _context.OwnedPhilosophers
                    .CountAsync(op => op.UserId == userId);

                var completedAchievements = await _context.AchievementProgress
                    .CountAsync(ap => ap.UserId == userId && ap.Completed);

                var averageQuizScore = await _context.QuizAttempts
                    .Where(qa => qa.UserId == userId && qa.CompletedAt != null)
                    .AverageAsync(qa => (double?)qa.Score) ?? 0;

                return Ok(new
                {
                    TotalTimeSpent = stats?.TotalTimeSpent ?? 0,
                    StreakDays = stats?.StreakDays ?? 0,
                    LastStreakUpdate = (stats?.LastStreakUpdate ?? DateTime.UtcNow).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    QuizzesCompleted = stats?.QuizzesCompleted ?? 0,
                    PerfectScores = stats?.PerfectScores ?? 0,
                    GachaTickets = stats?.GachaTickets ?? 3,
                    CompletedLessons = completedLessonsCount,
                    PhilosopherCount = philosopherCount,
                    CompletedAchievements = completedAchievements,
                    AverageQuizScore = Math.Round(averageQuizScore, 1)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stats for user {UserId}", userId);
                return StatusCode(500, new { message = "Error fetching stats", error = ex.Message });
            }
        }

        /// <summary>
        /// Update user profile information
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return NotFound();

                if (!string.IsNullOrEmpty(request.DisplayName))
                    user.DisplayName = request.DisplayName;

                if (!string.IsNullOrEmpty(request.AvatarUrl))
                    user.AvatarUrl = request.AvatarUrl;

                user.LastActive = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return StatusCode(500, new { message = "Error updating profile", error = ex.Message });
            }
        }

        /// <summary>
        /// Update daily streak
        /// </summary>
        [HttpPost("streak")]
        public async Task<IActionResult> UpdateStreak()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                await EnsureUserStatsExists(userId);

                var stats = await _context.UserStats
                    .FirstAsync(us => us.UserId == userId);

                var today = DateTime.UtcNow.Date;
                var lastUpdate = stats.LastStreakUpdate.Date;

                if (lastUpdate == today)
                {
                    // Already updated today
                    return Ok(new { streak = stats.StreakDays });
                }
                else if (lastUpdate == today.AddDays(-1))
                {
                    // Consecutive day - increment streak
                    stats.StreakDays++;
                }
                else
                {
                    // Broken streak - reset to 1
                    stats.StreakDays = 1;
                }

                stats.LastStreakUpdate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { streak = stats.StreakDays });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating streak for user {UserId}", userId);
                return StatusCode(500, new { message = "Error updating streak", error = ex.Message });
            }
        }

        #region Helper Methods

        private async Task EnsureUserProgressionExists(string userId)
        {
            var progression = await _context.UserProgressions
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (progression == null)
            {
                progression = new UserProgression
                {
                    UserId = userId,
                    Level = 1,
                    Experience = 0,
                    CurrentStage = "ancient-philosophy",
                    CompletedLessonsJson = "[]",
                    UnlockedPhilosophersJson = "[]"
                };

                _context.UserProgressions.Add(progression);
                await _context.SaveChangesAsync();
            }
        }

        private async Task EnsureUserStatsExists(string userId)
        {
            var stats = await _context.UserStats
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (stats == null)
            {
                stats = new UserStats
                {
                    UserId = userId,
                    TotalTimeSpent = 0,
                    StreakDays = 0,
                    LastStreakUpdate = DateTime.UtcNow,
                    QuizzesCompleted = 0,
                    PerfectScores = 0,
                    GachaTickets = 3 // Start with some tickets
                };

                _context.UserStats.Add(stats);
                await _context.SaveChangesAsync();
            }
        }

        private static int CalculateExperienceForLevel(int level)
        {
            // Simple exponential formula: level^2 * 100
            return level * level * 100;
        }

        private static string[] DeserializeStringArray(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        #endregion
    }
}