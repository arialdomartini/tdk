using NeatCoinKata.Dojo.DataTypes;

namespace NeatCoinKata.Dojo;

internal static class NeatCoin
{
    internal static Amount Balance(Ledger ledger, Account account)
    {
        if(ledger.Transaction == null) return Amount.Zero;
        
        var value = ledger.Transaction.Amount.Value;

        var given = ledger.Transaction?.From == account? value: 0;
        var received = ledger.Transaction?.To == account? value: 0;
        
        return Amount.Of(received - given);
    }
}
