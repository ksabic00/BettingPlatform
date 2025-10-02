using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class OfferOutcome
    {
        public Guid Id { get; set; }
        public Guid OfferId { get; set; }
        public Guid OutcomeTemplateId { get; set; }
        public decimal Odds { get; set; }      
        public bool IsEnabled { get; set; } = true;
    }
}
