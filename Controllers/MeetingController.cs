using E_learning.DTO.Zoom;
using E_learning.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_learning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MeetingController : ControllerBase
    {
        private readonly IZoomService _zoomService;
        private readonly ILogger<MeetingController> _logger;

        public MeetingController(IZoomService zoomService, ILogger<MeetingController> logger)
        {
            _zoomService = zoomService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateZoomMeeting([FromBody] CreateMeetingDTO meetingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("User email not found in JWT token.");
                return BadRequest(new { success = false, message = "User email not found in token." });
            }
            if (DateTime.TryParse(meetingDto.StartTime, out var startTime) && startTime <= DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "Meeting start time must be in the future." });
            }

            try
            {
                _logger.LogInformation("Attempting to create Zoom meeting for user {Email}", email);
                var meetingResponse = await _zoomService.CreateMeetingAsync(meetingDto, email);
                _logger.LogInformation("Successfully created Zoom meeting with ID {MeetingId} for user {Email}", meetingResponse?.Id, email);

                return Ok(new { success = true, data = meetingResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while creating a Zoom meeting for {Email}", email);
                return StatusCode(500, new { success = false, message = "An internal server error occurred.", error = ex.Message });
            }
        }
        [HttpGet("test-zoom-token")]
        [AllowAnonymous]
        public async Task<IActionResult> TestZoomToken()
        {
            var result = await _zoomService.TestTokenGenerationAsync();
            if (result.Contains("failed"))
            {
                return StatusCode(500, new { success = false, message = result });
            }
            return Ok(new { success = true, message = result });
        }
    }
}
