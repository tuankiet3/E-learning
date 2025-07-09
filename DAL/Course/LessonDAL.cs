using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using E_learning.Model.Courses;
using Microsoft.Extensions.Configuration;
namespace E_learning.DAL.Course
{
    public class LessonDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<LessonDAL> _logger;
        public LessonDAL(IConfiguration configuration, ILogger<LessonDAL> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection");
            _logger = logger;
        }
        // Lấy toàn bộ bài học theo CourseID
        public async Task<List<LessonModel>> GetLessonByCourseID(string courseID)
        {
            List<LessonModel> lessons = new List<LessonModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Lessons WHERE CourseID = @CourseID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CourseID", courseID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string lessonID = reader.GetString(reader.GetOrdinal("LessonID"));
                                string lessonTitle = reader.GetString(reader.GetOrdinal("LessonTitle"));
                                string lessonURL = reader.GetString(reader.GetOrdinal("LessonURL"));
                                LessonModel lesson = new LessonModel(lessonID, lessonTitle, lessonURL, courseID);
                                lessons.Add(lesson);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons for course ID: {CourseID}", courseID);
            }
            return lessons;
        }
        // Xóa một bài học theo LessonID

        public async Task<bool> deleteLesson(string lessonID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Lessons WHERE LessonID = @LessonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LessonID", lessonID);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lessons for course ID: {LessonID}", lessonID);
                return false;
            }
        }
        // Thêm một bài học mới
        public async Task<bool> insertLesson(LessonModel lesson)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Lessons (LessonID, LessonTitle, LessonURL, CourseID) VALUES (@LessonID, @LessonTitle, @LessonURL, @CourseID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LessonID", lesson.GetLessonID());
                        command.Parameters.AddWithValue("@LessonTitle", lesson.GetLessonTitle());
                        command.Parameters.AddWithValue("@LessonURL", lesson.GetLessonURL());
                        command.Parameters.AddWithValue("@CourseID", lesson.GetCourseID());
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting lesson with ID: {LessonID}", lesson.GetLessonID());
                return false;
            }
        }
        // lấy hết lesson
        public async Task<List<string>> GetAllLessonsID()
        {
            List<string> lessons = new List<string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT LessonID FROM Lessons";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string lessonID = reader.GetString(reader.GetOrdinal("LessonID"));
                                
                                lessons.Add(lessonID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all lessons");
            }
            return lessons;
        }
        public async Task<bool> checkBuyCourse(string userID, string lessonID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT COUNT(*) FROM Enrollment WHERE UserID = @UserID AND LessonID = @LessonID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@LessonID", lessonID);
                        int count = (int)await command.ExecuteScalarAsync();
                        return count > 0;
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
