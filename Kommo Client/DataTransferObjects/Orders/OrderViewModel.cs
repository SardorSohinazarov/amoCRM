using System;
using Kommo_Client.Controllers;
using Kommo_Client.Entities;

namespace DataTransferObjects.Orders;

public class OrderViewModel
{
    public long Id { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    public decimal Amount { get; set; }
    public EOrderStatus Status { get; set; }
    public long LeadId { get; set; }
}
