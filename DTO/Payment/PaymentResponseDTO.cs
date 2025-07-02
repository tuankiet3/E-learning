namespace E_learning.DTO.Payment
{
    public class PaymentResponseDTO
    {
        public string OrderInfo { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string Message { get; set; } // Add this line
    }
}
