using System.ComponentModel.DataAnnotations;

namespace CritiQuest2.Server.Model.Entities
{
    public enum Difficulty
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }
    public class Lesson
    {
        [MaxLength(100)]
        public string Id { get; set; } = string.Empty; // np. "stoicism-intro"

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Stage { get; set; } = string.Empty;

        public int Order { get; set; }
        public Difficulty Difficulty { get; set; }
        public int EstimatedTime { get; set; } // minuty

        public string PhilosophicalConceptsJson { get; set; } = "[]"; // JSON array

        [MaxLength(100)]
        public string? RequiredPhilosopher { get; set; }

        public string ContentJson { get; set; } = "{}"; // JSON object

        [MaxLength(100)]
        public string QuizId { get; set; } = string.Empty;

        // Nagrody
        public int RewardXp { get; set; }
        public int RewardCoins { get; set; }
        public string RewardContentJson { get; set; } = "[]"; // JSON array

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Nawigator
        public ICollection<LessonProgress> UserProgress { get; set; } = new List<LessonProgress>();
    }

    public class LessonProgress
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        [MaxLength(100)]
        public string LessonId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        public int Score { get; set; } = 0; // 0-100
        public int TimeSpent { get; set; } = 0; // minuty

        [MaxLength(2000)]
        public string? Notes { get; set; }

        public int Attempts { get; set; } = 1;
        public int BestScore { get; set; } = 0;
    }

    public class LessonInteractionResponse
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LessonId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string SectionId { get; set; } = string.Empty;

        public InteractionType InteractionType { get; set; }

        // JSON field to store all responses flexibly
        public string ResponseDataJson { get; set; } = "{}";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
    }
}