using Microsoft.EntityFrameworkCore;
using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
//using CritiQuest2.Server.Model.Enums;
using CritiQuest2.Server.DTOs;

namespace CritiQuest2.Server.Services
{
    public interface IGachaService
    {
        Task<GachaSummonResponse> PerformSummonAsync(string userId, int ticketCount = 1);
        GachaRatesResponse GetGachaRates();
        Task<GachaPreviewResponse> GetGachaPreviewAsync(string userId);
    }

    public class GachaService : IGachaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GachaService> _logger;

        // Rarity weights for gacha (higher = more common)
        private readonly Dictionary<Rarity, double> _rarityWeights = new()
        {
            { Rarity.Common, 50.0 },      // 50%
            { Rarity.Uncommon, 30.0 },    // 30%
            { Rarity.Rare, 15.0 },        // 15%
            { Rarity.Epic, 4.0 },         // 4%
            { Rarity.Legendary, 1.0 }     // 1%
        };

        // Experience gained from duplicates based on rarity
        private readonly Dictionary<Rarity, int> _duplicateExperience = new()
        {
            { Rarity.Common, 10 },
            { Rarity.Uncommon, 25 },
            { Rarity.Rare, 50 },
            { Rarity.Epic, 100 },
            { Rarity.Legendary, 250 }
        };

        public GachaService(ApplicationDbContext context, ILogger<GachaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GachaSummonResponse> PerformSummonAsync(string userId, int ticketCount = 1)
        {
            if (ticketCount <= 0 || ticketCount > 10)
            {
                throw new InvalidOperationException("Invalid ticket count. Must be between 1 and 10.");
            }

            var user = await _context.Users
                .Include(u => u.Stats)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (user.Stats == null)
            {
                throw new InvalidOperationException("User stats not found");
            }

            if (user.Stats.GachaTickets < ticketCount)
            {
                throw new InvalidOperationException($"Insufficient tickets. You have {user.Stats.GachaTickets}, need {ticketCount}.");
            }

            // Get all philosophers for selection
            var allPhilosophers = await _context.Philosophers.ToListAsync();
            if (!allPhilosophers.Any())
            {
                throw new InvalidOperationException("No philosophers available for summoning");
            }

            // Get user's current collection
            var userCollection = await _context.OwnedPhilosophers
                .Where(op => op.UserId == userId)
                .Include(op => op.Philosopher)
                .ToListAsync();

            var results = new List<GachaSummonResult>();
            int totalExperienceGained = 0;

            // Perform summons
            for (int i = 0; i < ticketCount; i++)
            {
                var selectedPhilosopher = SelectRandomPhilosopher(allPhilosophers);
                var existingOwned = userCollection.FirstOrDefault(oc => oc.PhilosopherId == selectedPhilosopher.Id);

                var result = new GachaSummonResult
                {
                    Philosopher = MapToSummaryDto(selectedPhilosopher),
                    IsNew = existingOwned == null,
                    IsDuplicate = existingOwned != null
                };

                if (existingOwned == null)
                {
                    // New philosopher - add to collection
                    var ownedPhilosopher = new OwnedPhilosopher
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        PhilosopherId = selectedPhilosopher.Id,
                        Level = 1,
                        Experience = 0,
                        Duplicates = 0,
                        CurrentWisdom = selectedPhilosopher.Wisdom,
                        CurrentLogic = selectedPhilosopher.Logic,
                        CurrentRhetoric = selectedPhilosopher.Rhetoric,
                        CurrentInfluence = selectedPhilosopher.Influence,
                        CurrentOriginality = selectedPhilosopher.Originality,
                        ObtainedAt = DateTime.UtcNow
                    };

                    _context.OwnedPhilosophers.Add(ownedPhilosopher);
                    userCollection.Add(ownedPhilosopher);

                    result.ExperienceGained = 0;
                    result.NewLevel = 1;

                    _logger.LogInformation("User {UserId} obtained new philosopher {PhilosopherId}", userId, selectedPhilosopher.Id);
                }
                else
                {
                    // Duplicate - add experience and increment duplicate count
                    var expGained = _duplicateExperience[selectedPhilosopher.Rarity];
                    existingOwned.Experience += expGained;
                    existingOwned.Duplicates++;

                    // Check for level up (simple: every 100 exp = 1 level)
                    var newLevel = (existingOwned.Experience / 100) + 1;
                    var leveledUp = newLevel > existingOwned.Level;

                    if (leveledUp)
                    {
                        existingOwned.Level = newLevel;
                        // Boost stats on level up (5% increase per level)
                        var boostMultiplier = 1 + (newLevel - 1) * 0.05;
                        existingOwned.CurrentWisdom = (int)(selectedPhilosopher.Wisdom * boostMultiplier);
                        existingOwned.CurrentLogic = (int)(selectedPhilosopher.Logic * boostMultiplier);
                        existingOwned.CurrentRhetoric = (int)(selectedPhilosopher.Rhetoric * boostMultiplier);
                        existingOwned.CurrentInfluence = (int)(selectedPhilosopher.Influence * boostMultiplier);
                        existingOwned.CurrentOriginality = (int)(selectedPhilosopher.Originality * boostMultiplier);
                    }

                    result.ExperienceGained = expGained;
                    result.NewLevel = newLevel;
                    totalExperienceGained += expGained;

                    _logger.LogInformation("User {UserId} got duplicate {PhilosopherId}, gained {Exp} exp",
                        userId, selectedPhilosopher.Id, expGained);
                }

                results.Add(result);
            }

            // Deduct tickets
            user.Stats.GachaTickets -= ticketCount;

            await _context.SaveChangesAsync();

            return new GachaSummonResponse
            {
                Results = results,
                RemainingTickets = user.Stats.GachaTickets,
                TotalExperienceGained = totalExperienceGained
            };
        }

