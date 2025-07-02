using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using E_learning.Model.Courses;
using Microsoft.Extensions.Configuration;
namespace E_learning.DAL.Course
{
    public class QuizDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<QuizDAL> _logger;
        public QuizDAL(IConfiguration configuration, ILogger<QuizDAL> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection");
            _logger = logger;
        }
        // Lấy toàn bộ quiz theo CourseID
        public async Task<List<QuizModel>> GetQuizByCourseID(string courseID)
        {
            List<QuizModel> quizzes = new List<QuizModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Quiz WHERE CourseID = @CourseID";
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
        // Xóa một quiz theo QuizID
        public async Task<bool> DeleteQuiz(string quizID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Quiz WHERE QuizID = @QuizID";
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
        // Thêm một quiz mới
        public async Task<bool> insertQuiz(QuizModel quiz)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Quiz (QuizID, QuizTitle, CourseID) VALUES (@QuizID, @QuizTitle, @CourseID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuizID", quiz.getQuizID());
                        command.Parameters.AddWithValue("@QuizTitle", quiz.getQuizTitle());
                        command.Parameters.AddWithValue("@CourseID", quiz.getCourseID());
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting quiz: {QuizTitle}", quiz.getQuizTitle());
                return false;
            }
        }
        // lấy hết Quiz
        public async Task<List<QuizModel>> GetAllQuiz()
        {
            List<QuizModel> quizzes = new List<QuizModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Quiz";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string quizID = reader.GetString(reader.GetOrdinal("QuizID"));
                                string quizTitle = reader.GetString(reader.GetOrdinal("QuizTitle"));
                                string courseID = reader.GetString(reader.GetOrdinal("CourseID"));
                                QuizModel quiz = new QuizModel(quizID, quizTitle, courseID);
                                quizzes.Add(quiz);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all quizzes");
            }
            return quizzes;
        }
    }
}
