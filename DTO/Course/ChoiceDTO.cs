using System.ComponentModel.DataAnnotations;

namespace E_learning.DTO.Course
{
    public class ChoiceDTO
    {
        [Required(ErrorMessage = "Choice text is required")]
        public string ChoiceText { get; set; }
        [Required(ErrorMessage = "IsCorrect field is required")]
        public bool IsCorrect { get; set; }
    }
}
