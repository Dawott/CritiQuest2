using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Model;
using CritiQuest2.Server.Model.DTOs;

namespace CritiQuest2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InteractiveScenariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InteractiveScenariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all interactive sections for a lesson
        [HttpGet("lessons/{lessonId}/sections")]
        public async Task<ActionResult<List<InteractiveSectionDto>>> GetLessonInteractiveSections(string lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sections = await _context.InteractiveSections
                .Where(s => s.LessonId == lessonId)
                .OrderBy(s => s.OrderInLesson)
                .ToListAsync();

            var result = new List<InteractiveSectionDto>();

            foreach (var section in sections)
            {
                // Get user's response if exists
                var userResponse = await _context.UserInteractionResponses
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.InteractiveSectionId == section.Id);

                result.Add(new InteractiveSectionDto
                {
                    Id = section.Id,
                    LessonId = section.LessonId,
                    Title = section.Title,
                    Description = section.Description,
                    Type = section.Type.ToString(),
                    OrderInLesson = section.OrderInLesson,
                    Configuration = JsonSerializer.Deserialize<object>(section.ConfigurationJson),
                    IsRequired = section.IsRequired,
                    EstimatedTimeMinutes = section.EstimatedTimeMinutes,
                    UserResponse = userResponse != null ? new UserResponseDto
                    {
                        Id = userResponse.Id,
                        ResponseData = JsonSerializer.Deserialize<object>(userResponse.ResponseDataJson),
                        TimeSpentSeconds = userResponse.TimeSpentSeconds,
                        IsCompleted = userResponse.IsCompleted,
                        CompletionPercentage = userResponse.CompletionPercentage,
                        QualityScore = userResponse.QualityScore,
                        LastUpdatedAt = userResponse.LastUpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    } : null
                });
            }

            return Ok(result);
        }

        // Get specific interactive section
        [HttpGet("sections/{sectionId}")]
        public async Task<ActionResult<InteractiveSectionDto>> GetInteractiveSection(string sectionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var section = await _context.InteractiveSections.FindAsync(sectionId);
            if (section == null)
                return NotFound();

            var userResponse = await _context.UserInteractionResponses
                .FirstOrDefaultAsync(r => r.UserId == userId && r.InteractiveSectionId == sectionId);

            return Ok(new InteractiveSectionDto
            {
                Id = section.Id,
                LessonId = section.LessonId,
                Title = section.Title,
                Description = section.Description,
                Type = section.Type.ToString(),
                OrderInLesson = section.OrderInLesson,
                Configuration = JsonSerializer.Deserialize<object>(section.ConfigurationJson),
                IsRequired = section.IsRequired,
                EstimatedTimeMinutes = section.EstimatedTimeMinutes,
                UserResponse = userResponse != null ? new UserResponseDto
                {
                    Id = userResponse.Id,
                    ResponseData = JsonSerializer.Deserialize<object>(userResponse.ResponseDataJson),
                    TimeSpentSeconds = userResponse.TimeSpentSeconds,
                    IsCompleted = userResponse.IsCompleted,
                    CompletionPercentage = userResponse.CompletionPercentage,
                    QualityScore = userResponse.QualityScore,
                    LastUpdatedAt = userResponse.LastUpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                } : null
            });
        }

        // Save or update user response
        [HttpPost("sections/{sectionId}/responses")]
        public async Task<ActionResult<UserResponseDto>> SaveResponse(string sectionId, [FromBody] SaveResponseRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var section = await _context.InteractiveSections.FindAsync(sectionId);
            if (section == null)
                return NotFound();

            // Calculate quality score based on response type and content
            var qualityScore = CalculateQualityScore(section.Type, request.ResponseData);
            var completionPercentage = CalculateCompletionPercentage(section.Type, request.ResponseData);

            var existingResponse = await _context.UserInteractionResponses
                .FirstOrDefaultAsync(r => r.UserId == userId && r.InteractiveSectionId == sectionId);

            if (existingResponse != null)
            {
                // Update existing response
                existingResponse.ResponseDataJson = JsonSerializer.Serialize(request.ResponseData);
                existingResponse.TimeSpentSeconds = request.TimeSpentSeconds;
                existingResponse.IsCompleted = request.IsCompleted;
                existingResponse.CompletionPercentage = completionPercentage;
                existingResponse.QualityScore = qualityScore;
                existingResponse.LastUpdatedAt = DateTime.UtcNow;

                if (request.IsCompleted && existingResponse.CompletedAt == null)
                {
                    existingResponse.CompletedAt = DateTime.UtcNow;
                }
            }
            else
            {
                // Create new response
                existingResponse = new UserInteractionResponse
                {
                    UserId = userId,
                    InteractiveSectionId = sectionId,
                    ResponseDataJson = JsonSerializer.Serialize(request.ResponseData),
                    TimeSpentSeconds = request.TimeSpentSeconds,
                    IsCompleted = request.IsCompleted,
                    CompletionPercentage = completionPercentage,
                    QualityScore = qualityScore,
                    StartedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    CompletedAt = request.IsCompleted ? DateTime.UtcNow : null
                };

                _context.UserInteractionResponses.Add(existingResponse);
            }

            // Update overall lesson progress
            await UpdateInteractionProgress(userId, section.LessonId);

            await _context.SaveChangesAsync();

            return Ok(new UserResponseDto
            {
                Id = existingResponse.Id,
                ResponseData = JsonSerializer.Deserialize<object>(existingResponse.ResponseDataJson),
                TimeSpentSeconds = existingResponse.TimeSpentSeconds,
                IsCompleted = existingResponse.IsCompleted,
                CompletionPercentage = existingResponse.CompletionPercentage,
                QualityScore = existingResponse.QualityScore,
                LastUpdatedAt = existingResponse.LastUpdatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        // Get lesson interaction progress
        [HttpGet("lessons/{lessonId}/progress")]
        public async Task<ActionResult<InteractionProgressDto>> GetLessonProgress(string lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var progress = await _context.InteractionProgress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

            if (progress == null)
            {
                // Calculate progress if not exists
                await UpdateInteractionProgress(userId, lessonId);
                progress = await _context.InteractionProgress
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);
            }

            if (progress == null)
                return Ok(new InteractionProgressDto()); // Empty progress

            return Ok(new InteractionProgressDto
            {
                LessonId = progress.LessonId,
                TotalSections = progress.TotalInteractiveSections,
                CompletedSections = progress.CompletedSections,
                CompletionPercentage = progress.TotalInteractiveSections > 0
                    ? (progress.CompletedSections * 100) / progress.TotalInteractiveSections
                    : 0,
                TotalTimeSpentSeconds = progress.TotalTimeSpentSeconds,
                LastActivityAt = progress.LastActivityAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        // Private helper methods
        private async Task UpdateInteractionProgress(string userId, string lessonId)
        {
            var totalSections = await _context.InteractiveSections
                .CountAsync(s => s.LessonId == lessonId);

            var completedSections = await _context.UserInteractionResponses
                .Include(r => r.InteractiveSection)
                .CountAsync(r => r.UserId == userId &&
                               r.InteractiveSection.LessonId == lessonId &&
                               r.IsCompleted);

            var totalTimeSpent = await _context.UserInteractionResponses
                .Include(r => r.InteractiveSection)
                .Where(r => r.UserId == userId && r.InteractiveSection.LessonId == lessonId)
                .SumAsync(r => r.TimeSpentSeconds);

            var existingProgress = await _context.InteractionProgress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

            if (existingProgress != null)
            {
                existingProgress.TotalInteractiveSections = totalSections;
                existingProgress.CompletedSections = completedSections;
                existingProgress.TotalTimeSpentSeconds = totalTimeSpent;
                existingProgress.LastActivityAt = DateTime.UtcNow;
            }
            else
            {
                var newProgress = new InteractionProgress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    TotalInteractiveSections = totalSections,
                    CompletedSections = completedSections,
                    TotalTimeSpentSeconds = totalTimeSpent,
                    LastActivityAt = DateTime.UtcNow
                };

                _context.InteractionProgress.Add(newProgress);
            }
        }

        private int CalculateQualityScore(InteractionType type, object responseData)
        {
            try
            {
                var json = JsonSerializer.Serialize(responseData);

                return type switch
                {
                    InteractionType.Reflection => CalculateReflectionQuality(json),
                    InteractionType.ConceptMap => CalculateConceptMapQuality(json),
                    InteractionType.Timeline => CalculateTimelineQuality(json),
                    InteractionType.Comparison => CalculateComparisonQuality(json),
                    _ => 50
                };
            }
            catch
            {
                return 0;
            }
        }

        private int CalculateCompletionPercentage(InteractionType type, object responseData)
        {
            try
            {
                var json = JsonSerializer.Serialize(responseData);

                return type switch
                {
                    InteractionType.Reflection => CalculateReflectionCompletion(json),
                    InteractionType.ConceptMap => CalculateConceptMapCompletion(json),
                    InteractionType.Timeline => CalculateTimelineCompletion(json),
                    InteractionType.Comparison => CalculateComparisonCompletion(json),
                    _ => 0
                };
            }
            catch
            {
                return 0;
            }
        }

        private int CalculateReflectionQuality(string json)
        {
            try
            {
                var response = JsonSerializer.Deserialize<ReflectionResponse>(json);
                if (response?.Responses == null) return 0;

                var avgLength = response.Responses
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Select(r => r.Length)
                    .DefaultIfEmpty(0)
                    .Average();

                return avgLength switch
                {
                    > 200 => 100,
                    > 150 => 80,
                    > 100 => 60,
                    > 50 => 40,
                    > 20 => 20,
                    _ => 10
                };
            }
            catch
            {
                return 0;
            }
        }

        private int CalculateReflectionCompletion(string json)
        {
            try
            {
                var response = JsonSerializer.Deserialize<ReflectionResponse>(json);
                if (response?.Responses == null) return 0;

                var completedPrompts = response.Responses.Count(r => !string.IsNullOrWhiteSpace(r));
                var totalPrompts = response.Responses.Count;

                return totalPrompts > 0 ? (completedPrompts * 100) / totalPrompts : 0;
            }
            catch
            {
                return 0;
            }
        }

        private int CalculateConceptMapQuality(string json) => 75; // Placeholder
        private int CalculateConceptMapCompletion(string json) => 50; // Placeholder
        private int CalculateTimelineQuality(string json) => 75; // Placeholder
        private int CalculateTimelineCompletion(string json) => 50; // Placeholder
        private int CalculateComparisonQuality(string json) => 75; // Placeholder
        private int CalculateComparisonCompletion(string json) => 50; // Placeholder
    }

   
}