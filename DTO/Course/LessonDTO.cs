using System.ComponentModel.DataAnnotations;

namespace E_learning.DTO.Course
{
    public class LessonDTO
    {
        [Required(ErrorMessage = "Lesson title is required")]
        public string lessonTitle { get; set; }
        public string courseID { get; set; }
        public IFormFile videoFile { get; set; }
    }
}
