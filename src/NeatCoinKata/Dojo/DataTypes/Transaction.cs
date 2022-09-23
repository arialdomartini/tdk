using System.Collections.Immutable;
using NeatCoinKata.Dojo.DataTypes;

namespace NeatCoinKata.Dojo;

internal record Transaction(Account From, Account To, Amount Amount)
{
    internal static Transaction Create(Account from, Account to, Amount amount)
        => new(from, to, amount);
    
    internal ImmutableList<Transaction> Singleton() => ImmutableList.Create(this);
}
