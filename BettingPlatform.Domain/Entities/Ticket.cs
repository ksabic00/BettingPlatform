using BettingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }

        public decimal StakeGross { get; set; }     
        public decimal FeePercent { get; set; }       
        public decimal FeeAmount { get; set; }
        public decimal StakeNet { get; set; }

        public decimal CombinedOdds { get; set; }
        public decimal PotentialPayout { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public TicketStatus Status { get; set; }
        public decimal? PayoutAmount { get; set; }
    }
}
