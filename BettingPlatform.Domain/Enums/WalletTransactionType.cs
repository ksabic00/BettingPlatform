using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BettingPlatform.Domain.Enums
{
    public enum WalletTransactionType
    {
        Deposit = 1,
        BetStakeDebit = 2,
        BetFeeDebit = 3,
        BetPayoutCredit = 4,
        Refund = 5
    }
}
