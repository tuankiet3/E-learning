using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using E_learning.Model.Courses;
namespace E_learning.DAL.Course
{
    public class ChoiceDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<ChoiceDAL> _logger;
        public ChoiceDAL(string connectionString, ILogger<ChoiceDAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }
        public async Task<List<ChoiceModel>> GetChoicesByQuizID(string quizID)
        {
            List<ChoiceModel> choices = new List<ChoiceModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Choices WHERE QuizID = @QuizID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuizID", quizID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string choiceID = reader.GetString(reader.GetOrdinal("ChoiceID"));
                                string choiceText = reader.GetString(reader.GetOrdinal("ChoiceText"));
                                bool isCorrect = reader.GetBoolean(reader.GetOrdinal("IsCorrect"));
                                ChoiceModel choice = new ChoiceModel(choiceID, choiceText, isCorrect, quizID);
                                choices.Add(choice);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving choices for quiz ID: {QuizID}", quizID);
            }
            return choices;
        }
        
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

        public async Task<bool> InsertChoice(ChoiceModel choice)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Choices (ChoiceID, ChoiceText, IsCorrect, QuizID) VALUES (@ChoiceID, @ChoiceText, @IsCorrect, @QuizID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ChoiceID", choice.GetChoiceID());
                        command.Parameters.AddWithValue("@ChoiceText", choice.GetChoiceText());
                        command.Parameters.AddWithValue("@IsCorrect", choice.GetIsCorrect());
                        command.Parameters.AddWithValue("@QuizID", choice.GetQuizID());
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
    }
}
