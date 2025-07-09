using E_learning.DTO.Zoom;
using E_learning.Services.Zoom;
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
        private readonly IConfiguration _configuration;

        public MeetingController(IZoomService zoomService, ILogger<MeetingController> logger, IConfiguration configuration)
        {
            _zoomService = zoomService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateZoomMeeting([FromBody] CreateMeetingDTO meetingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var defaultEmail = _configuration["Zoom:DefaultMeetingUserEmail"];
            if (string.IsNullOrEmpty(defaultEmail))
            {
                _logger.LogError("Default Zoom user email is not configured.");
                return StatusCode(500, new { success = false, message = "Lỗi cấu hình máy chủ." });
            }

            if (DateTime.TryParse(meetingDto.StartTime, out var startTime) && startTime <= DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "Thời gian bắt đầu cuộc họp phải ở tương lai." });
            }

            try
            {
                _logger.LogInformation("Đang cố gắng tạo cuộc họp Zoom cho người dùng mặc định {Email}", defaultEmail);
                var meetingResponse = await _zoomService.CreateMeetingAsync(meetingDto, defaultEmail);
                _logger.LogInformation("Đã tạo thành công cuộc họp Zoom với ID {MeetingId} cho người dùng mặc định {Email}", meetingResponse?.Id, defaultEmail);

                return Ok(new { success = true, data = meetingResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi không xác định khi tạo cuộc họp Zoom cho người dùng mặc định {Email}", defaultEmail);
                return StatusCode(500, new { success = false, message = "Đã xảy ra lỗi nội bộ máy chủ.", error = ex.Message });
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
