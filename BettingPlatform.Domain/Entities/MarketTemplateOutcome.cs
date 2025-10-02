using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class MarketTemplateOutcome
    {
        public Guid MarketTemplateId { get; set; }
        public Guid OutcomeTemplateId { get; set; }
    }
}