        public GachaRatesResponse GetGachaRates()
        {
            var totalWeight = _rarityWeights.Values.Sum();
            var rates = _rarityWeights.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => (kvp.Value / totalWeight) * 100
            );

            var duplicateExp = _duplicateExperience.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value
            );

            return new GachaRatesResponse
            {
                RarityRates = rates,
                DuplicateExperience = duplicateExp
            };
        }

        public async Task<GachaPreviewResponse> GetGachaPreviewAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Stats)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var totalPhilosophers = await _context.Philosophers.CountAsync();
            var ownedCount = await _context.OwnedPhilosophers.CountAsync(op => op.UserId == userId);

            var rarityBreakdown = await _context.Philosophers
                .GroupBy(p => p.Rarity)
                .Select(g => new { Rarity = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Rarity.ToString(), x => x.Count);

            // Get some featured philosophers (random selection of legendary/epic)
            var featuredPhilosophers = await _context.Philosophers
                .Where(p => p.Rarity == Rarity.Legendary || p.Rarity == Rarity.Epic)
                .OrderBy(p => Guid.NewGuid())
                .Take(3)
                .Select(p => MapToSummaryDto(p))
                .ToListAsync();

            return new GachaPreviewResponse
            {
                AvailableTickets = user.Stats?.GachaTickets ?? 0,
                TotalPhilosophers = totalPhilosophers,
                OwnedPhilosophers = ownedCount,
                RarityBreakdown = rarityBreakdown,
                FeaturedPhilosophers = featuredPhilosophers
            };
        }

        private Philosopher SelectRandomPhilosopher(List<Philosopher> philosophers)
        {
            var random = new Random();
            var totalWeight = _rarityWeights.Values.Sum();
            var randomValue = random.NextDouble() * totalWeight;

            double currentWeight = 0;
            Rarity selectedRarity = Rarity.Common;

            foreach (var kvp in _rarityWeights)
            {
                currentWeight += kvp.Value;
                if (randomValue <= currentWeight)
                {
                    selectedRarity = kvp.Key;
                    break;
                }
            }

            // Get philosophers of selected rarity
            var philosophersOfRarity = philosophers.Where(p => p.Rarity == selectedRarity).ToList();

            if (!philosophersOfRarity.Any())
            {
                // Fallback to any philosopher if none of selected rarity exist
                philosophersOfRarity = philosophers;
            }

            var selectedIndex = random.Next(philosophersOfRarity.Count);
            return philosophersOfRarity[selectedIndex];
        }

        private static PhilosopherSummaryDto MapToSummaryDto(Philosopher philosopher)
        {
            return new PhilosopherSummaryDto
            {
                Id = philosopher.Id,
                Name = philosopher.Name,
                Era = philosopher.Era,
                School = philosopher.School,
                Rarity = philosopher.Rarity,
                ImageUrl = philosopher.ImageUrl,
                Description = philosopher.Description
            };
        }
    }
}