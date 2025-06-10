using Kommo_Client.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Kommo_Client.Controllers
{
    [Route("api/[controller]/kommo")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly MarketplaceDb _dbContext;
        private readonly KommoClient _kommoClient;

        public WebhookController(MarketplaceDb dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _kommoClient = new KommoClient(
                configuration["KommoOptions:UserName"],
                configuration["KommoOptions:LongLivedToken"]);
        }

        [HttpPost("leads")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Receive(CancellationToken cancellationToken)
        {
            var form = await Request.ReadFormAsync(cancellationToken);

            await AddLeadAsync(ParseLeads<Add>(form, "add"), cancellationToken);
            await DeleteLeadAsync(ParseLeads<Delete>(form, "delete"), cancellationToken);
            //await UpdateLeadAsync(ParseLeads<Update>(form, "update"), cancellationToken);
            //await UpdateStatusAsync(ParseLeads<Status>(form, "status"), cancellationToken);

            return Ok();
        }

        //private async Task UpdateStatusAsync(List<Status> statuses, CancellationToken cancellationToken)
        //{
        //    //var pipelines = await _kommoClient.GetPipelinesAsync(cancellationToken);
        //    foreach (var status in statuses)
        //    {
        //        var order = await _dbContext.Orders
        //            .FirstOrDefaultAsync(l => l.Id == long.Parse(status.id), cancellationToken);

        //        if (Enum.TryParse(typeof(EOrderStatus), status.name, out object newStatus))
        //        {
        //            order.Status = (EOrderStatus)newStatus;
        //        }
        //    }

        //    await _dbContext.SaveChangesAsync(cancellationToken);
        //}

        private async Task DeleteLeadAsync(List<Delete> deletes, CancellationToken cancellationToken)
        {
            foreach (var delete in deletes)
            {
                var order = await _dbContext.Orders
                    .FirstOrDefaultAsync(l => l.LeadId == long.Parse(delete.id), cancellationToken);

                if (order != null)
                {
                    _dbContext.Orders.Remove(order);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateLeadAsync(List<Update> updates, CancellationToken cancellationToken)
        {
            var pipelines = await _kommoClient.GetPipelinesAsync(cancellationToken);
            foreach (var update in updates)
            {
                var pipeLine = pipelines
                    .FirstOrDefault(p => p.id == long.Parse(update.pipeline_id));

                var order = await _dbContext.Orders
                    .FirstOrDefaultAsync(l => l.LeadId == long.Parse(update.id), cancellationToken);

                if (Enum.TryParse(typeof(EOrderStatus), pipeLine.name, out object newStatus))
                {
                    order.Status = (EOrderStatus)newStatus;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task AddLeadAsync(List<Add> adds, CancellationToken cancellationToken = default)
        {
            foreach (var add in adds)
            {
                var order = new Order
                {
                    LeadId = long.Parse(add.id),
                    Amount = decimal.Parse(add.price),
                    PhoneNumber = "998912040618",
                    UserName = "Sardor",
                    Status = EOrderStatus.New,
                };

                _dbContext.Orders.Add(order);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private List<T> ParseLeads<T>(IFormCollection form, string sectionName) where T : new()
        {
            var results = new List<T>();

            var keys = form.Keys
                .Where(k => k.StartsWith($"leads[{sectionName}]"))
                .ToList();

            var indexes = keys
                .Select(k => k.Split(new[] { $"leads[{sectionName}][" }, StringSplitOptions.None)[1].Split(']')[0])
                .Distinct();

            foreach (var index in indexes)
            {
                var obj = new T();
                var properties = typeof(T).GetProperties();

                foreach (var prop in properties)
                {
                    var key = $"leads[{sectionName}][{index}][{prop.Name}]";
                    if (form.TryGetValue(key, out var value))
                    {
                        if (prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(obj, value.ToString());
                        }
                        else if (prop.PropertyType == typeof(List<CustomField>))
                        {
                            // optional: handle complex types
                            prop.SetValue(obj, new List<CustomField>());
                        }
                    }
                }

                results.Add(obj);
            }

            return results;
        }
    }

    public class Add
    {
        public string id { get; set; }
        public string name { get; set; }
        public string status_id { get; set; }
        public string price { get; set; }
        public string responsible_user_id { get; set; }
        public string last_modified { get; set; }
        public string modified_user_id { get; set; }
        public string created_user_id { get; set; }
        public string date_create { get; set; }
        public string pipeline_id { get; set; }
        public string account_id { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class CustomField
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<object> values { get; set; }
    }

    public class Leads
    {
        public List<Add> add { get; set; }
        public List<Update> update { get; set; }
        public List<Delete> delete { get; set; }
        public List<Status> status { get; set; }
    }

    public class Root
    {
        public Leads leads { get; set; }
    }

    public class Update
    {
        public string id { get; set; }
        public string name { get; set; }
        public string status_id { get; set; }
        public string old_status_id { get; set; }
        public string price { get; set; }
        public string responsible_user_id { get; set; }
        public string last_modified { get; set; }
        public string modified_user_id { get; set; }
        public string created_user_id { get; set; }
        public string date_create { get; set; }
        public string pipeline_id { get; set; }
        public string account_id { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }

    public class Delete
    {
        public string id { get; set; }
        public string status_id { get; set; }
        public string pipeline_id { get; set; }
    }

    public class Status
    {
        public string id { get; set; }
        public string name { get; set; }
        public string status_id { get; set; }
        public string old_status_id { get; set; }
        public string price { get; set; }
        public string responsible_user_id { get; set; }
        public string last_modified { get; set; }
        public string modified_user_id { get; set; }
        public string created_user_id { get; set; }
        public string date_create { get; set; }
        public string pipeline_id { get; set; }
        public string account_id { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
