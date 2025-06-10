namespace Kommo_Client.Entities;

public class Order
{
    public long Id { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    public decimal Amount { get; set; }
    public long LeadId { get; set; }
}
