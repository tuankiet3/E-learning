using E_learning.DAL.Payment;
using E_learning.Model.Payment;

namespace E_learning.Repositories.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAL _paymentDAL;
        public PaymentRepository(PaymentDAL paymentDAL)
        {
            _paymentDAL = paymentDAL;
        }

        public Task<List<PaymentModel>> getAllPaymentAsync()
        {
           return _paymentDAL.getAllPaymentAsync();
        }

        public Task<bool> SavePaymentAsync(PaymentModel paymentModel)
        {
            return _paymentDAL.SavePaymentAsync(paymentModel);
        }
        public Task<List<string>> getAllPaymentIDAsync()
        {
            return _paymentDAL.getAllPaymentIDAsync();
        }
    }
}
