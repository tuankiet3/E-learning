using E_learning.Model.Payment;
using Microsoft.Data.SqlClient;

namespace E_learning.DAL.Payment
{
    public class PaymentDAL
    {
        private readonly string _connectionString;
        private readonly ILogger<PaymentDAL> _logger;
        public PaymentDAL(IConfiguration configuration, ILogger<PaymentDAL> logger)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection");
            _logger = logger;
        }

        public async Task<bool> SavePaymentAsync(PaymentModel paymentModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"INSERT INTO Payments (PaymentID, BuyerName, Description, Amout, CourseID, BuyerID) VALUES 
                    (@PaymentID, @BuyerName, @Description, @Amout, @CourseID, @BuyerID)";

                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PaymentID", paymentModel.GetPaymentID());
                    command.Parameters.AddWithValue("@BuyerName", paymentModel.GetBuyerName());
                    command.Parameters.AddWithValue("@Description", paymentModel.GetDescription());
                    command.Parameters.AddWithValue("@Amout", paymentModel.GetAmount());
                    command.Parameters.AddWithValue("@CourseID", paymentModel.GetCourseId());
                    command.Parameters.AddWithValue("@BuyerID", paymentModel.GetBuyerID());

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
                    var query = "SELECT * FROM Payments";
                    var command = new SqlCommand(query, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string paymentId = reader.GetString(reader.GetOrdinal("PaymentID"));
                            string buyerName = reader.GetString(reader.GetOrdinal("BuyerName"));
                            string description = reader.GetString(reader.GetOrdinal("Description"));
                            decimal amount = reader.GetDecimal(reader.GetOrdinal("Amout"));
                            string courseId = reader.GetString(reader.GetOrdinal("CourseID"));
                            string buyerId = reader.GetString(reader.GetOrdinal("BuyerID"));
                            var payment = new PaymentModel(paymentId, buyerName, description, amount, courseId,buyerId );
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
