using CritiQuest2.Server.Model.Entities;

namespace CritiQuest2.Server.Model.DTOs
{
    public enum LessonSectionType
    {
        Text = 1,
        Interactive = 2,
        Video = 3,
        Image = 4
    }
    public class LessonContentDto
    {
        public List<LessonSectionDto> Sections { get; set; } = new();
    }

    public class LessonSectionDto
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public LessonSectionType Type { get; set; }
        public string? MediaUrl { get; set; }
        public InteractionType? InteractionType { get; set; }
        public object? InteractionData { get; set; }
    }

    // Specific interaction data structures
    public class ReflectionInteractionData
    {
        public string Scenario { get; set; } = "";
        public List<string> Prompts { get; set; } = new();
        public string Guidance { get; set; } = "";
    }

    public class ConceptMapInteractionData
    {
        public string CentralConcept { get; set; } = "";
        public List<string> RelatedConcepts { get; set; } = new();
        public string Instructions { get; set; } = "";
    }

    public class TimelineInteractionData
    {
        public string Topic { get; set; } = "";
        public List<TimelineEvent> Events { get; set; } = new();
        public string Instructions { get; set; } = "";
    }

    public class TimelineEvent
    {
        public string Year { get; set; } = "";
        public string Event { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class ComparisonInteractionData
    {
        public string Topic { get; set; } = "";
        public List<ComparisonItem> Items { get; set; } = new();
        public List<string> Criteria { get; set; } = new();
        public string Instructions { get; set; } = "";
    }

    public class ComparisonItem
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Dictionary<string, string> Properties { get; set; } = new();
    }
}
