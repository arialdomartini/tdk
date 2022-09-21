using System.Collections.Immutable;

namespace NeatCoinKata.Dojo.DataTypes;

internal record Ledger(ImmutableList<Transaction> Transactions)
{
    internal static Ledger Empty => 
        WithTransactions(ImmutableList<Transaction>.Empty);

    internal static Ledger WithTransactions(ImmutableList<Transaction> transactions) => 
        new(transactions);
}
