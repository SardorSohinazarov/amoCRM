using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Kommo_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly MarketplaceDb _dbContext;

        public WebhookController(MarketplaceDb dbContext)
        {
            _dbContext = dbContext;
        }

        //[HttpPost("api/webhook/kommo/lead-updated")]
        //public async Task<IActionResult> KommoLeadUpdated([FromBody] KommoWebhookModel model)
        //{

        //    var leadId = model.LeadId;
        //    var newStatusId = model.NewStatusId;

        //    // LeadId orqali tegishli Order topiladi
        //    var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.LeadId == leadId);
        //    if (order != null)
        //    {
        //        order.Status = MapKommoStatusToOrderStatus(newStatusId); // mapping qilinadi
        //        await _dbContext.SaveChangesAsync();
        //    }

        //    return Ok();
        //}
    }
}
