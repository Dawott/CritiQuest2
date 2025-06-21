using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using System.Security.Claims;
using System.Text.Json;
using CritiQuest2.Server.Extensions;

namespace CritiQuest2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class QuizzesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuizzesController> _logger;

        public QuizzesController(ApplicationDbContext context, ILogger<QuizzesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get quiz by ID with questions
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuiz(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.Questions.OrderBy(qu => qu.Order))
                    .Where(q => q.Id == id)
                    .Select(q => new
                    {
                        q.Id,
                        q.LessonId,
                        q.Title,
                        q.Type,
                        q.TimeLimit,
                        q.PassingScore,
                        PhilosopherBonus = string.IsNullOrEmpty(q.PhilosopherBonusJson)
                            ? null
                            : q.PhilosopherBonusJson.DeserializeObject(),
                        Questions = q.Questions.Select(qu => new
                        {
                            qu.Id,
                            qu.Text,
                            qu.Type,
                            Options = qu.OptionsJson.DeserializeStringArray(),
                            qu.PhilosophicalContext,
                            qu.Points,
                            qu.Order,
                            DebateConfig = string.IsNullOrEmpty(qu.DebateConfigJson)
                                ? null
                                : qu.DebateConfigJson.DeserializeObject()
                        }).ToList(),
                        UserAttempts = _context.QuizAttempts
                            .Where(qa => qa.UserId == userId && qa.QuizId == id)
                            .Select(qa => new
                            {
                                qa.Id,
                                qa.StartedAt,
                                qa.CompletedAt,
                                qa.Score,
                                qa.TimeSpent,
                                qa.Passed
                            })
                            .OrderByDescending(qa => qa.StartedAt)
                            .Take(5)
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (quiz == null)
                    return NotFound();

                return Ok(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching quiz {QuizId} for user {UserId}", id, userId);
                return StatusCode(500, new { message = "Error fetching quiz" });
            }
        }

        /// <summary>
        /// Start a new quiz attempt
        /// </summary>
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartQuizAttempt(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var quiz = await _context.Quizzes.FindAsync(id);
                if (quiz == null)
                    return NotFound();

                var attempt = new QuizAttempt
                {
                    UserId = userId,
                    QuizId = id,
                    StartedAt = DateTime.UtcNow
                };

                _context.QuizAttempts.Add(attempt);
                await _context.SaveChangesAsync();

                return Ok(new { attemptId = attempt.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting quiz attempt for quiz {QuizId} by user {UserId}", id, userId);
                return StatusCode(500, new { message = "Error starting quiz attempt" });
            }
        }

        /// <summary>
        /// Submit quiz attempt with answers
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<IActionResult> SubmitQuizAttempt(string id, [FromBody] QuizSubmissionRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.Questions)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                    return NotFound();

                var attempt = await _context.QuizAttempts
                    .FirstOrDefaultAsync(qa => qa.Id == request.AttemptId && qa.UserId == userId);

                if (attempt == null)
                    return NotFound("Quiz attempt not found");

                // Calculate score
                int totalPoints = 0;
                int earnedPoints = 0;
                var questionAnswers = new List<QuestionAnswer>();

                foreach (var question in quiz.Questions)
                {
                    totalPoints += question.Points;
                    var userAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == question.Id);

                    if (userAnswer != null)
                    {
                        var correctAnswers = JsonSerializer.Deserialize<string[]>(question.CorrectAnswersJson) ?? new string[0];
                        var isCorrect = AreAnswersCorrect(userAnswer.SelectedAnswers, correctAnswers);
                        var pointsEarned = isCorrect ? question.Points : 0;
                        earnedPoints += pointsEarned;

                        questionAnswers.Add(new QuestionAnswer
                        {
                            QuizAttemptId = attempt.Id,
                            QuestionId = question.Id,
                            AnswerJson = JsonSerializer.Serialize(userAnswer.SelectedAnswers),
                            IsCorrect = isCorrect,
                            PointsEarned = pointsEarned,
                            AnsweredAt = DateTime.UtcNow
                        });
                    }
                }

                int scorePercentage = totalPoints > 0 ? (int)((double)earnedPoints / totalPoints * 100) : 0;
                bool passed = scorePercentage >= quiz.PassingScore;

                // Update attempt
                attempt.CompletedAt = DateTime.UtcNow;
                attempt.Score = scorePercentage;
                attempt.TimeSpent = request.TimeSpent;
                attempt.Passed = passed;
                attempt.AnswersJson = JsonSerializer.Serialize(request.Answers);

                // Add question answers
                _context.QuestionAnswers.AddRange(questionAnswers);

                // Update user stats
                var userStats = await _context.UserStats
                    .FirstOrDefaultAsync(us => us.UserId == userId);

                if (userStats != null)
                {
                    userStats.QuizzesCompleted++;
                    if (scorePercentage == 100)
                        userStats.PerfectScores++;
                }

                await _context.SaveChangesAsync();

                // Return detailed results
                var results = quiz.Questions.Select(q => {
                    var userAnswer = request.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
                    var correctAnswers = JsonSerializer.Deserialize<string[]>(q.CorrectAnswersJson) ?? new string[0];
                    var isCorrect = userAnswer != null && AreAnswersCorrect(userAnswer.SelectedAnswers, correctAnswers);

                    return new
                    {
                        QuestionId = q.Id,
                        Question = q.Text,
                        UserAnswers = userAnswer?.SelectedAnswers ?? new string[0],
                        CorrectAnswers = correctAnswers,
                        IsCorrect = isCorrect,
                        Explanation = q.Explanation,
                        PhilosophicalContext = q.PhilosophicalContext,
                        Points = isCorrect ? q.Points : 0,
                        MaxPoints = q.Points
                    };
                }).ToList();

                return Ok(new
                {
                    AttemptId = attempt.Id,
                    Score = scorePercentage,
                    Passed = passed,
                    EarnedPoints = earnedPoints,
                    TotalPoints = totalPoints,
                    TimeSpent = request.TimeSpent,
                    Results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz attempt for quiz {QuizId} by user {UserId}", id, userId);
                return StatusCode(500, new { message = "Error submitting quiz attempt" });
            }
        }

        /// <summary>
        /// Get quiz results for a specific attempt
        /// </summary>
        [HttpGet("attempts/{attemptId}/results")]
        public async Task<IActionResult> GetQuizResults(string attemptId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var attempt = await _context.QuizAttempts
                    .Include(qa => qa.Quiz)
                    .ThenInclude(q => q.Questions)
                    .Include(qa => qa.Answers)
                    .ThenInclude(a => a.Question)
                    .FirstOrDefaultAsync(qa => qa.Id == attemptId && qa.UserId == userId);

                if (attempt == null)
                    return NotFound();

                var results = attempt.Answers.Select(a => new
                {
                    QuestionId = a.QuestionId,
                    Question = a.Question.Text,
                    UserAnswers = JsonSerializer.Deserialize<string[]>(a.AnswerJson),
                    CorrectAnswers = JsonSerializer.Deserialize<string[]>(a.Question.CorrectAnswersJson),
                    IsCorrect = a.IsCorrect,
                    Explanation = a.Question.Explanation,
                    PhilosophicalContext = a.Question.PhilosophicalContext,
                    Points = a.PointsEarned,
                    MaxPoints = a.Question.Points
                }).ToList();

                return Ok(new
                {
                    AttemptId = attempt.Id,
                    QuizTitle = attempt.Quiz.Title,
                    Score = attempt.Score,
                    Passed = attempt.Passed,
                    TimeSpent = attempt.TimeSpent,
                    CompletedAt = attempt.CompletedAt,
                    Results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching quiz results for attempt {AttemptId}", attemptId);
                return StatusCode(500, new { message = "Error fetching quiz results" });
            }
        }

        private bool AreAnswersCorrect(string[] userAnswers, string[] correctAnswers)
        {
            if (userAnswers.Length != correctAnswers.Length)
                return false;

            return userAnswers.OrderBy(x => x).SequenceEqual(correctAnswers.OrderBy(x => x));
        }
    }

    public class QuizSubmissionRequest
    {
        public string AttemptId { get; set; } = string.Empty;
        public int TimeSpent { get; set; } // in seconds
        public List<QuestionAnswerRequest> Answers { get; set; } = new();
    }

    public class QuestionAnswerRequest
    {
        public string QuestionId { get; set; } = string.Empty;
        public string[] SelectedAnswers { get; set; } = new string[0];
    }
}