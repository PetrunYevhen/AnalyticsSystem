namespace Analytics.Domain.Entities.Customer;

public enum CustomerStatus
{
    Potential = 0,
    New = 1,
    Active = 2,
    AtRisk = 3,
    Churned = 4,
    Reactivated = 5
}