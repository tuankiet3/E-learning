namespace E_learning.Model.Payment
{
    public class PaymentModel
    {
        private string PaymentID { get; set; }
        private string BuyerName { get; set; }
        private string Description { get; set; }
        private decimal Amount { get; set; }

        private string courseId { get; set; }

        public string GetPaymentID()
        {
            return PaymentID;
        }
        public void SetPaymentID(string paymentId)
        {
            PaymentID = paymentId;
        }
        public string GetBuyerName()
        {
            return BuyerName;
        }
        public void SetBuyerName(string buyerName)
        {
            BuyerName = buyerName;
        }
        public string GetDescription()
        {
            return Description;
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
        public decimal GetAmount()
        {
            return Amount;
        }
        public void SetAmount(decimal amount)
        {
            Amount = amount;
        }
        public string GetCourseId()
        {
            return courseId;
        }
        public void SetCourseId(string courseId)
        {
            this.courseId = courseId;
        }
        public PaymentModel(string paymentId, string buyerName, string description, decimal amount, string courseId)
        {
            PaymentID = paymentId;
            BuyerName = buyerName;
            Description = description;
            Amount = amount;
            this.courseId = courseId;
        }
    }
}
