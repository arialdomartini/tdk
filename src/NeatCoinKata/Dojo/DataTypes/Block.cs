using System.Collections.Immutable;

namespace NeatCoinKata.Dojo.DataTypes;

internal record Block(ImmutableList<Transaction> Transactions, Hash Hash)
{
    internal static Block Empty =>
        WithTransactions();

    internal static Block WithTransactions(params Transaction[] transactions) =>
        WithTransactions(transactions.ToImmutableList());

    private static Block WithTransactions(ImmutableList<Transaction> transactions) =>
        new(transactions, Hash.Undefined);
    
    internal static Block Create(ImmutableList<Transaction> transactions, Hash hash) =>
        new(transactions, hash);
}
