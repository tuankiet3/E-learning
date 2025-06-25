using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using E_learning.Model;
using E_learning.Model.Courses;
namespace E_learning.DAL.Course
{
    public class CoursesDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<CoursesDAL> _logger;
        public CoursesDAL(string connectionString, ILogger<CoursesDAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<List<CoursesModel>> getAllCourse()
        {
            List<CoursesModel> courses = new List<CoursesModel>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Courses";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string courseID = reader.GetString(reader.GetOrdinal("CourseID"));
                                string courseName = reader.GetString(reader.GetOrdinal("CourseName"));
                                decimal coursePrice = reader.GetDecimal(reader.GetOrdinal("CoursePrice"));
                                string authorID = reader.GetString(reader.GetOrdinal("AuthorID"));
                                CoursesModel course = new CoursesModel(courseID, courseName, coursePrice, authorID);
                                courses.Add(course);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
            }
            return courses;
        }

        public async Task<bool> InsertCourse(CoursesModel course)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Courses (CourseID, CourseName, CoursePrice, AuthorID) VALUES (@CourseID, @CourseName, @CoursePrice, @AuthorID)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CourseID", course.GetCourseID());
                        command.Parameters.AddWithValue("@CourseName", course.GetCourseName());
                        command.Parameters.AddWithValue("@CoursePrice", course.GetCoursePrice());
                        command.Parameters.AddWithValue("@AuthorID", course.GetAuthorID());
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting course");
                return false;
            }
        }

        public async Task<bool> deleteCourse(string courseID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Courses WHERE CourseID = @CourseID";
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
                _logger.LogError(ex, "Error deleting course");
                return false;
            }
        }

        public async Task<CoursesModel> getCourseByID(string courseID)
        {
            CoursesModel course = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT * FROM Courses WHERE CourseID = @CourseID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CourseID", courseID);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string courseName = reader.GetString(reader.GetOrdinal("CourseName"));
                                decimal coursePrice = reader.GetDecimal(reader.GetOrdinal("CoursePrice"));
                                string authorID = reader.GetString(reader.GetOrdinal("AuthorID"));
                                course = new CoursesModel(courseID, courseName, coursePrice, authorID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course by ID");
            }
            return course;
        }
    }
}
