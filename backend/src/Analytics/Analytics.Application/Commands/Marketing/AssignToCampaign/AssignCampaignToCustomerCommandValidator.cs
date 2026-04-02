using FluentValidation;

namespace Analytics.Application.Commands.Marketing.AssignToCampaign;

public sealed class AssignCampaignToCustomersCommandValidator : AbstractValidator<AssignCampaignToCustomersCommand>
{
    public AssignCampaignToCustomersCommandValidator()
    {
        RuleFor(x => x.CampaignId)
            .NotEmpty().WithMessage("Ідентифікатор кампанії не може бути порожнім.");

        RuleFor(x => x.CustomerIds)
            .NotNull().WithMessage("Список клієнтів не може бути відсутнім.")
            .NotEmpty().WithMessage("Список клієнтів не може бути порожнім.")
            .Must(ids => ids!.Count <= 1000)
            .WithMessage("За один раз можна призначити не більше 1000 клієнтів.")
            .Must(ids => ids!.Distinct().Count() == ids!.Count)
            .WithMessage("Список клієнтів містить дублікати.");

        RuleForEach(x => x.CustomerIds)
            .NotEmpty().WithMessage("Ідентифікатор клієнта не може бути порожнім.");
    }
}