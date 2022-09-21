using NeatCoinKata.Dojo;
using NeatCoinKata.Dojo.DataTypes;
using Xunit;
using static System.Collections.Immutable.ImmutableList;

namespace NeatCoinKata.Miyagi;

public class LedgerTest
{
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
        var ledger =
            Ledger.WithTransaction(
                Transaction.Create(from, to, Amount.Of(amount)));
        
        var balance = NeatCoin.Balance(ledger, from);
        
        Assert.Equal(Amount.Of(-amount), balance);
    }
        
    [Theory]
    [InlineData("alice", "bob", 42)]
    [InlineData("bob", "alice", 42)]
    void transaction_increases_balance_to_receiver(string from, string to, int amount)
    {
        var ledger =
            Ledger.WithTransaction(
                Transaction.Create(from, to, Amount.Of(amount)));
        
        var balance = NeatCoin.Balance(ledger, to);
        
        Assert.Equal(Amount.Of(+amount), balance);
    }
    
    [Theory]
    [InlineData("alice", "alice", 42)]
    void sending_money_to_self_does_not_change_balance(string from, string to, int amount)
    {
        var ledger =
            Ledger.WithTransaction(
                Transaction.Create(from, to, Amount.Of(amount)));
        
        var balance = NeatCoin.Balance(ledger, to);
        
        Assert.Equal(Amount.Of(0), balance);
    }
    
        
    [Fact]
    void multiple_transactions()
    {
        var ledger =
            Ledger.WithTransactions(
                Create(
                    Transaction.Create("bob", "alice", Amount.Of(10)),
                    Transaction.Create("bob", "alice", Amount.Of(20)),
                    Transaction.Create("alice", "bob", Amount.Of(5))));
        
        var balanceBob = NeatCoin.Balance(ledger, "bob");
        Assert.Equal(-10 - 20 + 5, balanceBob.Value);
        
        var balanceAlice = NeatCoin.Balance(ledger, "alice");
        Assert.Equal(10 + 20 - 5, balanceAlice.Value);
    }

}
