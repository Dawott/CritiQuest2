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

    public class EnhancedQuizResultDto : QuizResultDto
    {
        public QuizRewardsDto? Rewards { get; set; }
        public ProgressionResultDto? Progression { get; set; }
        public string Message { get; set; } = "";
    }

    public class QuizRewardsDto
    {
        public int Experience { get; set; }
        public ExperienceBreakdownDto ExperienceBreakdown { get; set; } = new();
    }

    public class ExperienceBreakdownDto
    {
        public int Base { get; set; }
        public int PointsBonus { get; set; }
        public int PerfectBonus { get; set; }
        public int PassBonus { get; set; }
    }

    public class ProgressionResultDto
    {
        public int ExperienceGained { get; set; }
        public int CurrentLevel { get; set; }
        public bool LeveledUp { get; set; }
        public int NewLevel { get; set; }
        public List<AchievementDto> NewAchievements { get; set; } = new();
    }

    public class AchievementDto
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int RewardExperience { get; set; }
        public int RewardGachaTickets { get; set; }
    }
}
