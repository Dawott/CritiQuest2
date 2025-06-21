using System.ComponentModel.DataAnnotations;

namespace CritiQuest2.Server.Model.Entities
{
    public enum QuestionType
    {
        Single = 1,
        Multiple = 2,
        Scenario = 3,
        Debate = 4
    }

    public enum QuizType
    {
        MultipleChoice = 1,
        Scenario = 2,
        Debate = 3
    }

    public class Quiz
    {
        [MaxLength(100)]
        public string Id { get; set; } = string.Empty; // e.g., "quiz-stoicism-intro"

        [MaxLength(100)]
        public string LessonId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public QuizType Type { get; set; }
        public int? TimeLimit { get; set; } // seconds
        public int PassingScore { get; set; } // percentage

        // Philosopher bonus (JSON serialized)
        public string? PhilosopherBonusJson { get; set; }

        // Navigation properties
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    }

    public class Question
    {
        [MaxLength(100)]
        public string Id { get; set; } = string.Empty;

        [MaxLength(100)]
        public string QuizId { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }
        public string OptionsJson { get; set; } = "[]"; // JSON array
        public string CorrectAnswersJson { get; set; } = "[]"; // JSON array

        [MaxLength(2000)]
        public string Explanation { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string PhilosophicalContext { get; set; } = string.Empty;

        public int Points { get; set; } = 10;
        public int Order { get; set; } = 0;

        // For debate questions - JSON serialized debate config
        public string? DebateConfigJson { get; set; }

        // Navigation properties
        public Quiz Quiz { get; set; } = null!;
    }

    public class QuizAttempt
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;
        [MaxLength(100)]
        public string QuizId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public Quiz Quiz { get; set; } = null!;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public int Score { get; set; } = 0; // percentage
        public int TimeSpent { get; set; } = 0; // seconds
        public bool Passed { get; set; } = false;

        // JSON serialized answers
        public string AnswersJson { get; set; } = "{}";

        // Navigation properties
        public ICollection<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
    }

    public class QuestionAnswer
    {
        [MaxLength(100)]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [MaxLength (100)]
        public string QuizAttemptId { get; set; } = string.Empty;
        [MaxLength(100)]
        public string QuestionId { get; set; } = string.Empty;

        public QuizAttempt QuizAttempt { get; set; } = null!;
        public Question Question { get; set; } = null!;

        public string AnswerJson { get; set; } = "[]"; // JSON array of selected answers
        public bool IsCorrect { get; set; } = false;
        public int PointsEarned { get; set; } = 0;
        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    }

    public class DebateArgument
    {
        [MaxLength(100)]
        public string Id { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        [MaxLength(500)]
        public string PhilosophicalBasis { get; set; } = string.Empty;

        public string StrengthAgainstJson { get; set; } = "[]"; // JSON array
        public string WeaknessAgainstJson { get; set; } = "[]"; // JSON array
        public string SchoolBonusJson { get; set; } = "[]"; // JSON array

        public int ConvictionPower { get; set; } = 50;

        [MaxLength(100)]
        public string? RequiresPhilosopher { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
