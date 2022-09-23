using System.Collections.Immutable;
using System.Linq;
using NeatCoinKata.Dojo;
using NeatCoinKata.Dojo.DataTypes;
using Xunit;
using static System.Collections.Immutable.ImmutableList;

namespace NeatCoinKata.Miyagi;

public class LedgerTest
{
    private static Block Block1 =>
        Block.WithTransactions(
            Transaction.Create("bob", "alice", Amount.Of(10)),
            Transaction.Create("bob", "alice", Amount.Of(20)),
            Transaction.Create("alice", "bob", Amount.Of(5)));

    private static Block Block2 =>
        Block.WithTransactions(
            Transaction.Create("bob", "alice", Amount.Of(100)),
            Transaction.Create("dan", "alice", Amount.Of(20)));

    [Theory]
    [InlineData("alice")]
    [InlineData("bob")]
    [InlineData("dan")]
    [InlineData("carol")]
    void everybody_s_balance_is_0(string name)
    {
        var ledger = Ledger.Empty;
        var account = Account.From(name);

        var balance = NeatCoin.Balance(ledger, account);

        Assert.Equal(Amount.Zero, balance);
    }

    [Theory]
    [InlineData("alice", "bob", 42)]
    [InlineData("bob", "alice", 42)]
    void transaction_decreases_balance_to_sender(string from, string to, int amount)
    {
        var block = Block.WithTransactions(
            Transaction.Create(from, to, Amount.Of(amount)));

        var ledger =
            Ledger.WithBlocks(Create(block));

        var balance = NeatCoin.Balance(ledger, from);

        Assert.Equal(Amount.Of(-amount), balance);
    }

    [Theory]
    [InlineData("alice", "bob", 42)]
    [InlineData("bob", "alice", 42)]
    void transaction_increases_balance_to_receiver(string from, string to, int amount)
    {
        var ledger =
            Ledger.WithBlocks(
                    Block.WithTransactions(
                        Transaction.Create(from, to, Amount.Of(amount))));

        var balance = NeatCoin.Balance(ledger, to);

        Assert.Equal(Amount.Of(+amount), balance);
    }

    [Theory]
    [InlineData("alice", "alice", 42)]
    void sending_money_to_self_does_not_change_balance(string from, string to, int amount)
    {
         var ledger = 
             Ledger.WithBlocks(
                Block.WithTransactions(
                    Transaction.Create(from, to, Amount.Of(amount))));

        var balance = NeatCoin.Balance(ledger, to);

        Assert.Equal(Amount.Of(0), balance);
    }

    [Fact]
    void multiple_transactions()
    {
        var block1 =
            Block.WithTransactions(
                Transaction.Create("bob", "alice", Amount.Of(10)),
                Transaction.Create("bob", "alice", Amount.Of(20)),
                Transaction.Create("alice", "bob", Amount.Of(5)));

        var block2 =
            Block.WithTransactions(
                Transaction.Create("bob", "alice", Amount.Of(100)),
                Transaction.Create("dan", "alice", Amount.Of(20)));

        var ledger =
            Ledger.WithBlocks(block1, block2);

        var balanceBob = NeatCoin.Balance(ledger, "bob");
        Assert.Equal(-10 - 20 + 5 - 100, balanceBob.Value);

        var balanceAlice = NeatCoin.Balance(ledger, "alice");
        Assert.Equal(10 + 20 - 5 + 100 + 20, balanceAlice.Value);

        var balanceDan = NeatCoin.Balance(ledger, "dan");
        Assert.Equal(-20, balanceDan.Value);
    }

    [Fact]
    void blocks_are_hashed_generating_unique_hashes()
    {
        var blocks =
            Create(
                Block.Empty,
                Block1,
                Block2);

        var hashes = blocks.Select(NeatCoin.CalculateBlockHash).ToImmutableList();

        Assert.Equal(hashes.Count, hashes.Distinct().Count());
    }

    [Fact]
    void hash_is_included_in_block()
    {
        var blocks =
            Create(
                Block.Empty,
                Block1,
                Block2);

        var hashed = blocks.Select(NeatCoin.Hashed).ToImmutableList();

        hashed.ForEach(b =>
            Assert.Equal(b.Hash, NeatCoin.CalculateBlockHash(b)));
    }
}
