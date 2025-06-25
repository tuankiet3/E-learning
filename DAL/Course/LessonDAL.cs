using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using E_learning.Model.Courses;
namespace E_learning.DAL.Course
{
    public class LessonDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<LessonDAL> _logger;
        public LessonDAL(string connectionString, ILogger<LessonDAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }
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

        public async Task<bool> deleteLesson(string courseID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Lessons WHERE CourseID = @CourseID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CourseID", courseID);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lessons for course ID: {CourseID}", courseID);
                return false;
            }
        }
    }
}
