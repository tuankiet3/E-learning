using E_learning.DAL.Auth;
using E_learning.Model.Users;

namespace E_learning.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDAL _authDAL;

        public AuthRepository(AuthDAL authDAL)
        {
            _authDAL = authDAL;
        }

        public Task<bool> AddUserAsync(UserModel user) => _authDAL.AddUserAsync(user);

        public Task<bool> CheckUsernameExistsAsync(string username) => _authDAL.CheckUsernameExistsAsync(username);

        public Task<UserModel> GetUserByUsernameAsync(string username) => _authDAL.GetUserByUsername(username);
    }
}
