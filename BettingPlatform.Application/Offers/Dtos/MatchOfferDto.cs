using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Application.Offers.Dtos
{
    public class MatchOfferDto
    {
        public Guid MatchId { get; init; }
        public string HomeTeam { get; init; } = default!;
        public string AwayTeam { get; init; } = default!;
        public DateTime StartsAtUtc { get; init; }
        public IReadOnlyList<MarketOfferDto> Markets { get; init; } = new List<MarketOfferDto>();
    }
}
