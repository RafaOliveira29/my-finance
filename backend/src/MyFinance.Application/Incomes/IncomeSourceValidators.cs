using FluentValidation;

namespace MyFinance.Application.Incomes;

public sealed class CreateIncomeSourceRequestValidator : AbstractValidator<CreateIncomeSourceRequest>
{
    public CreateIncomeSourceRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DefaultAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CompetenceDay).InclusiveBetween(1, 31);
        RuleFor(x => x.RecurrenceType).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate is not null);
    }
}

public sealed class UpdateIncomeSourceRequestValidator : AbstractValidator<UpdateIncomeSourceRequest>
{
    public UpdateIncomeSourceRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        RuleFor(x => x.DefaultAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CompetenceDay).InclusiveBetween(1, 31);
        RuleFor(x => x.RecurrenceType).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate is not null);
    }
}