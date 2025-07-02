using System.ComponentModel.DataAnnotations;

namespace E_learning.DTO.Course
{
    public class CourseDTO
    {
        [Required(ErrorMessage = "Course name is not null")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "Course price is not null")]
        [Range(0, double.MaxValue, ErrorMessage = "Course price must be positive")]
        public decimal CoursePrice { get; set; }
        [Required(ErrorMessage = "Author is not null")]
        public string Author { get; set; }
        [Required(ErrorMessage = "Course description is not null")]
        public string CourseDescription { get; set; }

    }
}
