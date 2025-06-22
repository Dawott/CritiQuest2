using CritiQuest2.Server.Model.Entities;

namespace CritiQuest2.Server.Model.DTOs
{
    public class ProgressionResult
    {
        public int ExperienceGained { get; set; }
        public int TotalExperience { get; set; }
        public int CurrentLevel { get; set; }
        public bool LeveledUp { get; set; }
        public int NewLevel { get; set; }
        public List<LevelRewards> LevelRewards { get; set; } = new();
        public List<Achievement> NewAchievements { get; set; } = new();
        public string Source { get; set; } = "";
    }

    public class ProgressionSummary
    {
        public int CurrentLevel { get; set; }
        public int CurrentExperience { get; set; }
        public int ExperienceForCurrentLevel { get; set; }
        public int ExperienceForNextLevel { get; set; }
        public int ExperienceToNextLevel { get; set; }
        public int UnlockedAchievements { get; set; }
        public int TotalAchievements { get; set; }
        public int QuizzesCompleted { get; set; }
        public int PerfectScores { get; set; }
        public int StreakDays { get; set; }
    }

    public class LevelRewards
    {
        public int Level { get; set; }
        public int GachaTickets { get; set; }
        public int Experience { get; set; }
        public string? PhilosopherId { get; set; }
        public string? BadgeId { get; set; }
    }

}