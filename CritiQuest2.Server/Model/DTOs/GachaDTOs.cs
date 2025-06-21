using CritiQuest2.Server.Model.Entities;
//using CritiQuest2.Server.Model.Enums;

namespace CritiQuest2.Server.DTOs
{
    public class GachaSummonRequest
    {
        public int TicketCount { get; set; } = 1;
    }

    public class GachaSummonResponse
    {
        public List<GachaSummonResult> Results { get; set; } = new();
        public int RemainingTickets { get; set; }
        public int TotalExperienceGained { get; set; }
    }

    public class GachaSummonResult
    {
        public PhilosopherSummaryDto Philosopher { get; set; } = null!;
        public bool IsNew { get; set; }
        public bool IsDuplicate { get; set; }
        public int ExperienceGained { get; set; }
        public int NewLevel { get; set; }
    }

    public class PhilosopherSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Era { get; set; } = string.Empty;
        public string School { get; set; } = string.Empty;
        public Rarity Rarity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class GachaRatesResponse
    {
        public Dictionary<string, double> RarityRates { get; set; } = new();
        public Dictionary<string, int> DuplicateExperience { get; set; } = new();
    }

    public class GachaPreviewResponse
    {
        public int AvailableTickets { get; set; }
        public int TotalPhilosophers { get; set; }
        public int OwnedPhilosophers { get; set; }
        public Dictionary<string, int> RarityBreakdown { get; set; } = new();
        public List<PhilosopherSummaryDto> FeaturedPhilosophers { get; set; } = new();
    }
}