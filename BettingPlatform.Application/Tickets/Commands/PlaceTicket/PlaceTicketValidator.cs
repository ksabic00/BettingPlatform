using BettingPlatform.Application.Tickets.Dtos;
using FluentValidation;

namespace BettingPlatform.Application.Tickets.Commands.PlaceTicket;

public sealed class PlaceTicketValidator : AbstractValidator<PlaceTicketCommand>
{
    public PlaceTicketValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.Stake).GreaterThan(0);

        RuleFor(x => x.Selections).NotEmpty();
        RuleForEach(x => x.Selections).SetValidator(new SelectionValidator());
    }

    private sealed class SelectionValidator : AbstractValidator<PlaceTicketSelectionDto>
    {
        public SelectionValidator()
        {
            RuleFor(x => x.OfferId).NotEmpty();
            RuleFor(x => x.OutcomeTemplateId).NotEmpty();
        }
    }
}
