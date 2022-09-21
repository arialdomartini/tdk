using System.Collections;
using System.Collections.Generic;
using NeatCoinKata.Dojo.DataTypes;
using static NeatCoinKata.Dojo.NeatCoin;

namespace NeatCoinKata.Dojo;

static class BlocksExtensions
{
    internal static IEnumerable<Block> GetEnumerable(this Ledger ledger) => 
        new Blocks(ledger);
}

internal class Blocks : IEnumerable<Block>
{
    private readonly Ledger _ledger;
    private Block _current;

    internal Blocks(Ledger ledger)
    {
        _ledger = ledger;
    }

    public IEnumerator<Block> GetEnumerator()
    {
        _current = Origin(_ledger);
        yield break;
        while (_current != null)
        {
            yield return _current;
            _current = FindChildOf(_ledger, _current.Hash);
        } 
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
