using E_learning.Model.Payment;

namespace E_learning.Repositories.Payment
{
    public interface IPaymentRepository
    {
        Task<bool> SavePaymentAsync(PaymentModel paymentModel);
        Task<List<PaymentModel>> getAllPaymentAsync();
        Task<List<string>> getAllPaymentIDAsync();
    }
}
