using BettingPlatform.Application.Offers.Dtos;
using MediatR;

namespace BettingPlatform.Application.Offers.Queries.ListOffers;

public sealed class ListOffersQuery : IRequest<IReadOnlyList<OfferDto>>
{
    public DateTime? AsOfUtc { get; }
    public ListOffersQuery(DateTime? asOfUtc = null) => AsOfUtc = asOfUtc;
}
