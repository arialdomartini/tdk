using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NeatCoinKata.Dojo;
using NeatCoinKata.Dojo.DataTypes;
using Xunit;
using static System.Collections.Immutable.ImmutableList;
using static System.Collections.Immutable.ImmutableList<NeatCoinKata.Dojo.Transaction>;

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
            Ledger.WithBlockchain(block);

        var balance = NeatCoin.Balance(ledger, from);

        Assert.Equal(Amount.Of(-amount), balance);
    }

    [Theory]
    [InlineData("alice", "bob", 42)]
    [InlineData("bob", "alice", 42)]
    void transaction_increases_balance_to_receiver(string from, string to, int amount)
    {
        var ledger =
            Ledger.WithBlockchain(
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
            Ledger.WithBlockchain(
                Block.WithTransactions(
                    Transaction.Create(from, to, Amount.Of(amount))));

        var balance = NeatCoin.Balance(ledger, to);

        Assert.Equal(Amount.Of(0), balance);
    }

    [Fact]
    void multiple_transactions()
    {
        var block1 =
            NeatCoin.Hashed(
                Block.WithTransactions(
                    Transaction.Create("bob", "alice", Amount.Of(10)),
                    Transaction.Create("bob", "alice", Amount.Of(20)),
                    Transaction.Create("alice", "bob", Amount.Of(5))));

        var block2 =
            NeatCoin.Hashed(
                Block.WithTransactions(
                        Transaction.Create("bob", "alice", Amount.Of(100)),
                        Transaction.Create("dan", "alice", Amount.Of(20)))
                    .HavingParent(block1));

        var ledger =
            Ledger.WithBlockchain(block1, block2);

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

    [Fact]
    void blocks_with_different_parent_have_different_hash_values()
    {
        var block1 =
            NeatCoin.Hashed(
                Block1);

        var block2 =
            NeatCoin.Hashed(
                Block1
                    .HavingParent(block1));

        Assert.NotEqual(block1.Hash, block2.Hash);
    }

    public static IEnumerable<object[]> InvalidBlocks() =>
        new[]
        {
            new object[] { Block.Empty },
            new object[] { Block.WithTransactions(Transaction.Create("bob", "alice", Amount.Of(44))) },
            new object[] { new Block(Empty, Hash.Undefined, Hash.Undefined) },
            new object[] { new Block(Empty, new Hash("random-hash"), Hash.Undefined) }
        };

    public static IEnumerable<object[]> ValidBlocks() =>
        new[]
        {
            new object[]
            {
                NeatCoin.Hashed(
                    Block.Empty)
            },
            new object[]
            {
                NeatCoin.Hashed(
                    Block.WithTransactions(Transaction.Create("bob", "alice", Amount.Of(44))))
            },
            new object[]
            {
                NeatCoin.Hashed(
                    Block.WithTransactions(
                        Transaction.Create("bob", "alice", Amount.Of(10)),
                        Transaction.Create("bob", "alice", Amount.Of(20)),
                        Transaction.Create("alice", "bob", Amount.Of(5))))
            }
        };

    [Theory]
    [MemberData(nameof(ValidBlocks))]
    internal void block_is_valid_if_matches_its_hash(Block block)
    {
        Assert.True(NeatCoin.IsValid(block));
    }

    [Theory]
    [MemberData(nameof(InvalidBlocks))]
    internal void block_is_not_valid_if_it_does_not_match_its_hash(Block block)
    {
        Assert.False(NeatCoin.IsValid(block));
    }

    [Fact]
    void blocks_can_be_retrieved_by_hash()
    {
        var empty = NeatCoin.Hashed(Block.Empty);
        var block1 = NeatCoin.Hashed(Block1.HavingParent(empty));
        var block2 = NeatCoin.Hashed(Block2.HavingParent(block1));

        var ledger = Ledger.WithBlockchain(empty, block1, block2);

        ledger.Blockchain.ForEach(block =>
        {
            var found = NeatCoin.GetBlock(ledger, block.Hash);

            Assert.Equal(block, found);
        });
    }

    [Fact]
    void not_found_blocks_return_null()
    {
        var empty = NeatCoin.Hashed(Block.Empty);
        var block1 = NeatCoin.Hashed(Block1);

        var ledger = Ledger.WithBlockchain(empty, block1);

        Assert.Null(NeatCoin.GetBlock(ledger, "not-existing"));
    }

    [Fact]
    void blocks_can_be_retrieved_by_parent_hash()
    {
        var empty = NeatCoin.Hashed(Block.Empty);
        var block1 = NeatCoin.Hashed(Block1.HavingParent(empty));
        var block2 = NeatCoin.Hashed(Block2.HavingParent(block1));

        var ledger = Ledger.WithBlockchain(empty, block1, block2);

        Assert.Equal(block1, NeatCoin.FindChildOf(ledger, empty.Hash));
        Assert.Equal(block2, NeatCoin.FindChildOf(ledger, block1.Hash));
    }

    [Fact]
    void origin_block_has_no_parent()
    {
        var block1 = NeatCoin.Hashed(Block1);
        var empty = NeatCoin.Hashed(Block.Empty.HavingParent(block1));
        var block2 = NeatCoin.Hashed(Block2.HavingParent(block1));

        var ledger = Ledger.WithBlockchain(empty, block1, block2);

        var origin = NeatCoin.Origin(ledger);

        Assert.Equal(block1, origin);
        Assert.Equal(string.Empty, origin.Parent);
    }

    [Fact]
    void find_origin()
    {
        var block1 =
            NeatCoin.Hashed(
                Block.WithTransactions(
                    Transaction.Create("bob", "alice", Amount.Of(10)),
                    Transaction.Create("bob", "alice", Amount.Of(20)),
                    Transaction.Create("alice", "bob", Amount.Of(5))));

        var block2 =
            NeatCoin.Hashed(
                Block.WithTransactions(
                        Transaction.Create("bob", "alice", Amount.Of(100)),
                        Transaction.Create("dan", "alice", Amount.Of(20)))
                    .HavingParent(block1));

        var ledger =
            Ledger.WithBlockchain(block1, block2);

        Assert.Equal(block1, NeatCoin.Origin(ledger));
    }
}
