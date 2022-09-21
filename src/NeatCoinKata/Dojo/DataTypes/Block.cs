using System.Collections.Immutable;

namespace NeatCoinKata.Dojo.DataTypes;

internal record Block(ImmutableList<Transaction> Transactions)
{
    internal static Block Empty => 
        WithTransactions();

    internal static Block WithTransactions(params Transaction[] transactions) => 
        WithTransactions(transactions.ToImmutableList());
    
    internal static Block WithTransactions(ImmutableList<Transaction> transactions) => 
        new(transactions);
}
