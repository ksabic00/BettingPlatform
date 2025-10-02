using BettingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class TicketSelection
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid OfferId { get; set; }
        public Guid MatchId { get; set; }
        public Guid OutcomeTemplateId { get; set; }

        public decimal OddAtPlacement { get; set; }
        public OfferCategory CategoryAtPlacement { get; set; }  
    }
}
