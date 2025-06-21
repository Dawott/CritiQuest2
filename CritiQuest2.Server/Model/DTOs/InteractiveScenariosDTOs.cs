using Microsoft.AspNetCore.Mvc;

namespace CritiQuest2.Server.Model.DTOs
{
    public class InteractiveSectionDto
    {
        public string Id { get; set; } = "";
        public string LessonId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public int OrderInLesson { get; set; }
        public object? Configuration { get; set; }
        public bool IsRequired { get; set; }
        public int EstimatedTimeMinutes { get; set; }
        public UserResponseDto? UserResponse { get; set; }
    }

    public class UserResponseDto
    {
        public string Id { get; set; } = "";
        public object? ResponseData { get; set; }
        public int TimeSpentSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public int CompletionPercentage { get; set; }
        public int QualityScore { get; set; }
        public string LastUpdatedAt { get; set; } = "";
    }

    public class SaveResponseRequest
    {
        public object ResponseData { get; set; } = new();
        public int TimeSpentSeconds { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class InteractionProgressDto
    {
        public string LessonId { get; set; } = "";
        public int TotalSections { get; set; }
        public int CompletedSections { get; set; }
        public int CompletionPercentage { get; set; }
        public int TotalTimeSpentSeconds { get; set; }
        public string LastActivityAt { get; set; } = "";
    }
}
