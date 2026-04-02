using FluentValidation;

namespace Analytics.Application.Commands.Marketing.UpdateCampaign.Validator;

public class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty().WithMessage("CampaignId обов'язковий.");

        RuleFor(x => x.ActualSpend)
            .GreaterThan(0).WithMessage("Сума має бути більше нуля.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Валюта обов'язкова.");
    }
}