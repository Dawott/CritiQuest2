using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using CritiQuest2.Server.Data;
using CritiQuest2.Server.Model.Entities;
using CritiQuest2.Server.Model.DTOs;

namespace CritiQuest2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InteractionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InteractionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("responses")]
        public async Task<ActionResult<InteractionResponseDto>> SaveResponse([FromBody] SaveInteractionRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Check if response already exists
            var existing = await _context.LessonInteractionResponses
                .FirstOrDefaultAsync(r => r.UserId == userId &&
                                         r.LessonId == request.LessonId &&
                                         r.SectionId == request.SectionId);

            if (existing != null)
            {
                // Update existing response
                existing.ResponseDataJson = JsonSerializer.Serialize(request.ResponseData);
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new response
                var response = new LessonInteractionResponse
                {
                    UserId = userId,
                    LessonId = request.LessonId,
                    SectionId = request.SectionId,
                    InteractionType = Enum.Parse<InteractionType>(request.InteractionType, true),
                    ResponseDataJson = JsonSerializer.Serialize(request.ResponseData),
                    CreatedAt = DateTime.UtcNow
                };

                _context.LessonInteractionResponses.Add(response);
            }

            await _context.SaveChangesAsync();

            var savedResponse = existing ?? await _context.LessonInteractionResponses
                .FirstAsync(r => r.UserId == userId &&
                               r.LessonId == request.LessonId &&
                               r.SectionId == request.SectionId);

            return Ok(new InteractionResponseDto
            {
                Id = savedResponse.Id,
                LessonId = savedResponse.LessonId,
                SectionId = savedResponse.SectionId,
                InteractionType = savedResponse.InteractionType.ToString(),
                ResponseData = JsonSerializer.Deserialize<object>(savedResponse.ResponseDataJson),
                CreatedAt = savedResponse.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = savedResponse.UpdatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpGet("responses/{lessonId}/{sectionId}")]
        public async Task<ActionResult<InteractionResponseDto>> GetResponse(string lessonId, string sectionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _context.LessonInteractionResponses
                .FirstOrDefaultAsync(r => r.UserId == userId &&
                                         r.LessonId == lessonId &&
                                         r.SectionId == sectionId);

            if (response == null)
                return NotFound();

            return Ok(new InteractionResponseDto
            {
                Id = response.Id,
                LessonId = response.LessonId,
                SectionId = response.SectionId,
                InteractionType = response.InteractionType.ToString(),
                ResponseData = JsonSerializer.Deserialize<object>(response.ResponseDataJson),
                CreatedAt = response.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = response.UpdatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpGet("lessons/{lessonId}/responses")]
        public async Task<ActionResult<List<InteractionResponseDto>>> GetLessonResponses(string lessonId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var responses = await _context.LessonInteractionResponses
                .Where(r => r.UserId == userId && r.LessonId == lessonId)
                .ToListAsync();

            var result = responses.Select(r => new InteractionResponseDto
            {
                Id = r.Id,
                LessonId = r.LessonId,
                SectionId = r.SectionId,
                InteractionType = r.InteractionType.ToString(),
                ResponseData = JsonSerializer.Deserialize<object>(r.ResponseDataJson),
                CreatedAt = r.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                UpdatedAt = r.UpdatedAt?.ToString("yyyy-MM-ddTHH:mm:ssZ")
            }).ToList();

            return Ok(result);
        }

        [HttpDelete("responses/{lessonId}/{sectionId}")]
        public async Task<ActionResult> DeleteResponse(string lessonId, string sectionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _context.LessonInteractionResponses
                .FirstOrDefaultAsync(r => r.UserId == userId &&
                                         r.LessonId == lessonId &&
                                         r.SectionId == sectionId);

            if (response == null)
                return NotFound();

            _context.LessonInteractionResponses.Remove(response);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}