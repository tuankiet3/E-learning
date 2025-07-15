using E_learning.Model.Enrollment;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace E_learning.DAL.Enrollment
{
    public class EnrollmentDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<EnrollmentDAL> _logger;
        public EnrollmentDAL(IConfiguration configuration, ILogger<EnrollmentDAL> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection");
            _logger = logger;
        }
        public async Task<bool> InsertEnrollment(EnrollmentModel Enroll)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Enrollments (EnrolmentID, StudentID, CourseID) VALUES (@EnrolmentID, @StudentID, @CourseID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EnrolmentID", Enroll.GetEnrollmentID());
                        command.Parameters.AddWithValue("@StudentID", Enroll.GetUserID());
                        command.Parameters.AddWithValue("@CourseID", Enroll.GetCourseID());
                   
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting enrollment for UserID: {UserID}, CourseID: {CourseID}", Enroll.GetUserID, Enroll.GetCourseID);
                return false;
            }
        }

        public async Task<List<EnrollmentModel>> GetAllEnrollments(int offset, int fetchnext)
        {
            List<EnrollmentModel> enrollments = new List<EnrollmentModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT EnrollmentID, EnrollmentID, CourseID  FROM Enrollments ODER BY EnrollmentID OFFSET @offset ROWS FETCH NEXT @fetchnext ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            command.Parameters.AddWithValue("@offset", offset);
                            command.Parameters.AddWithValue("@fetchnext", fetchnext);
                            while (await reader.ReadAsync())
                            {
                                string enrollmentID = reader.GetString(reader.GetOrdinal("EnrollmentID"));
                                string userID = reader.GetString(reader.GetOrdinal("EnrollmentID"));
                                string courseID = reader.GetString(reader.GetOrdinal("CourseID"));
                                EnrollmentModel enrollment = new EnrollmentModel(enrollmentID, userID, courseID);
                                enrollments.Add(enrollment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments");
            }
            return enrollments;
        }
        public async Task<List<string>> GetAllEnrollmentsID()
        {
            List<string> enrollments = new List<string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT EnrollmentID FROM Enrollments";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string enrollmentID = reader.GetString(reader.GetOrdinal("EnrollmentID"));

                                enrollments.Add(enrollmentID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments");
            }
            return enrollments;
        }
        public async Task<List<EnrollmentModel>> getEnrollbyUserID(string userID)
        {
            List<EnrollmentModel> enrollments = new List<EnrollmentModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Enrollments WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string enrollmentID = reader.GetString(reader.GetOrdinal("EnrollmentID"));
                                string courseID = reader.GetString(reader.GetOrdinal("CourseID"));
                                EnrollmentModel enrollment = new EnrollmentModel(enrollmentID, userID, courseID);
                                enrollments.Add(enrollment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving enrollments for UserID: {UserID}", userID);
            }
            return enrollments;
        }

        public async Task<bool> checkBuyCourse(string userID, string courseID)
        {
            try
            {
                Console.WriteLine("Step 1");
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
           

                    const string query = "SELECT COUNT(*) FROM Enrollments WHERE StudentID = @StudentID AND CourseID = @CourseID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StudentID", userID);
                        command.Parameters.AddWithValue("@CourseID", courseID);

                        int count = (int)await command.ExecuteScalarAsync();
                        Console.WriteLine("count:" +count);
                        if(count > 0)
                        {
                            return true; // User has already bought the course
                        }
                        else
                        {
                            return false; // User has not bought the course
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return false;
            }
        }
    }
}
