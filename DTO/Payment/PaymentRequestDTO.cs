namespace E_learning.DTO.Payment
{
    public class PaymentRequestDTO
    {
        public string OrderType { get; set; } = "other";
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }
        public string BuyerID { get; set; }
        public string courseId { get; set; }
    }
}
