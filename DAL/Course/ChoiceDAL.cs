using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using E_learning.Model.Courses;
using Microsoft.Extensions.Configuration;
namespace E_learning.DAL.Course
{
  
    public class ChoiceDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<ChoiceDAL> _logger;

        public ChoiceDAL(IConfiguration configuration, ILogger<ChoiceDAL> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection");
            _logger = logger;
        }
        // lấy toàn bô choices theo quizID
        public async Task<List<ChoiceModel>> GetChoicesByQuizID(string QuestionID)
        {
            List<ChoiceModel> choices = new List<ChoiceModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Choices WHERE QuestionID = @QuestionID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuestionID", QuestionID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string choiceID = reader.GetString(reader.GetOrdinal("ChoiceID"));
                                string choiceText = reader.GetString(reader.GetOrdinal("ChoiceText"));
                                bool isCorrect = reader.GetBoolean(reader.GetOrdinal("IsCorrect"));
                                ChoiceModel choice = new ChoiceModel(choiceID, choiceText, isCorrect, QuestionID);
                                choices.Add(choice);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving choices for quiz ID: {QuestionID}", QuestionID);
            }
            return choices;
        }
        // xóa một choice theo choiceID
        public async Task<bool> deleteChoice(string choiceID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Choices WHERE ChoiceID = @ChoiceID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ChoiceID", choiceID);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting choice with ID: {ChoiceID}", choiceID);
                return false;
            }
        }
        // thêm một choice mới
        public async Task<bool> InsertChoice(ChoiceModel choice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Choices (ChoiceID, ChoiceText, IsCorrect, QuestionID) VALUES (@ChoiceID, @ChoiceText, @IsCorrect, @QuestionID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ChoiceID", choice.GetChoiceID());
                        command.Parameters.AddWithValue("@ChoiceText", choice.GetChoiceText());
                        command.Parameters.AddWithValue("@IsCorrect", choice.GetIsCorrect());
                        command.Parameters.AddWithValue("@QuestionID", choice.getQuestionID());
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting choice");
                return false;
            }
        }
        // lấy hết choices
        public async Task<List<string>> getAllChoiceID()
        {
            List<string> choices = new List<string>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT ChoiceID FROM Choices";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string choiceID = reader.GetString(reader.GetOrdinal("ChoiceID"));
                                choices.Add(choiceID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all choices");
            }
            return choices;
        }
    }
}
