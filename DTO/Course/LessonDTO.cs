using System.ComponentModel.DataAnnotations;

namespace E_learning.DTO.Course
{
    public class LessonDTO
    {
        [Required(ErrorMessage = "Lesson title is required")]
        public string lessonTitle { get; set; }
        [Required(ErrorMessage = "Lesson URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string lessonURL { get; set; }
        [Required(ErrorMessage = "Course ID is required")]
        public string courseID { get; set; }
    }
}
