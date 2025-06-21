namespace CritiQuest2.Server.Model.DTOs
{
    public class QuizDto
    {
        public string Id { get; set; } = "";
        public string LessonId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Type { get; set; } = "";
        public int? TimeLimit { get; set; }
        public int PassingScore { get; set; }
        public object? PhilosopherBonus { get; set; }
        public List<QuestionDto> Questions { get; set; } = [];
        public List<QuizAttemptDto> UserAttempts { get; set; } = [];
    }

    public class QuestionDto
    {
        public string Id { get; set; } = "";
        public string Text { get; set; } = "";
        public string Type { get; set; } = "";
        public string[] Options { get; set; } = [];
        public string PhilosophicalContext { get; set; } = "";
        public int Points { get; set; }
        public int Order { get; set; }
        public object? DebateConfig { get; set; }
    }

    public class QuizAttemptDto
    {
        public string Id { get; set; } = "";
        public string StartedAt { get; set; } = "";
        public string? CompletedAt { get; set; }
        public int Score { get; set; }
        public int TimeSpent { get; set; }
        public bool Passed { get; set; }
    }

    public class StartAttemptResponse
    {
        public string AttemptId { get; set; } = "";
    }

    public class QuizSubmissionDto
    {
        public string AttemptId { get; set; } = "";
        public int TimeSpent { get; set; }
        public List<AnswerDto> Answers { get; set; } = [];
    }

    public class AnswerDto
    {
        public string QuestionId { get; set; } = "";
        public string[] SelectedAnswers { get; set; } = [];
    }

    public class QuizResultDto
    {
        public string AttemptId { get; set; } = "";
        public int Score { get; set; }
        public bool Passed { get; set; }
        public int EarnedPoints { get; set; }
        public int TotalPoints { get; set; }
        public int TimeSpent { get; set; }
        public List<QuestionResultDto> Results { get; set; } = [];
    }

    public class QuestionResultDto
    {
        public string QuestionId { get; set; } = "";
        public string Question { get; set; } = "";
        public string[] UserAnswers { get; set; } = [];
        public string[] CorrectAnswers { get; set; } = [];
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; } = "";
        public string PhilosophicalContext { get; set; } = "";
        public int Points { get; set; }
        public int MaxPoints { get; set; }
    }
}
