using Analytics.Domain.Entities.Marketing.Events;
using Domain;
using ValueObjects.ValueObject;

namespace Analytics.Domain.Entities.Marketing;

public class MarketingExpense : Entity
{
   public Guid? CampaignId { get; private set; }
   public Money Amount { get; private set; }
   public string AdSource { get; private set; }    
   public DateOnly ExpenseDate { get; private set; }   
   public int? Impressions { get; private set; } 
   public int? Clicks { get; private set; }      
   public int? Leads { get; private set; }
   public DateTime? MetricsRecordedAt { get; private set; }
   
   private MarketingExpense(){}

   public static MarketingExpense Create(
      Guid tenantId,
      Guid? campaignId,
      string adSource,
      Money amount,
      DateOnly expenseDate)
   {
      var expense = new MarketingExpense
      {
         Id = Guid.NewGuid(),
         TenantId = tenantId,
         CampaignId = campaignId,
         AdSource = adSource,
         Amount = amount,
         ExpenseDate = expenseDate,
         CreatedAt = DateTime.UtcNow
      };
      
      expense.AddDomainEvent(new MarketingExpensesCreatedDomainEvent(tenantId, expense.Id));
      
      return expense;
   }
   
   public void RecordMetrics(int? impressions, int? clicks, int? leads, DateTime? metricsRecordedAt)
   {
      if (impressions < 0 || clicks < 0 || leads < 0)
         throw new ArgumentException("Метрики не можуть бути від'ємними");
      if (clicks > impressions)
         throw new ArgumentException("Кліки не можуть перевищувати покази");
      if (leads > clicks)
         throw new ArgumentException("Ліди не можуть перевищувати кліки");

      Impressions = impressions;
      Clicks = clicks;
      Leads = leads;
      MetricsRecordedAt = metricsRecordedAt;
   }
   
   public double? ClickThroughRate =>
      Impressions == 0 ? null : (double?)Clicks / Impressions;

   public double? ConversionRate =>
      Clicks == 0 ? null : (double?)Leads / Clicks;
}