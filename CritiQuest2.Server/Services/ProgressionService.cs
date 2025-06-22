using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Model.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CritiQuest2.Server.Services
{
    public interface IProgressionService
    {
        Task<ProgressionResult> AddExperienceAsync(string userId, int experience, string source, object? metadata = null);
        Task<List<Achievement>> CheckAchievementsAsync(string userId);
        Task<bool> UpdateAchievementProgressAsync(string userId, string achievementType, int value = 1);
        Task<UserProgression> RecalculateUserLevelAsync(string userId);
        Task<ProgressionSummary> GetProgressionSummaryAsync(string userId);
    }

    public class ProgressionService : IProgressionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProgressionService> _logger;

        // Level rewards - you can adjust these
        private readonly Dictionary<int, LevelRewards> _levelRewards = new()
        {
            { 2, new LevelRewards { GachaTickets = 2, Experience = 0 } },
            { 3, new LevelRewards { GachaTickets = 3, Experience = 0 } },
            { 4, new LevelRewards { GachaTickets = 4, Experience = 0 } },
            { 5, new LevelRewards { GachaTickets = 5, Experience = 50 } },
            { 10, new LevelRewards { GachaTickets = 10, Experience = 100 } },
            { 15, new LevelRewards { GachaTickets = 15, Experience = 150 } },
            { 20, new LevelRewards { GachaTickets = 20, Experience = 200 } }
        };

        public ProgressionService(ApplicationDbContext context, ILogger<ProgressionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProgressionResult> AddExperienceAsync(string userId, int experience, string source, object? metadata = null)
        {
            var result = new ProgressionResult();

            try
            {
                // Get user progression
                var userProgression = await _context.UserProgressions
                    .FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProgression == null)
                {
                    userProgression = new UserProgression
                    {
                        UserId = userId,
                        Level = 1,
                        Experience = 0,
                        CurrentStage = "ancient-philosophy",
                        CompletedLessonsJson = "[]",
                        UnlockedPhilosophersJson = "[]"
                    };
                    _context.UserProgressions.Add(userProgression);
                }

                var oldLevel = userProgression.Level;
                var oldExperience = userProgression.Experience;

                // Add experience
                userProgression.Experience += experience;

                // Check for level up
                var newLevel = CalculateLevelFromExperience(userProgression.Experience);
                var leveledUp = newLevel > oldLevel;

                if (leveledUp)
                {
                    userProgression.Level = newLevel;
                    _logger.LogInformation("User {UserId} leveled up from {OldLevel} to {NewLevel}", userId, oldLevel, newLevel);

                    // Add level up rewards
                    await GrantLevelRewardsAsync(userId, oldLevel + 1, newLevel);

                    result.LeveledUp = true;
                    result.NewLevel = newLevel;
                    result.LevelRewards = GetLevelRewardsBetween(oldLevel + 1, newLevel);
                }

                result.ExperienceGained = experience;
                result.TotalExperience = userProgression.Experience;
                result.CurrentLevel = userProgression.Level;
                result.Source = source;

                // Log progression event
                await LogProgressionEventAsync(userId, source, experience, newLevel, leveledUp, metadata);

                // Check for achievements
                result.NewAchievements = await CheckAchievementsAsync(userId);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Added {Experience} XP to user {UserId} from {Source}", experience, userId, source);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding experience to user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<Achievement>> CheckAchievementsAsync(string userId)
        {
            var newAchievements = new List<Achievement>();

            try
            {
                // Get all achievements and user progress
                var allAchievements = await _context.Achievements.ToListAsync();
                var userAchievements = await _context.AchievementProgress
                    .Where(ap => ap.UserId == userId)
                    .ToListAsync();

                var userStats = await _context.UserStats.FirstOrDefaultAsync(us => us.UserId == userId);
                var userProgression = await _context.UserProgressions.FirstOrDefaultAsync(up => up.UserId == userId);
                var philosopherCount = await _context.OwnedPhilosophers.CountAsync(op => op.UserId == userId);

                foreach (var achievement in allAchievements)
                {
                    var existingProgress = userAchievements.FirstOrDefault(ua => ua.AchievementId == achievement.Id);

                    if (existingProgress?.Completed == true)
                        continue;

                    var currentValue = await GetCurrentValueForAchievement(userId, achievement);
                    var targetValue = GetTargetValueForAchievement(achievement);
                    var isCompleted = currentValue >= targetValue;

                    if (existingProgress == null)
                    {
                        // Create new achievement progress
                        existingProgress = new AchievementProgress
                        {
                            UserId = userId,
                            AchievementId = achievement.Id,
                            CurrentValue = currentValue,
                            TargetValue = targetValue,
                            Completed = isCompleted,
                            UnlockedAt = isCompleted ? DateTime.UtcNow : (DateTime?)null,
                            Viewed = false
                        };
                        _context.AchievementProgress.Add(existingProgress);
                    }
                    else
                    {
                        // Update existing progress
                        existingProgress.CurrentValue = currentValue;
                        existingProgress.TargetValue = targetValue;

                        if (!existingProgress.Completed && isCompleted)
                        {
                            existingProgress.Completed = true;
                            existingProgress.UnlockedAt = DateTime.UtcNow;
                            existingProgress.Viewed = false;
                        }
                    }

                    if (isCompleted && !existingProgress.Completed)
                    {
                        // Grant achievement rewards
                        await GrantAchievementRewardsAsync(userId, achievement);
                        newAchievements.Add(achievement);

                        _logger.LogInformation("User {UserId} unlocked achievement {AchievementId}", userId, achievement.Id);
                    }
                }

                return newAchievements;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking achievements for user {UserId}", userId);
                return new List<Achievement>();
            }
        }

        public async Task<bool> UpdateAchievementProgressAsync(string userId, string achievementType, int value = 1)
        {
            // This method can be called from specific actions to update achievement progress
            var allAchievements = await _context.Achievements.ToListAsync();
            var matchingAchievements = allAchievements
                .Where(a => GetCriteriaType(a.CriteriaJson) == achievementType)
                .ToList();

            foreach (var achievement in matchingAchievements)
            {
                var progress = await _context.AchievementProgress
                    .FirstOrDefaultAsync(ap => ap.UserId == userId && ap.AchievementId == achievement.Id);

                if (progress == null)
                {
                    progress = new AchievementProgress
                    {
                        UserId = userId,
                        AchievementId = achievement.Id,
                        CurrentValue = value,
                        TargetValue = GetTargetValueForAchievement(achievement),
                        Completed = false,
                        Viewed = false
                    };
                    _context.AchievementProgress.Add(progress);
                }
                else if (!progress.Completed)
                {
                    progress.CurrentValue = Math.Max(progress.CurrentValue, value);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserProgression> RecalculateUserLevelAsync(string userId)
        {
            var userProgression = await _context.UserProgressions
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProgression != null)
            {
                var correctLevel = CalculateLevelFromExperience(userProgression.Experience);
                if (correctLevel != userProgression.Level)
                {
                    _logger.LogInformation("Correcting user {UserId} level from {OldLevel} to {NewLevel}",
                        userId, userProgression.Level, correctLevel);
                    userProgression.Level = correctLevel;
                    await _context.SaveChangesAsync();
                }
            }

            return userProgression;
        }

        public async Task<ProgressionSummary> GetProgressionSummaryAsync(string userId)
        {
            var userProgression = await _context.UserProgressions
                .FirstOrDefaultAsync(up => up.UserId == userId);

            var userStats = await _context.UserStats
                .FirstOrDefaultAsync(us => us.UserId == userId);

            var unlockedAchievements = await _context.AchievementProgress
                .Include(ap => ap.Achievement)
                .Where(ap => ap.UserId == userId && ap.Completed)
                .CountAsync();

            var totalAchievements = await _context.Achievements.CountAsync();

            var currentLevel = userProgression?.Level ?? 1;
            var currentExp = userProgression?.Experience ?? 0;
            var expForCurrentLevel = CalculateExperienceForLevel(currentLevel - 1);
            var expForNextLevel = CalculateExperienceForLevel(currentLevel);

            return new ProgressionSummary
            {
                CurrentLevel = currentLevel,
                CurrentExperience = currentExp,
                ExperienceForCurrentLevel = expForCurrentLevel,
                ExperienceForNextLevel = expForNextLevel,
                ExperienceToNextLevel = Math.Max(0, expForNextLevel - currentExp),
                UnlockedAchievements = unlockedAchievements,
                TotalAchievements = totalAchievements,
                QuizzesCompleted = userStats?.QuizzesCompleted ?? 0,
                PerfectScores = userStats?.PerfectScores ?? 0,
                StreakDays = userStats?.StreakDays ?? 0
            };
        }

        #region Private Methods

        private static int CalculateLevelFromExperience(int experience)
        {
            // Reverse the formula: level^2 * 100 = experience
            // So: level = sqrt(experience / 100)
            var level = (int)Math.Floor(Math.Sqrt(experience / 100.0)) + 1;
            return Math.Max(1, level);
        }

        private static int CalculateExperienceForLevel(int level)
        {
            return level * level * 100;
        }

        private async Task<int> GetCurrentValueForAchievement(string userId, Achievement achievement)
        {
            var criteriaType = GetCriteriaType(achievement.CriteriaJson);

            return criteriaType switch
            {
                "perfect_score" => await _context.UserStats
                    .Where(us => us.UserId == userId)
                    .Select(us => us.PerfectScores)
                    .FirstOrDefaultAsync(),

                "collection_count" => await _context.OwnedPhilosophers
                    .CountAsync(op => op.UserId == userId),

                "legendary_collection" => await _context.OwnedPhilosophers
                    .Include(op => op.Philosopher)
                    .CountAsync(op => op.UserId == userId && op.Philosopher.Rarity == Rarity.Legendary),

                "quiz_completion" => await _context.UserStats
                    .Where(us => us.UserId == userId)
                    .Select(us => us.QuizzesCompleted)
                    .FirstOrDefaultAsync(),

                "daily_streak" => await _context.UserStats
                    .Where(us => us.UserId == userId)
                    .Select(us => us.StreakDays)
                    .FirstOrDefaultAsync(),

                "player_level" => await _context.UserProgressions
                    .Where(up => up.UserId == userId)
                    .Select(up => up.Level)
                    .FirstOrDefaultAsync(),

                "lesson_completion" => JsonSerializer.Deserialize<string[]>(
                    await _context.UserProgressions
                        .Where(up => up.UserId == userId)
                        .Select(up => up.CompletedLessonsJson)
                        .FirstOrDefaultAsync() ?? "[]")?.Length ?? 0,

                "debate_wins" => 0, // TODO: Implement when you add debate system

                "win_streak" => 0, // TODO: Implement when you add streak tracking

                "lesson_speedrun" => 0, // TODO: Implement with lesson completion times

                _ => 0
            };
        }

        private int GetTargetValueForAchievement(Achievement achievement)
        {
            try
            {
                var criteria = JsonSerializer.Deserialize<Dictionary<string, object>>(achievement.CriteriaJson);
                if (criteria == null) return 1;

                // Try to get different possible target fields
                if (criteria.TryGetValue("minCount", out var minCountObj) && minCountObj != null)
                {
                    if (minCountObj is JsonElement element && element.TryGetInt32(out var minCount))
                        return minCount;
                    if (int.TryParse(minCountObj.ToString(), out var parsedMinCount))
                        return parsedMinCount;
                }

                if (criteria.TryGetValue("minWins", out var minWinsObj) && minWinsObj != null)
                {
                    if (minWinsObj is JsonElement element && element.TryGetInt32(out var minWins))
                        return minWins;
                    if (int.TryParse(minWinsObj.ToString(), out var parsedMinWins))
                        return parsedMinWins;
                }

                if (criteria.TryGetValue("minDays", out var minDaysObj) && minDaysObj != null)
                {
                    if (minDaysObj is JsonElement element && element.TryGetInt32(out var minDays))
                        return minDays;
                    if (int.TryParse(minDaysObj.ToString(), out var parsedMinDays))
                        return parsedMinDays;
                }

                return 1;
            }
            catch
            {
                return 1;
            }
        }

        private string GetCriteriaType(string criteriaJson)
        {
            try
            {
                var criteria = JsonSerializer.Deserialize<Dictionary<string, object>>(criteriaJson);
                if (criteria?.TryGetValue("type", out var typeObj) == true)
                {
                    if (typeObj is JsonElement element && element.ValueKind == JsonValueKind.String)
                        return element.GetString() ?? "";
                    return typeObj.ToString() ?? "";
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private async Task GrantLevelRewardsAsync(string userId, int fromLevel, int toLevel)
        {
            var userStats = await _context.UserStats.FirstOrDefaultAsync(us => us.UserId == userId);
            if (userStats == null) return;

            for (int level = fromLevel; level <= toLevel; level++)
            {
                if (_levelRewards.TryGetValue(level, out var rewards))
                {
                    userStats.GachaTickets += rewards.GachaTickets;
                    _logger.LogInformation("Granted level {Level} rewards to user {UserId}: {Tickets} tickets",
                        level, userId, rewards.GachaTickets);
                }
            }
        }

        private List<LevelRewards> GetLevelRewardsBetween(int fromLevel, int toLevel)
        {
            var rewards = new List<LevelRewards>();
            for (int level = fromLevel; level <= toLevel; level++)
            {
                if (_levelRewards.TryGetValue(level, out var reward))
                {
                    rewards.Add(reward);
                }
            }
            return rewards;
        }

        private async Task GrantAchievementRewardsAsync(string userId, Achievement achievement)
        {
            var userProgression = await _context.UserProgressions.FirstOrDefaultAsync(up => up.UserId == userId);
            var userStats = await _context.UserStats.FirstOrDefaultAsync(us => us.UserId == userId);

            if (userProgression != null)
            {
                userProgression.Experience += achievement.RewardExperience;
            }

            if (userStats != null)
            {
                userStats.GachaTickets += achievement.RewardGachaTickets;
            }
        }

        private async Task LogProgressionEventAsync(string userId, string source, int experience, int level, bool leveledUp, object? metadata)
        {
            // This could be implemented if you want to track progression events
            // For now, just log to the application logs
            _logger.LogInformation("Progression event for user {UserId}: {Source}, +{Experience} XP, Level {Level}, LevelUp: {LeveledUp}",
                userId, source, experience, level, leveledUp);
        }

        #endregion
    }
}