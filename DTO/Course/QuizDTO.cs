using System.ComponentModel.DataAnnotations;

namespace E_learning.DTO.Course
{
    public class QuizDTO
    {
        [Required(ErrorMessage = "Quiz title is required")]
        public string QuizTitle { get; set; }
        [Required(ErrorMessage = "Course ID is required")]
        public string courseID { get; set; }
    }
}
