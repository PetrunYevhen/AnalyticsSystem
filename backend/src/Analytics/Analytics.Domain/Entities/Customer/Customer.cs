using Domain;

namespace Analytics.Domain.Entities.Customer;


public class Customer : Entity
{
    public string FullName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string ExternalId { get; private set; }
    public string? Email { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public  DateTime? FirstOrderDate { get; private set; }
    public DateTime LastOrderDate { get; private set; }
    public CustomerStatus Status { get; private set; }
    public string AcquisitionChannel { get; private set; } 
    public string? AcquisitionCampaign { get; private set; }
    public decimal LifetimeValue { get; private set; }
    public int TotalOrdersCount { get; private set; }
    
    private Customer(){}
    
    public Customer(
        Guid tenantId, 
        string externalId, 
        string fullName,
        string phoneNumber, 
        string? email, 
        DateTime registrationDate, 
        string acquisitionChannel, 
        string acquisitionCampaign)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ExternalId = externalId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
        RegistrationDate = registrationDate;
        AcquisitionChannel = acquisitionChannel;
        AcquisitionCampaign = acquisitionCampaign;
        Status = CustomerStatus.New;
        LifetimeValue = 0;
        TotalOrdersCount = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void RecordOrder(DateTime orderDate)
    {
        if(FirstOrderDate == default)
            FirstOrderDate = orderDate;
        
        if(orderDate >  LastOrderDate)
            LastOrderDate = orderDate;
        
        UpdateStatus();
    }
    
    private void UpdateStatus()
    {
        var daySinceLastOrder =(DateTime.Now - LastOrderDate).Days;
        
        Status = daySinceLastOrder switch
        {
            <= 90 => CustomerStatus.Active,
            <= 180 => CustomerStatus.AtRisk,
            _ => CustomerStatus.Churned
        };
    }

    public void RecordNewOrder(decimal orderAmount, DateTime orderDate)
    {
        if (FirstOrderDate == null)
        {
            FirstOrderDate = orderDate;
        }
        LastOrderDate = orderDate;
        TotalOrdersCount++;
        LifetimeValue += orderAmount;
        
        Status = CustomerStatus.Active;
        UpdateStatus();
    }
}