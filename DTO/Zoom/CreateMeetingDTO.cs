using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Add this line

namespace E_learning.DTO.Zoom
{
    public class CreateMeetingDTO
    {
        [Required(ErrorMessage = "Topic is required")]
        [StringLength(200, ErrorMessage = "Topic cannot exceed 200 characters")]
        public string Topic { get; set; }

        [Range(1, 8, ErrorMessage = "Type must be between 1 and 8")]
        public int Type { get; set; } = 2;

        [Required(ErrorMessage = "Start time is required")]
        public string StartTime { get; set; }

        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes")]
        public int Duration { get; set; }

        public string Timezone { get; set; } = "Asia/Ho_Chi_Minh";

        public string? Agenda { get; set; }

        // Add the [JsonPropertyName] attribute to ensure correct serialization
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}