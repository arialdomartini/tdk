using System.Collections.Immutable;
using System.Linq;
using NeatCoinKata.Dojo.DataTypes;

namespace NeatCoinKata.Dojo;

internal static class NeatCoin
{
    internal static Amount Balance(Ledger ledger, Account account) =>
        ledger.GetEnumerable()
            .Aggregate(
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
        CryptographicHash.Hash(new { block.Transactions, block.Parent });

    internal static Block Hashed(Block block) =>
        Block.Create(
            block.Transactions,
            CalculateBlockHash(block),
            block.Parent);

    internal static bool IsValid(Block block) =>
        Hashed(block).Hash == block.Hash;

    internal static Block GetBlock(Ledger ledger, Hash hash) =>
        Find(ledger.Blockchain, hash);

    private static Block Find(ImmutableList<Block> blockchain, Hash hash) => 
        blockchain.SingleOrDefault(b => b.Hash == hash);

    internal static Block Origin(Ledger ledger) =>
        FindChildOf(ledger, Hash.Undefined);

    internal static Block? FindChildOf(Ledger ledger, Hash parent) => 
        ledger.Blockchain.SingleOrDefault(b => b.Parent == parent);
}
