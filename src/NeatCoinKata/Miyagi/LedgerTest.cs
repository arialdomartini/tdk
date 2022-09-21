using NeatCoinKata.Dojo;
using NeatCoinKata.Dojo.DataTypes;
using Xunit;

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
}
