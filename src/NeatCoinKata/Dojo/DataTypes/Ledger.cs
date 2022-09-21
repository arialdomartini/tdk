namespace NeatCoinKata.Dojo.DataTypes;

internal record Ledger(Transaction? Transaction = null)
{
    internal static Ledger Empty => new();

    internal static Ledger WithTransaction(Transaction transaction) => new(transaction);
}
