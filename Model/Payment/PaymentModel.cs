namespace E_learning.Model.Payment
{
    public class PaymentModel
    {
        private string PaymentID { get; set; }
        private string BuyerName { get; set; }
        private string Description { get; set; }
        private decimal Amount { get; set; }
        private string BuyerID { get; set; }
        private string CourseId { get; set; }

        // Public properties to expose data
        public string PaymentId => PaymentID;
        public string buyerName => BuyerName;
        public string description => Description;
        public decimal amount=> Amount;
        public string BuyerId => BuyerID;
        public string CourseID => CourseId;
        // Getter and Setter methods for encapsulation

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
            return CourseId;
        }
        public void SetCourseId(string courseId)
        {
            CourseId = courseId;
        }
        public string GetBuyerID()
        {
            return BuyerID;
        }
        public void SetBuyerID(string buyerId)
        {
            BuyerID = buyerId;
        }
        public PaymentModel(string paymentId, string buyerName, string description, decimal amount, string courseId, string buyerId)
        {
            PaymentID = paymentId;
            BuyerName = buyerName;
            Description = description;
            Amount = amount;
            CourseId = courseId;
            BuyerID = buyerId;
        }
    }
}
