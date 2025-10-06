using BettingPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Application.Wallets.Dtos
{
    public class WalletTransactionDto
    {
        public DateTime OccurredAtUtc { get; init; }
        public WalletTransactionType Type { get; init; }
        public decimal Amount { get; init; }
        public string? ReferenceId { get; init; }
    }
}
