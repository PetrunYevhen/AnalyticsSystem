namespace Domain;

public class Entity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; } // Key for isolation between clients
    public DateTime CreatedAt { get; set; }
}