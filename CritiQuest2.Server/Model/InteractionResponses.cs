namespace CritiQuest2.Server.Model
{
    public class ReflectionResponse
    {
        public List<string> Responses { get; set; } = new();
        public int ThoughtfulnessScore { get; set; } = 0;
        public List<string> DetectedConcepts { get; set; } = new();
    }

    public class ConceptMapResponse
    {
        public List<UserConcept> UserConcepts { get; set; } = new();
        public List<ConceptRelationship> Relationships { get; set; } = new();
        public int ComplexityScore { get; set; } = 0;
    }

    public class UserConcept
    {
        public string Text { get; set; } = "";
        public ConceptPosition Position { get; set; } = new();
    }

    public class ConceptPosition
    {
        public string Left { get; set; } = "0px";
        public string Top { get; set; } = "0px";
    }

    public class ConceptRelationship
    {
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class TimelineResponse
    {
        public List<UserTimelineEvent> UserEvents { get; set; } = new();
        public TimelineAnalysis Analysis { get; set; } = new();
    }

    public class UserTimelineEvent
    {
        public string Id { get; set; } = "";
        public string Year { get; set; } = "";
        public string Event { get; set; } = "";
        public string Description { get; set; } = "";
        public string Significance { get; set; } = "";
    }

    public class TimelineAnalysis
    {
        public int TotalEvents { get; set; } = 0;
        public int UserContributions { get; set; } = 0;
        public int TimeSpan { get; set; } = 0;
        public int AverageSpacing { get; set; } = 0;
    }

    public class ComparisonResponse
    {
        public Dictionary<string, string> Comparisons { get; set; } = new();
        public List<string> CustomCriteria { get; set; } = new();
        public string FinalEvaluation { get; set; } = "";
        public ComparisonStats Stats { get; set; } = new();
    }

    public class ComparisonStats
    {
        public int TotalCriteria { get; set; } = 0;
        public int FilledComparisons { get; set; } = 0;
        public int CompletionPercentage { get; set; } = 0;
        public int AverageLength { get; set; } = 0;
    }
}
