using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class MarketTemplate
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;  
        public string Name { get; set; } = default!;  
    }
}
