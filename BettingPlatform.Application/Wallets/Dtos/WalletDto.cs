using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Application.Wallets.Dtos
{
    public class WalletDto
    {
        public Guid PlayerId { get; init; }
        public decimal Balance { get; init; }
        public IReadOnlyList<WalletTransactionDto> LastTransactions { get; init; }
            = Array.Empty<WalletTransactionDto>();
    }
}
