using BettingPlatform.Application.Offers.Dtos;
using MediatR;

namespace BettingPlatform.Application.Offers.Queries.ListOffersGrouped;

public sealed class ListOffersGroupedQuery : IRequest<IReadOnlyList<MatchOfferDto>>
{
    public DateTime? AsOfUtc { get; }
    public ListOffersGroupedQuery(DateTime? asOfUtc = null) => AsOfUtc = asOfUtc;
}
