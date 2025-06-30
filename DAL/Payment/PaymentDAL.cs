using E_learning.Model.Payment;
using Microsoft.Data.SqlClient;

namespace E_learning.DAL.Payment
{
    public class PaymentDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<PaymentDAL> _logger;
        public PaymentDAL(string connectionString, ILogger<PaymentDAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<bool> SavePaymentAsync(PaymentModel paymentModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO Payments (PaymentID, buyerName, Description, Amout, CourseID) VALUES (@PaymentID, @buyerName, @Description, @Amout, @CourseID)";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PaymentID", paymentModel.GetPaymentID);
                    command.Parameters.AddWithValue("@buyerName", paymentModel.GetBuyerName);
                    command.Parameters.AddWithValue("@Description", paymentModel.GetDescription);
                    command.Parameters.AddWithValue("@Amout", paymentModel.GetPaymentID);
                    command.Parameters.AddWithValue("@CourseID", paymentModel.GetCourseId);
                    var result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving payment to database.");
                return false;
            }
        }

        public async Task<List<PaymentModel>> getAllPaymentAsync()
        {
            List<PaymentModel> payments = new List<PaymentModel>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT PaymentID, buyerName, Description, Amout, CourseID FROM Payments";
                    var command = new SqlCommand(query, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string paymentId = reader.GetString(reader.GetOrdinal("PaymentID"));
                            string buyerName = reader.GetString(reader.GetOrdinal("buyerName"));
                            string description = reader.GetString(reader.GetOrdinal("Description"));
                            decimal amount = reader.GetDecimal(reader.GetOrdinal("Amout"));
                            string courseId = reader.GetString(reader.GetOrdinal("CourseID"));
                            var payment = new PaymentModel(paymentId, buyerName, description, amount, courseId);
                            payments.Add(payment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments from database.");
            }
            return payments;
        }

    }
}
