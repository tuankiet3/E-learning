using E_learning.DTO.Zoom;
using E_learning.Enums;
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

        public MeetingController(IZoomService zoomService)
        {
            _zoomService = zoomService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateZoomMeeting([FromBody] CreateMeetingDTO meetingDto)
        {
            try
            {
                // Lấy email của giảng viên đang đăng nhập từ token
                var email = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("User email not found in token.");
                }

                var meetingResponse = await _zoomService.CreateMeetingAsync(meetingDto, email);
                return Ok(meetingResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
