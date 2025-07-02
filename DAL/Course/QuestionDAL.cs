using E_learning.Model.Courses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace E_learning.DAL.Course
{
    public class QuestionDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<QuestionDAL> _logger;
        public QuestionDAL(IConfiguration configuration, ILogger<QuestionDAL> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection");
            _logger = logger;
        }
        public async Task<List<QuestionModel>> getALLQuestion()
        {
            List<QuestionModel> questions = new List<QuestionModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Question ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string questionID = reader.GetString(reader.GetOrdinal("QuestionID"));
                                string questionText = reader.GetString(reader.GetOrdinal("QuestionContent"));
                                string quizID = reader.GetString(reader.GetOrdinal("QuizID"));
                                QuestionModel question = new QuestionModel(questionID, questionText, quizID);
                                questions.Add(question);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions");
            }
            return questions;
        }

        public async Task<bool> InsertQuestion(QuestionModel question)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Question (QuestionID, QuestionContent, QuizID) VALUES (@QuestionID, @QuestionContent, @QuizID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuestionID", question.GetQuestionID);
                        command.Parameters.AddWithValue("@QuestionContent", question.GetQuestionContent);
                        command.Parameters.AddWithValue("@QuizID", question.GetQuizID);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting question");
                return false;
            }
        }

        public async Task<List<QuestionModel>> GetQuestionsByQuizID(string quizID)
        {
            List<QuestionModel> questions = new List<QuestionModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Question WHERE QuizID = @QuizID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuizID", quizID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string questionID = reader.GetString(reader.GetOrdinal("QuestionID"));
                                string questionText = reader.GetString(reader.GetOrdinal("QuestionContent"));
                                QuestionModel question = new QuestionModel(questionID, questionText, quizID);
                                questions.Add(question);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving questions for quiz ID: {QuizID}", quizID);
            }
            return questions;
        }

        public async Task<bool> DeleteQuestion(string questionID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Question WHERE QuestionID = @QuestionID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuestionID", questionID);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question with ID: {QuestionID}", questionID);
                return false;
            }
        }

    }
}
