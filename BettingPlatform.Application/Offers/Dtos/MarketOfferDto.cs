using BettingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Application.Offers.Dtos
{
    public class MarketOfferDto
    {
        public Guid OfferId { get; init; }
        public string MarketCode { get; init; } = default!;
        public OfferCategory Category { get; init; }
        public IReadOnlyList<OfferOutcomeDto> Outcomes { get; init; } = new List<OfferOutcomeDto>();
    }
}
