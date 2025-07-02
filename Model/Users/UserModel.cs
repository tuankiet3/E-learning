using E_learning.Enums;

namespace E_learning.Model.Users
{
    public class UserModel
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public UserRole UserRole { get; set; }
    }
}
