namespace E_learning.Model.Zoom
{
    public class ZoomMeetingResponse
    {
        public long Id { get; set; }
        public string Topic { get; set; }
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public string JoinUrl { get; set; }
        public string Password { get; set; }
    }
}
