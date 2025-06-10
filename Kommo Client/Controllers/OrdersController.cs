using DataTransferObjects.Orders;
using Kommo_Client.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kommo_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly KommoClient _kommoClient;
        private readonly MarketplaceDb _dbContext;
        public OrdersController(IConfiguration configuration, MarketplaceDb dbContext)
        {
            _kommoClient = new KommoClient(
                configuration["KommoOptions:UserName"],
                configuration["KommoOptions:LongLivedToken"]);

            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(OrderCreationDto orderCreationDto, CancellationToken cancellationToken)
        {
            if (orderCreationDto == null)
            {
                return BadRequest("Order creation data is required.");
            }

            var addLeadResponse = await _kommoClient.AddLeadAsync(
                orderCreationDto.UserName ?? $"Order from {orderCreationDto.PhoneNumber}",
                orderCreationDto.Amount,
                null, // Assuming no contact ID is provided
                cancellationToken: cancellationToken
            );

            var order = new Order
            {
                PhoneNumber = orderCreationDto.PhoneNumber,
                UserName = orderCreationDto.UserName,
                Amount = orderCreationDto.Amount,
                LeadId = addLeadResponse._embedded.leads.FirstOrDefault()?.id ?? 0 // Assuming the first lead ID is used
            };

            await _dbContext.Orders.AddAsync(order, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var orderViewModel = new OrderViewModel
            {
                Id = order.Id,
                PhoneNumber = order.PhoneNumber,
                UserName = order.UserName,
                Amount = order.Amount,
                LeadId = order.LeadId
            };

            return Ok(orderViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var orders = await _dbContext.Orders
                .Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    PhoneNumber = o.PhoneNumber,
                    UserName = o.UserName,
                    Amount = o.Amount,
                    LeadId = o.LeadId
                })
                .ToListAsync(cancellationToken);

            return Ok(orders);
        }
    }
}
