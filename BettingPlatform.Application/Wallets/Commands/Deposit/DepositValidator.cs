using FluentValidation;

namespace BettingPlatform.Application.Wallets.Commands.Deposit;

public sealed class DepositValidator : AbstractValidator<DepositCommand>
{
    public DepositValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
