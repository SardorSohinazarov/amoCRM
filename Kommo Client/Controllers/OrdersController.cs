using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kommo_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public OrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(OrderCreationDto orderCreationDto)
        {
            if (orderCreationDto == null)
            {
                return BadRequest("Order creation data is required.");
            }

            var kommoClient = new KommoClient(_configuration["KommoOptions:UserName"], _configuration["KommoOptions:LongLivedToken"]);
            await kommoClient.AddLeadAsync(
                orderCreationDto.UserName ?? $"Order from {orderCreationDto.PhoneNumber}",
                orderCreationDto.Amount,
                null // Assuming no contact ID is provided
            );

            var order = new Order
            {
                Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), // Simulating an ID
                PhoneNumber = orderCreationDto.PhoneNumber,
                UserName = orderCreationDto.UserName,
                Amount = orderCreationDto.Amount
            };

            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                PhoneNumber = order.PhoneNumber,
                UserName = order.UserName,
                Amount = order.Amount
            };

            return CreatedAtAction(nameof(CreateAsync), new { id = order.Id }, orderViewModel);
        }
    }

    public class Order
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderCreationDto
    {
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
    }

    public class OrderViewModel
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public decimal Amount { get; set; }
    }
}
