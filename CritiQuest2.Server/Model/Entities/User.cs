using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CritiQuest2.Server.Model.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        //public int GachaTickets { get; set; } = 5;

        // Navigation properties
        public UserProgression? Progression { get; set; }
        public UserStats? Stats { get; set; }
        public ICollection<OwnedPhilosopher> PhilosopherCollection { get; set; } = new List<OwnedPhilosopher>();
        public ICollection<AchievementProgress> AchievementProgress { get; set; } = new List<AchievementProgress>();
        public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
    }

    public class UserProgression
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        [MaxLength(100)]
        public string CurrentStage { get; set; } = "ancient-philosophy";

        public string CompletedLessonsJson { get; set; } = "[]"; // JSON array of lesson IDs
        public string UnlockedPhilosophersJson { get; set; } = "[]"; // JSON array of philosopher IDs
    }

    public class UserStats
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int TotalTimeSpent { get; set; } = 0; // minuty
        public int StreakDays { get; set; } = 0;
        public DateTime LastStreakUpdate { get; set; } = DateTime.UtcNow;
        public int QuizzesCompleted { get; set; } = 0;
        public int PerfectScores { get; set; } = 0;
        public int GachaTickets { get; set; } = 0;
    }
}
