using E_learning.Model.Payment;
using E_learning.Services.VNPay;
using Microsoft.AspNetCore.Mvc;

namespace E_learning.Controllers.Payment
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly VnPayService _vnPayService;

        public PaymentController(VnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] PaymentRequestModel model)
        {
            if (model == null || model.Amount <= 0)
                return BadRequest("Invalid payment information.");

            try
            {
                var paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);

                return Ok(new
                {
                    success = true,
                    paymentUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("vnpay-return")]
        public IActionResult VNPayReturn()
        {
            var response = _vnPayService.PaymentExecute(HttpContext.Request.Query);
            Console.WriteLine("Response: " + response.OrderInfo);

            if (response.Success)
            {
               
                return Redirect("https://localhost:7173/swagger/index.html");
            }
            else
            {
               
                return Redirect("https://localhost:7188/swagger/payment-failed");
            }
        }
    }
}