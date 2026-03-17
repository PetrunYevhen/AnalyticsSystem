using Analytics.Domain.Entities.Customer.Events;
using Analytics.Domain.Entities.Marketing.Events;
using Analytics.Domain.Entities.Order;
using Analytics.Domain.Enums;
using Analytics.Domain.Exceptions;
using Domain;

namespace Analytics.Domain.Entities.Customer;


public class Customer : Entity
{    
    public string? ExternalId { get; private set; }
    
    public string? FullName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string Email { get; private set; }
    
    public DateTime RegistrationDate { get; private set; }
    public  DateTime? FirstOrderDate { get; private set; }
    public DateTime? LastOrderDate { get; private set; }
    public int OrderCount { get; private set; }
    
    public CustomerStatus Status { get; private set; }
    
    public AcquisitionChannel? AcquisitionChannel { get; private set; }
    public Guid? CampaignId { get; private set; }
    
    public decimal TotalRevenue { get; private set; }    
    public IReadOnlyList<Order.Order> Orders => _orders.AsReadOnly();
    private readonly List<Order.Order> _orders = [];
    
    private Customer(){}
    
    public static Customer Create(
        Guid tenantId,
        string? externalId,
        string? fullName,
        string? phoneNumber,
        string email,
        DateTime registrationDate,
        AcquisitionChannel? acquisitionChannel,
        Guid? campaignId)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ExternalId = externalId,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            RegistrationDate = registrationDate,
            AcquisitionChannel = acquisitionChannel,
            CampaignId = campaignId,
            Status = CustomerStatus.New,
            CreatedAt = DateTime.UtcNow,
        };
        return customer;
    }

    private void RefreshStatus(DateTime now)
    {
        if (LastOrderDate is null)
        {
            Status = CustomerStatus.New;
            return;
        }

        if ((now - RegistrationDate).Days <= 7)
        {
            Status = CustomerStatus.New;
            return;
        }

        var daysSinceLastOrder = (now - LastOrderDate.Value).Days;

        Status = daysSinceLastOrder switch
        {
            <= 90  => CustomerStatus.Active,
            <= 180 => CustomerStatus.AtRisk,
            _      => CustomerStatus.Churned
        };
    }
    private void RecalculateTotalRevenue()
    {
        TotalRevenue = _orders
            .Where(o => o.Status == OrderStatus.Completed)
            .Sum(o => o.TotalAmount.Amount);
    }

    private void RecordOrder(DateTime orderDate, DateTime now)
    {
        if (orderDate > now)
            throw new DomainException("Дата замовлення не може бути в майбутньому");

        FirstOrderDate ??= orderDate;

        if (LastOrderDate is null || orderDate > LastOrderDate)
            LastOrderDate = orderDate;
        
        OrderCount++;
        RefreshStatus(now);
    }
    
    public void AddOrder(Order.Order order, DateTime now)
    {
        _orders.Add(order);
        RecalculateTotalRevenue();
        RecordOrder(order.OrderDate, now);
    }
    
    public void AssignToCampaign(Guid campaignId, AcquisitionChannel channel)
    {
        CampaignId = campaignId;
        AcquisitionChannel = channel;
        AddDomainEvent(new CustomerAssignedToCampaignDomainEvent(Id, campaignId, TenantId));
    }
    public void DeleteFromCampaign()
    {
        var oldCampaignId = CampaignId;
        CampaignId = null;
        AcquisitionChannel = null;
        AddDomainEvent(new CustomerRemovedFromCampaignDomainEvent(Id, oldCampaignId, TenantId));
    }
}