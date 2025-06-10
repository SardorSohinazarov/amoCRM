namespace Kommo_Client.Entities;

public class Order
{
    public long Id { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    public decimal Amount { get; set; }
    public EOrderStatus Status { get; set; } = EOrderStatus.New;
    public long LeadId { get; set; }
}

public enum EOrderStatus
{
    New = 0,
    Processing = 1,
    Completed = 2,
    Cancelled = 3
}
