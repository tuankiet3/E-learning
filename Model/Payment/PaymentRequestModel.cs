namespace E_learning.Model.Payment
{
    public class PaymentRequestModel
    {
        public string OrderType { get; set; } = "other";
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
    }
}
