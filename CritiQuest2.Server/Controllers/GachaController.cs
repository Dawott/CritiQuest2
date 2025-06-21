using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CritiQuest2.Server.Services;
using CritiQuest2.Server.DTOs;

namespace CritiQuest2.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GachaController : ControllerBase
    {
        private readonly IGachaService _gachaService;
        private readonly ILogger<GachaController> _logger;

        public GachaController(IGachaService gachaService, ILogger<GachaController> logger)
        {
            _gachaService = gachaService;
            _logger = logger;
        }

        [HttpPost("summon")]
        public async Task<ActionResult<GachaSummonResponse>> PerformSummon([FromBody] GachaSummonRequest request)
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var result = await _gachaService.PerformSummonAsync(userId, request.TicketCount);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing gacha summon for user {UserId}", User.Identity?.Name);
                return StatusCode(500, "An error occurred while performing the summon");
            }
        }

        [HttpGet("rates")]
        public ActionResult<GachaRatesResponse> GetGachaRates()
        {
            var rates = _gachaService.GetGachaRates();
            return Ok(rates);
        }

        [HttpGet("preview")]
        public async Task<ActionResult<GachaPreviewResponse>> GetGachaPreview()
        {
            try
            {
                var userId = User.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var preview = await _gachaService.GetGachaPreviewAsync(userId);
                return Ok(preview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gacha preview for user {UserId}", User.Identity?.Name);
                return StatusCode(500, "An error occurred while getting gacha preview");
            }
        }
    }
}