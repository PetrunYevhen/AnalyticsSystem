using Domain;

namespace Analytics.Domain.Entities;


// Entity to calculate CAC
public class MarketingExpense : Entity
{
   public DateTime Date { get; private set; }
   public decimal Amount { get; private set; }
   public string Currency { get; private set; }
   public string AdSource { get; private set; }
   public string? CampaignName { get; private set; } 
   public int Impressions { get; private set; } 
   public int Clicks { get; private set; }      
   public int Leads { get; private set; }
   
   private MarketingExpense(){}
   
   public MarketingExpense(Guid tenantId, DateTime date, decimal amount, string currency, string adSource, string? campaignName)
   {
      Id = Guid.NewGuid();
      TenantId = tenantId;
      Date = date;
      Amount = amount;
      Currency = currency;
      AdSource = adSource;
      CampaignName = campaignName;
      Impressions = 0;
      Clicks = 0;
      Leads = 0;
      CreatedAt = DateTime.UtcNow;
   }
   
   public void RecordMetrics(int impressions, int clicks, int leads)
   {
      if (impressions < 0 || clicks < 0 || leads < 0)
         throw new ArgumentException("Метрики не можуть бути від'ємними.");

      Impressions = impressions;
      Clicks = clicks;
      Leads = leads;
        
      CreatedAt = DateTime.UtcNow;
   }
}