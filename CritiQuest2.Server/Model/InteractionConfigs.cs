namespace CritiQuest2.Server.Model
{
    public class ReflectionConfig
    {
        public string Scenario { get; set; } = "";
        public List<string> Prompts { get; set; } = new();
        public string Guidance { get; set; } = "";
        public List<string> PhilosophicalConcepts { get; set; } = new();
    }

    public class ConceptMapConfig
    {
        public string CentralConcept { get; set; } = "";
        public List<string> RelatedConcepts { get; set; } = new();
        public string Instructions { get; set; } = "";
        public int MinimumConnections { get; set; } = 3;
    }

    public class TimelineConfig
    {
        public string Topic { get; set; } = "";
        public List<TimelineEvent> PredefinedEvents { get; set; } = new();
        public string Instructions { get; set; } = "";
        public int MinimumUserEvents { get; set; } = 2;
    }

    public class TimelineEvent
    {
        public string Year { get; set; } = "";
        public string Event { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class ComparisonConfig
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
