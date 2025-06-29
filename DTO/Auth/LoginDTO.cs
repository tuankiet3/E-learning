using System.ComponentModel.DataAnnotations;

namespace E_learning.DTO.Auth
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
