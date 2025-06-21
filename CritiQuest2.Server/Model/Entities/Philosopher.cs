using System.ComponentModel.DataAnnotations;

namespace CritiQuest2.Server.Model.Entities
{
    public enum Rarity
    {
        Common = 1,
        Uncommon = 2,
        Rare = 3,
        Epic = 4,
        Legendary = 5
    }
    public class Philosopher
    {
        [MaxLength(100)]
        public string Id { get; set; } = string.Empty; // np. "marcus-aurelius"

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Era { get; set; } = string.Empty;

        [MaxLength(100)]
        public string School { get; set; } = string.Empty;

        public Rarity Rarity { get; set; }

        // Base stats
        public int Wisdom { get; set; }
        public int Logic { get; set; }
        public int Rhetoric { get; set; }
        public int Influence { get; set; }
        public int Originality { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public string QuotesJson { get; set; } = "[]"; // JSON array
        public string SpecialAbilityJson { get; set; } = "{}"; // JSON object

        // Navigation properties
        public ICollection<OwnedPhilosopher> OwnedBy { get; set; } = new List<OwnedPhilosopher>();
    }

    public class OwnedPhilosopher
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        [MaxLength(100)]
        public string PhilosopherId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public Philosopher Philosopher { get; set; } = null!;

        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Duplicates { get; set; } = 0;

        // Aktualne statystyki
        public int CurrentWisdom { get; set; }
        public int CurrentLogic { get; set; }
        public int CurrentRhetoric { get; set; }
        public int CurrentInfluence { get; set; }
        public int CurrentOriginality { get; set; }

        public DateTime ObtainedAt { get; set; } = DateTime.UtcNow;
    }
}
