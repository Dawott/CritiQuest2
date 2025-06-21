using System.ComponentModel.DataAnnotations;

namespace CritiQuest2.Server.Model.Entities
{
    public class Achievement
    {
        public string Id { get; set; } = string.Empty; // np. "first-perfect-score"

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // Kryteria
        public string CriteriaJson { get; set; } = "{}";

        // NAgrody
        public int RewardExperience { get; set; }
        public int RewardGachaTickets { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Nawigator
        public ICollection<AchievementProgress> UserProgress { get; set; } = new List<AchievementProgress>();
    }

    public class AchievementProgress
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string AchievementId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public Achievement Achievement { get; set; } = null!;

        public int CurrentValue { get; set; } = 0;
        public int TargetValue { get; set; } = 1;
        public bool Completed { get; set; } = false;
        public DateTime? UnlockedAt { get; set; }
        public bool Viewed { get; set; } = false;

        public decimal Progress => TargetValue > 0 ? (decimal)CurrentValue / TargetValue * 100 : 0;
    }
}
