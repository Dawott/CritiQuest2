using CritiQuest2.Server.Model.Entities;
using System.Text.Json;

namespace CritiQuest2.Server.Data.Seeds
{
    public static class AchievementSeedData
    {
        public static List<Achievement> GetAchievements()
        {
            return new List<Achievement>
            {
                new Achievement
                {
                    Id = "first-perfect-score",
                    Name = "Doskonałość Filozofa",
                    Description = "Uzyskaj 100% w jakimkolwiek quizie",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "perfect_score", minCount = 1 }),
                    RewardExperience = 100,
                    RewardGachaTickets = 1
                },
                new Achievement
                {
                    Id = "quiz-master",
                    Name = "Myśliciel",
                    Description = "Uzyskaj perfekcyjny wynik w 10 quizach",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "perfect_score", minCount = 10 }),
                    RewardExperience = 500,
                    RewardGachaTickets = 5
                },
                new Achievement
                {
                    Id = "speed-learner",
                    Name = "Szybki Neuron",
                    Description = "Zakończ lekcję w 5 minut",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "lesson_speedrun", maxTime = 300 }),
                    RewardExperience = 150,
                    RewardGachaTickets = 2
                },
                new Achievement
                {
                    Id = "first-philosopher",
                    Name = "Wyjście z Jaskini",
                    Description = "Odblokuj pierwszego filozofa",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "collection_count", minCount = 1 }),
                    RewardExperience = 50,
                    RewardGachaTickets = 1
                },
                new Achievement
                {
                    Id = "school-collector",
                    Name = "Gimnazjon",
                    Description = "Uzyskaj 10 filozofów",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "collection_count", minCount = 10 }),
                    RewardExperience = 300,
                    RewardGachaTickets = 3
                },
                new Achievement
                {
                    Id = "legendary-collector",
                    Name = "Kurator Legend",
                    Description = "Uzyskaj 3 legendarnych filozofów",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "legendary_collection", minCount = 3 }),
                    RewardExperience = 1000,
                    RewardGachaTickets = 10
                },
                new Achievement
                {
                    Id = "first-debate-victory",
                    Name = "Sztuka Erystyki",
                    Description = "Wygraj pierwszą debatę",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "debate_wins", minWins = 1 }),
                    RewardExperience = 100,
                    RewardGachaTickets = 1
                },
                new Achievement
                {
                    Id = "debate-champion",
                    Name = "Faktami i Logiką",
                    Description = "Wygraj 20 debat",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "debate_wins", minWins = 20 }),
                    RewardExperience = 500,
                    RewardGachaTickets = 5
                },
                new Achievement
                {
                    Id = "undefeated-philosopher",
                    Name = "Zatwardziała Logika",
                    Description = "Wygraj 5 debat pod rząd",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "win_streak", minWins = 5 }),
                    RewardExperience = 400,
                    RewardGachaTickets = 4
                },
                new Achievement
                {
                    Id = "daily-thinker",
                    Name = "Praktyka Czyni",
                    Description = "Ukończ quiz codziennie przez 7 dni",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "daily_streak", minDays = 7 }),
                    RewardExperience = 200,
                    RewardGachaTickets = 2
                },
                new Achievement
                {
                    Id = "devoted-student",
                    Name = "Wierni",
                    Description = "Zachowaj 30-dniowy streak",
                    CriteriaJson = JsonSerializer.Serialize(new { type = "daily_streak", minDays = 30 }),
                    RewardExperience = 1000,
                    RewardGachaTickets = 10
                }
            };
        }
    }
}
