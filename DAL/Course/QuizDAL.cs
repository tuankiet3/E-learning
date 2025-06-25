using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using E_learning.Model.Courses;
namespace E_learning.DAL.Course
{
    public class QuizDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<QuizDAL> _logger;
        public QuizDAL(string connectionString, ILogger<QuizDAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }
        public async Task<List<QuizModel>> GetQuizByCourseID(string courseID)
        {
            List<QuizModel> quizzes = new List<QuizModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Quizzes WHERE CourseID = @CourseID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CourseID", courseID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string quizID = reader.GetString(reader.GetOrdinal("QuizID"));
                                string quizTitle = reader.GetString(reader.GetOrdinal("QuizTitle"));
                                QuizModel quiz = new QuizModel(quizID, quizTitle,courseID);
                                quizzes.Add(quiz);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving quizzes for course ID: {CourseID}", courseID);
            }
            return quizzes;
        }

        public async Task<bool> DeleteQuiz(string quizID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Quizzes WHERE QuizID = @QuizID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuizID", quizID);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz with ID: {QuizID}", quizID);
                return false;
            }
        }
    }
}
