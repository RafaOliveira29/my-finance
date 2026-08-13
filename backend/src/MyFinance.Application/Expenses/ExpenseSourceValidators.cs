using FluentValidation;

namespace MyFinance.Application.Expenses;

public sealed class CreateExpenseSourceRequestValidator : AbstractValidator<CreateExpenseSourceRequest>
{
    public CreateExpenseSourceRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ExpenseKind).IsInEnum();
        RuleFor(x => x.DefaultAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DueDay).InclusiveBetween(1, 31).When(x => x.DueDay is not null);
        RuleFor(x => x.DueMonthOffset).InclusiveBetween(0, 1);
        RuleFor(x => x.RecurrenceType).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate is not null);
    }
}

public sealed class UpdateExpenseSourceRequestValidator : AbstractValidator<UpdateExpenseSourceRequest>
{
    public UpdateExpenseSourceRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ExpenseKind).IsInEnum();
        RuleFor(x => x.DefaultAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DueDay).InclusiveBetween(1, 31).When(x => x.DueDay is not null);
        RuleFor(x => x.DueMonthOffset).InclusiveBetween(0, 1);
        RuleFor(x => x.RecurrenceType).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate is not null);
    }
}