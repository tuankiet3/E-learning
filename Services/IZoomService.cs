using E_learning.DTO.Zoom;
using E_learning.Model.Zoom;

namespace E_learning.Services
{
    public interface IZoomService
    {
        Task<ZoomMeetingResponse> CreateMeetingAsync(CreateMeetingDTO meetingDto, string email);
        Task<string> TestTokenGenerationAsync();
    }
}