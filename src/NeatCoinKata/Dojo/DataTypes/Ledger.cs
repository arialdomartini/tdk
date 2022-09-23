using System.Collections.Immutable;
using System.Linq;

namespace NeatCoinKata.Dojo.DataTypes;

internal record Ledger(ImmutableList<Transaction> Transactions)
{
    internal Transaction? Transaction => !Transactions.Any()? null : Transactions.Single();
    
    internal static Ledger Empty => 
        new(ImmutableList<Transaction>.Empty);

    internal static Ledger WithTransaction(Transaction transaction) => 
        WithTransactions(ImmutableList.Create(transaction));
    
    internal static Ledger WithTransactions(ImmutableList<Transaction> transactions) => 
        new(transactions);
}
