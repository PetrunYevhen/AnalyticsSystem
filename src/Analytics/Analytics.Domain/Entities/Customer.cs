using Domain;

namespace Analytics.Domain.Entities;

public class Customer : Entity
{
    public string? PhoneNumber { get; private set; }
    public string ExternalId { get; private set; }
    public string? Email { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    
    private Customer(){}
    
    public Customer(Guid tenantId, string externalId, string phoneNumber, string? email, DateTime registrationDate)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        ExternalId = externalId;
        PhoneNumber = phoneNumber;
        Email = email;
        RegistrationDate = registrationDate;
        CreatedAt = DateTime.UtcNow;
    }
}