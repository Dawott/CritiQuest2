using System.ComponentModel.DataAnnotations;

namespace CritiQuest2.Server.Model.Entities
{
    public enum InteractionType
    {
        Reflection = 1,
        ConceptMap = 2,
        Timeline = 3,
        Comparison = 4,
        Scenario = 5
    }

    // Interactive sections that can be linked to lessons
    public class InteractiveSection
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(100)]
        public string LessonId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public InteractionType Type { get; set; }

        public int OrderInLesson { get; set; } = 0;

        // JSON field for flexible configuration data
        public string ConfigurationJson { get; set; } = "{}";

        public bool IsRequired { get; set; } = false;
        public int EstimatedTimeMinutes { get; set; } = 5;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Lesson Lesson { get; set; } = null!;
        public ICollection<UserInteractionResponse> UserResponses { get; set; } = new List<UserInteractionResponse>();
    }

    // Reusable interaction templates
    public class InteractionTemplate
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public InteractionType Type { get; set; }

        // Template configuration that can be reused
        public string TemplateConfigJson { get; set; } = "{}";

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty; // e.g., "ethics", "logic", "metaphysics"

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // User responses to interactive sections
    public class UserInteractionResponse
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string InteractiveSectionId { get; set; } = string.Empty;

        // JSON field to store the actual response data
        public string ResponseDataJson { get; set; } = "{}";

        public int TimeSpentSeconds { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;

        // Analytics fields
        public int CompletionPercentage { get; set; } = 0;
        public int QualityScore { get; set; } = 0; // 0-100, calculated based on response depth

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public InteractiveSection InteractiveSection { get; set; } = null!;
    }

    // Progress tracking for interactive content
    public class InteractionProgress
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LessonId { get; set; } = string.Empty;

        public int TotalInteractiveSections { get; set; } = 0;
        public int CompletedSections { get; set; } = 0;
        public int TotalTimeSpentSeconds { get; set; } = 0;

        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
    }
}