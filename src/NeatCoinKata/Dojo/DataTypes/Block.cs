using System.Collections.Immutable;

namespace NeatCoinKata.Dojo.DataTypes;

internal record Block(ImmutableList<Transaction> Transactions, Hash Hash, Hash Parent)
{
    internal static Block Empty =>
        WithTransactions();

    internal static Block WithTransactions(params Transaction[] transactions) =>
        WithTransactions(transactions.ToImmutableList());
 
    private static Block WithTransactions(ImmutableList<Transaction> transactions) =>
        new(transactions, Hash.Undefined, Hash.Undefined);

    internal Block HavingParent(Block parent) =>
        Create(Transactions, Hash.Undefined, parent.Hash);
    
    internal static Block Create(ImmutableList<Transaction> transactions, Hash hash, Hash parent) =>
        new(transactions, hash, parent);
}
