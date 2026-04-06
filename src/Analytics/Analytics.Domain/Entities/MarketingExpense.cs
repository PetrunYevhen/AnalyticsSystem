using Domain;

namespace Analytics.Domain.Entities;


// Entity to calculate CAC
public class MarketingExpense : Entity
{
   public DateTime Date { get; private set; }
   public decimal Amount { get; private set; }
   public string Currency { get; private set; }
   public string AdSource { get; private set; }
   public string CampaignName { get; private set; } 
   
   private MarketingExpense(){}
   
   public MarketingExpense(Guid tenantId, DateTime date, decimal amount, string currency, string adSource, string? campaignName = null)
   {
      Id = Guid.NewGuid();
      TenantId = tenantId;
      Date = date;
      Amount = amount;
      Currency = currency;
      AdSource = adSource;
      CampaignName = campaignName;
      CreatedAt = DateTime.UtcNow;
   }
}