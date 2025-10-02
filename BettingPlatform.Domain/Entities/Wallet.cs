using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public decimal Balance { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
