using Domain;

namespace Analytics.Domain.Entities;

public class Customer : Entity
{
    public string Name { get; set; }
    public string? PhoneNumber { get; private set; }
    public string ExternalId { get; private set; }
    public string? Email { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public  DateTime FirstOrderDate { get; private set; }
    public DateTime LastOrderDate { get; private set; }
    public CustomerStatus Status { get; private set; }
    public string AcquisitionChannel { get; private set; } 
    public string? AcquisitionCampaign { get; private set; }
    
    private Customer(){}
    
    public Customer(
        Guid tenantId, 
        string externalId, 
        string phoneNumber, 
        string? email, 
        DateTime registrationDate, 
        string acquisitionChannel, 
        string acquisitionCampaign)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ExternalId = externalId;
        PhoneNumber = phoneNumber;
        Email = email;
        RegistrationDate = registrationDate;
        AcquisitionChannel = acquisitionChannel;
        AcquisitionCampaign = acquisitionCampaign;
        CreatedAt = DateTime.UtcNow;
    }
}