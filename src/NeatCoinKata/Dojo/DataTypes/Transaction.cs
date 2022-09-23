using NeatCoinKata.Dojo.DataTypes;

namespace NeatCoinKata.Dojo;

internal record Transaction(Account From, Account To, Amount Amount)
{
    public static Transaction Create(Account from, Account to, Amount amount)
        => new(from, to, amount);
}
