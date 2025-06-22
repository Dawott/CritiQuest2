using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.DTOs;
using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;

namespace CritiQuest2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizzesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IProgressionService _progressionService;
        private readonly ILogger<QuizzesController> _logger;

        public QuizzesController(ApplicationDbContext context,IProgressionService progressionService,
            ILogger<QuizzesController> logger)
        {
            _context = context;
            _progressionService = progressionService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuiz(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();

            // Get user's previous attempts
            var userAttempts = await _context.QuizAttempts
                .Where(qa => qa.QuizId == id && qa.UserId == userId)
                .OrderByDescending(qa => qa.StartedAt)
                .ToListAsync();

            return Ok(new QuizDto
            {
                Id = quiz.Id,
                LessonId = quiz.LessonId,
                Title = quiz.Title,
                Type = quiz.Type.ToString(),
                TimeLimit = quiz.TimeLimit,
                PassingScore = quiz.PassingScore,
                PhilosopherBonus = string.IsNullOrEmpty(quiz.PhilosopherBonusJson)
                    ? null
                    : JsonSerializer.Deserialize<object>(quiz.PhilosopherBonusJson),
                Questions = quiz.Questions.OrderBy(q => q.Order).Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type.ToString(),
                    Options = JsonSerializer.Deserialize<string[]>(q.OptionsJson) ?? [],
                    PhilosophicalContext = q.PhilosophicalContext,
                    Points = q.Points,
                    Order = q.Order,
                    DebateConfig = string.IsNullOrEmpty(q.DebateConfigJson)
                        ? null
                        : JsonSerializer.Deserialize<object>(q.DebateConfigJson)
                }).ToList(),
                UserAttempts = userAttempts.Select(ua => new QuizAttemptDto
                {
                    Id = ua.Id,
                    StartedAt = ua.StartedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    CompletedAt = ua.CompletedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Score = ua.Score,
                    TimeSpent = ua.TimeSpent,
                    Passed = ua.Passed
                }).ToList()
            });
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult<StartAttemptResponse>> StartAttempt(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                return NotFound();

            var attempt = new QuizAttempt
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                QuizId = id,
                StartedAt = DateTime.UtcNow,
                AnswersJson = "[]",
                Score = 0,
                TimeSpent = 0,
                Passed = false
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            return Ok(new StartAttemptResponse { AttemptId = attempt.Id });
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz(string id, [FromBody] QuizSubmissionDto submission)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attempt = await _context.QuizAttempts
                .Include(qa => qa.Quiz)
                .ThenInclude(q => q.Questions)
                .FirstOrDefaultAsync(qa => qa.Id == submission.AttemptId && qa.UserId == userId);

            if (attempt == null)
                return NotFound();

            // Calculate score (keep your existing logic)
            var questionResults = new List<QuestionResultDto>();
            var totalPoints = 0;
            var earnedPoints = 0;

            foreach (var question in attempt.Quiz.Questions.OrderBy(q => q.Order))
            {
                var correctAnswers = JsonSerializer.Deserialize<string[]>(question.CorrectAnswersJson) ?? [];
                var userAnswer = submission.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var userAnswers = userAnswer?.SelectedAnswers ?? [];

                var isCorrect = AreAnswersEqual(correctAnswers, userAnswers);
                var pointsEarned = isCorrect ? question.Points : 0;

                totalPoints += question.Points;
                earnedPoints += pointsEarned;

                questionResults.Add(new QuestionResultDto
                {
                    QuestionId = question.Id,
                    Question = question.Text,
                    UserAnswers = userAnswers,
                    CorrectAnswers = correctAnswers,
                    IsCorrect = isCorrect,
                    Explanation = question.Explanation,
                    PhilosophicalContext = question.PhilosophicalContext,
                    Points = pointsEarned,
                    MaxPoints = question.Points
                });
            }

            var scorePercentage = totalPoints > 0 ? (earnedPoints * 100) / totalPoints : 0;
            var passed = scorePercentage >= attempt.Quiz.PassingScore;
            var isPerfectScore = scorePercentage == 100;

            // Update attempt (keep your existing logic)
            attempt.AnswersJson = JsonSerializer.Serialize(submission.Answers);
            attempt.Score = scorePercentage;
            attempt.TimeSpent = submission.TimeSpent;
            attempt.Passed = passed;
            attempt.CompletedAt = DateTime.UtcNow;

            // 🎯 NEW: Add progression logic here
            try
            {
                // Update user stats
                var userStats = await _context.UserStats.FirstOrDefaultAsync(us => us.UserId == userId);
                if (userStats != null)
                {
                    userStats.QuizzesCompleted++;
                    if (isPerfectScore)
                    {
                        userStats.PerfectScores++;
                    }
                }

                // Calculate experience reward based on your existing scoring
                var baseExperience = 20; // Base XP for completing any quiz
                var pointsBonus = (earnedPoints * 50) / Math.Max(totalPoints, 1); // Scale points to 0-50 XP
                var perfectBonus = isPerfectScore ? 30 : 0; // Extra 30 XP for perfect score
                var passBonus = passed ? 10 : 0; // Bonus for passing
                
                var totalExperience = baseExperience + pointsBonus + perfectBonus + passBonus;

                // Add experience and check for level ups/achievements
                var progressionResult = await _progressionService.AddExperienceAsync(
                    userId, 
                    totalExperience, 
                    "quiz_completion",
                    new { 
                        QuizId = id, 
                        Score = scorePercentage, 
                        IsPerfect = isPerfectScore,
                        Points = earnedPoints,
                        TotalPoints = totalPoints,
                        Passed = passed
                    }
                );

                // Update achievement progress
                await _progressionService.UpdateAchievementProgressAsync(userId, "quiz_completion", userStats?.QuizzesCompleted ?? 1);
                
                if (isPerfectScore)
                {
                    await _progressionService.UpdateAchievementProgressAsync(userId, "perfect_score", userStats?.PerfectScores ?? 1);
                }

                await _context.SaveChangesAsync();

                return Ok(new EnhancedQuizResultDto
                {
                    // Keep all existing quiz result data
                    AttemptId = attempt.Id,
                    Score = scorePercentage,
                    Passed = passed,
                    EarnedPoints = earnedPoints,
                    TotalPoints = totalPoints,
                    TimeSpent = submission.TimeSpent,
                    Results = questionResults,

                    // Add new progression data
                    Rewards = new QuizRewardsDto
                    {
                        Experience = totalExperience,
                        ExperienceBreakdown = new ExperienceBreakdownDto
                        {
                            Base = baseExperience,
                            PointsBonus = pointsBonus,
                            PerfectBonus = perfectBonus,
                            PassBonus = passBonus
                        }
                    },
                    Progression = new ProgressionResultDto
                    {
                        ExperienceGained = progressionResult.ExperienceGained,
                        CurrentLevel = progressionResult.CurrentLevel,
                        LeveledUp = progressionResult.LeveledUp,
                        NewLevel = progressionResult.NewLevel,
                        NewAchievements = progressionResult.NewAchievements.Select(a => new AchievementDto
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Description = a.Description,
                            RewardExperience = a.RewardExperience,
                            RewardGachaTickets = a.RewardGachaTickets
                        }).ToList()
                    },
                    Message = GetQuizCompletionMessage(scorePercentage, isPerfectScore, passed, progressionResult.LeveledUp)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing quiz progression for user {UserId}, quiz {QuizId}", userId, id);
                
                // Still save the quiz attempt even if progression fails
                await _context.SaveChangesAsync();
                
                // Return basic result without progression data
                return Ok(new QuizResultDto
                {
                    AttemptId = attempt.Id,
                    Score = scorePercentage,
                    Passed = passed,
                    EarnedPoints = earnedPoints,
                    TotalPoints = totalPoints,
                    TimeSpent = submission.TimeSpent,
                    Results = questionResults
                });
            }
        }

        [HttpGet("attempts/{attemptId}/results")]
        public async Task<ActionResult<QuizResultDto>> GetResults(string attemptId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var attempt = await _context.QuizAttempts
                .Include(qa => qa.Quiz)
                .ThenInclude(q => q.Questions)
                .FirstOrDefaultAsync(qa => qa.Id == attemptId && qa.UserId == userId);

            if (attempt == null)
                return NotFound();

            // Rebuild results from stored answers
            var submissionAnswers = JsonSerializer.Deserialize<List<AnswerDto>>(attempt.AnswersJson) ?? [];
            var questionResults = new List<QuestionResultDto>();

            foreach (var question in attempt.Quiz.Questions.OrderBy(q => q.Order))
            {
                var correctAnswers = JsonSerializer.Deserialize<string[]>(question.CorrectAnswersJson) ?? [];
                var userAnswer = submissionAnswers.FirstOrDefault(a => a.QuestionId == question.Id);
                var userAnswers = userAnswer?.SelectedAnswers ?? [];

                var isCorrect = AreAnswersEqual(correctAnswers, userAnswers);
                var pointsEarned = isCorrect ? question.Points : 0;

                questionResults.Add(new QuestionResultDto
                {
                    QuestionId = question.Id,
                    Question = question.Text,
                    UserAnswers = userAnswers,
                    CorrectAnswers = correctAnswers,
                    IsCorrect = isCorrect,
                    Explanation = question.Explanation,
                    PhilosophicalContext = question.PhilosophicalContext,
                    Points = pointsEarned,
                    MaxPoints = question.Points
                });
            }

            return Ok(new QuizResultDto
            {
                AttemptId = attempt.Id,
                Score = attempt.Score,
                Passed = attempt.Passed,
                EarnedPoints = questionResults.Sum(qr => qr.Points),
                TotalPoints = questionResults.Sum(qr => qr.MaxPoints),
                TimeSpent = attempt.TimeSpent,
                Results = questionResults
            });
        }

        private string GetQuizCompletionMessage(int score, bool isPerfect, bool passed, bool leveledUp)
        {
            if (leveledUp)
                return "🎉 Level up! Gratulacje!";

            if (isPerfect)
                return "🎯 Perfekcyjny wynik! Doskonałe myślenie krytyczne!";

            if (score >= 90)
                return "🌟 Niesamowita robota! Masz dociekliwy umysł!";

            if (score >= 80)
                return "👏 Dobra robota! Twoje rozumowanie jest solidne!";

            if (passed)
                return "✅ Nieźle! Zdałeś quiz!";

            return "📚 Więcej nauki! Filozofia wymaga ćwiczeń.";
        }

        private static bool AreAnswersEqual(string[] correct, string[] user)
        {
            if (correct.Length != user.Length) return false;

            var sortedCorrect = correct.OrderBy(a => a).ToArray();
            var sortedUser = user.OrderBy(a => a).ToArray();

            return sortedCorrect.SequenceEqual(sortedUser);
        }
    }
}