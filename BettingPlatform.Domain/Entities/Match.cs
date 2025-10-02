using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class Match
    {
        public Guid Id { get; set; }
        public string HomeTeam { get; set; } = default!;
        public string AwayTeam { get; set; } = default!;
        public DateTime StartsAtUtc { get; set; }
        public string Sport { get; set; } = "football";
        public string? Status { get; set; }
    }
}
