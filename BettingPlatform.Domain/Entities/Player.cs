using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class Player
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = default!;
    }
}
