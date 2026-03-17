using Analytics.Domain.Entities.Marketing.Events;
using Analytics.Domain.Enums;
using Analytics.Domain.Exceptions;
using Domain;
using FluentResults;
using ValueObjects.ValueObject;

namespace Analytics.Domain.Entities.Marketing;

public class Campaign : Entity
{
    public string Name { get; private set; }
    public AcquisitionChannel Channel { get; private set; }
    public Money Budget { get; private set; }
    public CampaignStatus Status { get; private set; }
    public Money ActualSpend { get; private set; }
    public DateRange ActivePeriod { get; private set; }

    public static Campaign Create(
        Guid tenantId,
        string name,
        AcquisitionChannel channel,
        DateRange activePeriod,
        Money budget)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Вкажіть назву кампанії");
        if (budget.Amount < 0)
            throw new DomainException("Бюджет не може бути відʼємний");
        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name.Trim(),
            Channel = channel,
            ActivePeriod = activePeriod,
            Budget = budget,
            ActualSpend = new Money(0, budget.Currency),
            Status = CampaignStatus.Draft,
            CreatedAt = DateTime.UtcNow,
        };

        campaign.AddDomainEvent(new CampaignCreatedDomainEvent(tenantId, campaign.Id));
        return campaign;
    }

    public void AddSpend(Money spend)
    {
        if (spend.Amount <= 0)
            throw new DomainException("Витрата має бути більше нуля.");

        if (spend.Currency != ActualSpend.Currency)
            throw new DomainException("Валюта витрати не збігається з валютою кампанії.");
        
        ActualSpend = new Money(ActualSpend.Amount + spend.Amount, ActualSpend.Currency);
    }

    public Result Activate(DateOnly currentDate)
    {
        if (Status != CampaignStatus.Draft)
            return Result.Fail("Неможливо активувати кампанію: поточний статус не є 'Draft'.");
        if (ActivePeriod.EndDate < currentDate)
            return Result.Fail("Неможливо активувати кампанію: дата завершення періоду дії вже минула.");
        Status = CampaignStatus.Active;
        
        AddDomainEvent(new CampaignStatusChangedDomainEvent(TenantId, Id, nameof(CampaignStatus.Active)));
        return Result.Ok();
    }

    public Result Complete()
    {
        if (Status != CampaignStatus.Active)
            return Result.Fail("Неможливо завершити кампанію: вона не є активною.");
        Status = CampaignStatus.Completed;
        AddDomainEvent(new CampaignStatusChangedDomainEvent(TenantId, Id, nameof(CampaignStatus.Completed)));
        return Result.Ok();
    }

    public Result Pause()
    {
        if (Status != CampaignStatus.Active)
            return Result.Fail("Можна призупинити лише активну кампанію.");
        Status = CampaignStatus.Paused;
        AddDomainEvent(new CampaignStatusChangedDomainEvent(TenantId, Id, nameof(CampaignStatus.Paused)));

        return Result.Ok();
    }

    public Result Resume(DateOnly currentDate)
    {
        if (Status != CampaignStatus.Paused)
            return Result.Fail("Можна відновити лише призупинену кампанію.");
        if (ActivePeriod.EndDate < currentDate)
            return Result.Fail("Період дії кампанії вже минув.");
        Status = CampaignStatus.Active;
        AddDomainEvent(new CampaignStatusChangedDomainEvent(TenantId, Id, nameof(CampaignStatus.Active)));
        return Result.Ok();
    }

    public void UpdateBudget(Money budget)
    {
        if (budget.Amount < 0)
            throw new DomainException("Бюджет не може бути від'ємний.");
        Budget = budget;
        
        AddDomainEvent(new CampaignUpdatedDomainEvent(TenantId, Id));
    }

    public void UpdateEndDate(DateOnly endDate, DateOnly today)
    {
        if (endDate < today)
            throw new DomainException("Дата завершення не може бути в минулому.");
        ActivePeriod = new DateRange(ActivePeriod.StartDate, endDate);
        AddDomainEvent(new CampaignUpdatedDomainEvent(TenantId, Id));
    }
}