namespace E_learning.DTO.Zoom
{
    public class CreateMeetingDTO
    {
        public string Topic { get; set; }
        public int Type { get; set; } = 2;
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public string Timezone { get; set; } = "Asia/Ho_Chi_Minh";
    }
}
