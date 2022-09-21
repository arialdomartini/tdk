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

    internal static Hash CalculateBlockHash(Block block) => 
        CryptographicHash.Hash(new { block.Transactions });

    internal static Block Hashed(Block block) =>
        Block.Create(
            block.Transactions, 
            CalculateBlockHash(block));

    internal static bool IsValid(Block block) => 
        Hashed(block).Hash == block.Hash;

    public static Block GetBlock(Ledger ledger, Hash hash) => 
        ledger.Blocks.SingleOrDefault(b => b.Hash == hash);
}
