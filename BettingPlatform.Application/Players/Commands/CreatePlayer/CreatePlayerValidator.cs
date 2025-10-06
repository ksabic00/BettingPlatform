using FluentValidation;

namespace BettingPlatform.Application.Players.Commands.CreatePlayer;

public sealed class CreatePlayerValidator : AbstractValidator<CreatePlayerCommand>
{
    public CreatePlayerValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
    }
}
