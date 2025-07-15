using E_learning.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_learning.DTO.Auth
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "User role is required")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid User Role. Valid roles are Student, Lecturer, Admin.")]
        public string UserRole { get; set; } = "Student";
    }
}
