using E_learning.Enums;
using E_learning.Model.Users;
using Microsoft.Data.SqlClient;

namespace E_learning.DAL.Auth
{
    public class AuthDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<AuthDAL> _logger;

        public AuthDAL(string connectionString, ILogger<AuthDAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<UserModel> GetUserByUsername(string username)
        {
            UserModel user = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Users WHERE Username = @Username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                Enum.TryParse<UserRole>(reader.GetString(reader.GetOrdinal("UserRole")), true, out var role);

                                user = new UserModel
                                {
                                    UserID = reader.GetString(reader.GetOrdinal("UserID")),
                                    Username = reader.GetString(reader.GetOrdinal("Username")),
                                    Password = reader.GetString(reader.GetOrdinal("Password")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    UserRole = role
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by username: {Username}", username);
            }
            return user;
        }
        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        int count = (int)await command.ExecuteScalarAsync();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if username exists: {Username}", username);
                return true; 
            }
        }

        public async Task AddUserAsync(UserModel user, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"INSERT INTO Users (UserID, Username, Password, Email, FirstName, LastName, UserRole) 
                             VALUES (@UserID, @Username, @Password, @Email, @FirstName, @LastName, @UserRole)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@UserID", user.UserID);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                // THAY ĐỔI: Chuyển enum thành chuỗi để lưu vào DB
                command.Parameters.AddWithValue("@UserRole", user.UserRole.ToString());
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddStudentAsync(string studentID, string userID, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "INSERT INTO Students (StudentID, UserID) VALUES (@StudentID, @UserID)";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@StudentID", studentID);
                command.Parameters.AddWithValue("@UserID", userID);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddLecturerAsync(string lecturerID, string userID, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "INSERT INTO Lecturers (LecturerID, UserID) VALUES (@LecturerID, @UserID)";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@LecturerID", lecturerID);
                command.Parameters.AddWithValue("@UserID", userID);
                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task AddAdminAsync(string adminID, string userID, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "INSERT INTO Admin (AdminID, UserID) VALUES (@AdminID, @UserID)";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@AdminID", adminID);
                command.Parameters.AddWithValue("@UserID", userID);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
