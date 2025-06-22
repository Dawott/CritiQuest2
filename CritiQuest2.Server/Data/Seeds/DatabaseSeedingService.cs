using CritiQuest2.Server.Data;
using CritiQuest2.Server.Data.Seeds;
using CritiQuest2.Server.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace CritiQuest2.Server.Services
{
    public interface IDatabaseSeedingService
    {
        Task SeedDatabaseAsync();
        Task SeedPhilosophersAsync();
        Task SeedAchievementsAsync();
        Task SeedLessonsAsync();
        Task SeedQuizzesAsync();
        Task SeedDebateArgumentsAsync();
        Task SeedInteractiveLessonsAsync();
        Task UpdateLessonContentAsync(string lessonId);
        Task UpdateAllLessonsContentAsync();
    }

    public class DatabaseSeedingService : IDatabaseSeedingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseSeedingService> _logger;

        public DatabaseSeedingService(ApplicationDbContext context, ILogger<DatabaseSeedingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Seed in proper order (considering foreign key dependencies)
                await SeedPhilosophersAsync();
                await SeedAchievementsAsync();
                await SeedLessonsAsync();
                await SeedQuizzesAsync();
                await SeedDebateArgumentsAsync();
                await SeedInteractiveLessonsAsync();

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database seeding");
                throw;
            }
        }

        public async Task SeedPhilosophersAsync()
        {
            if (await _context.Philosophers.AnyAsync())
            {
                _logger.LogInformation("Philosophers already exist, skipping seeding");
                return;
            }

            _logger.LogInformation("Seeding philosophers...");

            var philosophers = PhilosopherSeedData.GetPhilosophers();
            await _context.Philosophers.AddRangeAsync(philosophers);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {philosophers.Count} philosophers");
        }

        public async Task SeedAchievementsAsync()
        {
            if (await _context.Achievements.AnyAsync())
            {
                _logger.LogInformation("Achievements already exist, skipping seeding");
                return;
            }

            _logger.LogInformation("Seeding achievements...");

            var achievements = AchievementSeedData.GetAchievements();
            await _context.Achievements.AddRangeAsync(achievements);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {achievements.Count} achievements");
        }

        public async Task SeedInteractiveLessonsAsync()
        {
            if (await _context.InteractiveSections.AnyAsync())
            {
                _logger.LogInformation("Interactive sections already exist, skipping seeding");
                return;
            }

            _logger.LogInformation("Seeding interactive sections...");

            InteractiveLessonSeed.SeedInteractiveScenarios(_context);

            var sectionCount = await _context.InteractiveSections.CountAsync();
            _logger.LogInformation($"Seeded {sectionCount} interactive sections");
        }

        public async Task SeedLessonsAsync()
        {
            if (await _context.Lessons.AnyAsync())
            {
                _logger.LogInformation("Lessons already exist, skipping seeding");
                return;
            }

            _logger.LogInformation("Seeding lessons...");

            var lessons = LessonSeedData.GetLessons();
            await _context.Lessons.AddRangeAsync(lessons);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {lessons.Count} lessons");
        }

        public async Task UpdateLessonContentAsync(string lessonId)
        {
            _logger.LogInformation($"Updating content for lesson: {lessonId}");

            var seedLessons = LessonSeedData.GetLessons();
            var seedLesson = seedLessons.FirstOrDefault(l => l.Id == lessonId);

            if (seedLesson == null)
            {
                throw new ArgumentException($"Lesson with ID '{lessonId}' not found in seed data");
            }

            var existingLesson = await _context.Lessons.FindAsync(lessonId);
            if (existingLesson == null)
            {
                throw new ArgumentException($"Lesson with ID '{lessonId}' not found in database");
            }

            // Update ContentJson using the same serialization as seed data
            existingLesson.ContentJson = seedLesson.ContentJson;
            existingLesson.UpdatedAt = DateTime.UtcNow;

            _context.Lessons.Update(existingLesson);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully updated content for lesson: {lessonId}");
        }

        public async Task UpdateAllLessonsContentAsync()
        {
            _logger.LogInformation("Updating content for all lessons from seed data");

            var seedLessons = LessonSeedData.GetLessons();
            var updatedCount = 0;

            foreach (var seedLesson in seedLessons)
            {
                var existingLesson = await _context.Lessons.FindAsync(seedLesson.Id);
                if (existingLesson != null)
                {
                    existingLesson.ContentJson = seedLesson.ContentJson;
                    existingLesson.UpdatedAt = DateTime.UtcNow;
                    _context.Lessons.Update(existingLesson);
                    updatedCount++;
                    _logger.LogInformation($"Updated content for lesson: {seedLesson.Id}");
                }
                else
                {
                    _logger.LogWarning($"Lesson {seedLesson.Id} found in seed data but not in database");
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Successfully updated content for {updatedCount} lessons");
        }

        public async Task SeedQuizzesAsync()
        {
            if (await _context.Quizzes.AnyAsync())
            {
                _logger.LogInformation("Quizzes already exist, skipping seeding");
                return;
            }

            _logger.LogInformation("Seeding quizzes and questions...");

            var quizzes = QuizSeedData.GetQuizzes();
            var questions = QuizSeedData.GetQuestions();

            await _context.Quizzes.AddRangeAsync(quizzes);
            await _context.SaveChangesAsync();

            await _context.Questions.AddRangeAsync(questions);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {quizzes.Count} quizzes and {questions.Count} questions");
        }

        public async Task SeedDebateArgumentsAsync()
        {
            if (await _context.DebateArguments.AnyAsync())
            {
                _logger.LogInformation("Debate arguments already exist, skipping seeding");
                return;
            }

            _logger.LogInformation("Seeding debate arguments...");

            var debateArguments = DebateArgumentSeedData.GetDebateArguments();
            await _context.DebateArguments.AddRangeAsync(debateArguments);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {debateArguments.Count} debate arguments");
        }
    }



    // Extension method for easy seeding
    public static class DatabaseSeedingExtensions
    {
        public static async Task<IServiceProvider> SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var seedingService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedingService>();
            await seedingService.SeedDatabaseAsync();
            return serviceProvider;
        }
    }
}