namespace DataTransferObjects.Orders;

public class OrderModificationDto
{
	public string PhoneNumber { get; set; }
	public string UserName { get; set; }
	public decimal Amount { get; set; }
	public long LeadId { get; set; }
}
