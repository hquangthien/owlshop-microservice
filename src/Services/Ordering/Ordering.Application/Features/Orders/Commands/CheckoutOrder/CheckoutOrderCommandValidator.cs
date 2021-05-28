using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty().WithMessage("{UserName} is required")
                .NotNull()
                .MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters.");

            RuleFor(p => p.EmailAddress)
                .NotEmpty().WithMessage("{UserName} is required");

            RuleFor(p => p.TotalPrice)
                .NotEmpty().WithMessage("{TotoPrice} is requited")
                .GreaterThan(0).WithMessage("{TotalPrice} must be greater than zero");
        }
    }
}