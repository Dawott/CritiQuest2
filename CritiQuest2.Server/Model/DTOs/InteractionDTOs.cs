namespace CritiQuest2.Server.Model.DTOs
{
    public class SaveInteractionRequest
    {
        public string LessonId { get; set; } = "";
        public string SectionId { get; set; } = "";
        public string InteractionType { get; set; } = "";
        public object ResponseData { get; set; } = new();
    }

    public class InteractionResponseDto
    {
        public string Id { get; set; } = "";
        public string LessonId { get; set; } = "";
        public string SectionId { get; set; } = "";
        public string InteractionType { get; set; } = "";
        public object? ResponseData { get; set; }
        public string CreatedAt { get; set; } = "";
        public string? UpdatedAt { get; set; }
    }
}
