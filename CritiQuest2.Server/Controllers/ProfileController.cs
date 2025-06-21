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
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;

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
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return NotFound();

                // Initialize progression and stats if they don't exist
                await EnsureUserProgressionExists(userId);
                await EnsureUserStatsExists(userId);

                // Refresh user with newly created data
                user = await _context.Users
                    .Include(u => u.Progression)
                    .Include(u => u.Stats)
                    .Include(u => u.PhilosopherCollection)
                    .ThenInclude(pc => pc.Philosopher)
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
                            ap.Achievement.RewardExperience,
                            ap.Achievement.RewardGachaTickets
                        },
                        ap.CurrentValue,
                        ap.TargetValue,
                        ap.Completed,
                        ap.UnlockedAt,
                        ap.Viewed,
                        ap.Progress
                    })
                    .ToListAsync();

                // Get recent lesson progress
                var recentLessons = await _context.LessonProgress
                    .Include(lp => lp.Lesson)
                    .Where(lp => lp.UserId == userId)
                    .OrderByDescending(lp => lp.CompletedAt)
                    .Take(5)
                    .Select(lp => new
                    {
                        lp.LessonId,
                        LessonTitle = lp.Lesson.Title,
                        lp.Score,
                        lp.CompletedAt,
                        lp.TimeSpent
                    })
                    .ToListAsync();

                // Calculate level from experience
                var level = CalculateLevel(user.Progression?.Experience ?? 0);
                var experienceForNextLevel = CalculateExperienceForLevel(level + 1);
                var experienceForCurrentLevel = CalculateExperienceForLevel(level);

                var profile = new
                {
                    User = new
                    {
                        user.Id,
                        user.Email,
                        user.DisplayName,
                        user.AvatarUrl,
                        user.JoinedAt,
                        user.LastActive
                    },
                    Progression = new
                    {
                        Level = level,
                        Experience = user.Progression?.Experience ?? 0,
                        ExperienceForNextLevel = experienceForNextLevel,
                        ExperienceForCurrentLevel = experienceForCurrentLevel,
                        ExperienceToNextLevel = experienceForNextLevel - (user.Progression?.Experience ?? 0),
                        CurrentStage = user.Progression?.CurrentStage ?? "ancient-philosophy",
                        CompletedLessons = JsonSerializer.Deserialize<string[]>(user.Progression?.CompletedLessonsJson ?? "[]"),
                        UnlockedPhilosophers = JsonSerializer.Deserialize<string[]>(user.Progression?.UnlockedPhilosophersJson ?? "[]")
                    },
                    Stats = new
                    {
                        user.Stats?.TotalTimeSpent ?? 0,
                        user.Stats?.StreakDays ?? 0,
                        user.Stats?.LastStreakUpdate ?? DateTime.UtcNow,
                        user.Stats?.QuizzesCompleted ?? 0,
                        user.Stats?.PerfectScores ?? 0,
                        user.Stats?.GachaTickets ?? 0
                    },
                    PhilosopherCollection = user.PhilosopherCollection.Select(pc => new
                    {
                        pc.Id,
                        PhilosopherId = pc.PhilosopherId,
                        PhilosopherName = pc.Philosopher.Name,
                        PhilosopherRarity = pc.Philosopher.Rarity,
                        pc.Level,
                        pc.Experience,
                        pc.Duplicates,
                        pc.ObtainedAt
                    }).ToList(),
                    Achievements = achievementProgress,
                    RecentActivity = recentLessons
                };

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching profile for user {UserId}", userId);
                return StatusCode(500, new { message = "Error fetching profile" });
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
                    .Select(us => new
                    {
                        us.TotalTimeSpent,
                        us.StreakDays,
                        us.LastStreakUpdate,
                        us.QuizzesCompleted,
                        us.PerfectScores,
                        us.GachaTickets
                    })
                    .FirstOrDefaultAsync();

                // Get additional calculated stats
                var completedLessonsCount = await _context.LessonProgress
                    .CountAsync(lp => lp.UserId == userId);

                var philosopherCount = await _context.OwnedPhilosophers
                    .CountAsync(op => op.UserId == userId);

                var completedAchievements = await _context.AchievementProgress
                    .CountAsync(ap => ap.UserId == userId && ap.Completed);

                var averageQuizScore = await _context.QuizAttempts
                    .Where(qa => qa.UserId == userId && qa.CompletedAt != null)
                    .AverageAsync(qa => (double?)qa.Score) ?? 0;

                return Ok(new
                {
                    stats?.TotalTimeSpent ?? 0,
                    stats?.StreakDays ?? 0,
                    stats?.LastStreakUpdate ?? DateTime.UtcNow,
                    stats?.QuizzesCompleted ?? 0,
                    stats?.PerfectScores ?? 0,
                    stats?.GachaTickets ?? 0,
                    CompletedLessons = completedLessonsCount,
                    PhilosopherCount = philosopherCount,
                    CompletedAchievements = completedAchievements,
                    AverageQuizScore = Math.Round(averageQuizScore, 1)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching stats for user {UserId}", userId);
                return StatusCode(500, new { message = "Error fetching stats" });
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
                return StatusCode(500, new { message = "Error updating profile" });
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

                var userStats = await _context.UserStats
                    .FirstOrDefaultAsync(us => us.UserId == userId);

                if (userStats != null)
                {
                    var today = DateTime.UtcNow.Date;
                    var lastUpdate = userStats.LastStreakUpdate.Date;

                    if (lastUpdate < today)
                    {
                        if (lastUpdate == today.AddDays(-1))
                        {
                            // Consecutive day - increment streak
                            userStats.StreakDays++;
                        }
                        else if (lastUpdate < today.AddDays(-1))
                        {
                            // Missed days - reset streak
                            userStats.StreakDays = 1;
                        }

                        userStats.LastStreakUpdate = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new { streak = userStats?.StreakDays ?? 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating streak for user {UserId}", userId);
                return StatusCode(500, new { message = "Error updating streak" });
            }
        }

        private async Task EnsureUserProgressionExists(string userId)
        {
            var existingProgression = await _context.UserProgressions
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (existingProgression == null)
            {
                var progression = new UserProgression
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
            var existingStats = await _context.UserStats
                .FirstOrDefaultAsync(us => us.UserId == userId);

            if (existingStats == null)
            {
                var stats = new UserStats
                {
                    UserId = userId,
                    TotalTimeSpent = 0,
                    StreakDays = 0,
                    LastStreakUpdate = DateTime.UtcNow,
                    QuizzesCompleted = 0,
                    PerfectScores = 0,
                    GachaTickets = 3 // Starting tickets
                };

                _context.UserStats.Add(stats);
                await _context.SaveChangesAsync();
            }
        }

        private int CalculateLevel(int experience)
        {
            // Simple leveling formula: Level = floor(sqrt(experience / 100)) + 1
            return (int)Math.Floor(Math.Sqrt(experience / 100.0)) + 1;
        }

        private int CalculateExperienceForLevel(int level)
        {
            // Experience needed for level: (level - 1)^2 * 100
            return (level - 1) * (level - 1) * 100;
        }
    }

    public class UpdateProfileRequest
    {
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}