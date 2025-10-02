using BettingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Entities
{
    public class WalletTransaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public WalletTransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceId { get; set; }
        public string? Note { get; set; }
    }
}
