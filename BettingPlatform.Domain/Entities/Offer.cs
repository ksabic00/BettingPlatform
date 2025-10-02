using BettingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class Offer
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public Guid MarketTemplateId { get; set; }
        public OfferCategory Category { get; set; } 
        public DateTime ValidFromUtc { get; set; }
        public DateTime? ValidToUtc { get; set; }
    }
}
