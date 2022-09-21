using System.Linq;
using NeatCoinKata.Dojo.DataTypes;

namespace NeatCoinKata.Dojo;

internal static class NeatCoin
{
    internal static Amount Balance(Ledger ledger, Account account) =>
        ledger.Blocks.Aggregate(
            Amount.Zero,
            (sum, block) =>
                sum + block.Transactions.Aggregate(
                    Amount.Zero,
                    (sum, transaction) =>
                        sum + Calculate(transaction, account)));

    private static Amount Calculate(Transaction transaction, Account account)
    {
        var received =
            account == transaction.To ? transaction.Amount : Amount.Zero;

        var sent =
            account == transaction.From ? transaction.Amount : Amount.Zero;

        return received - sent;
    }
}
