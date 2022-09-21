using System.Collections.Immutable;

namespace NeatCoinKata.Dojo.DataTypes;

internal record Ledger(ImmutableList<Block> Blockchain)
{
    internal static Ledger Empty => 
        WithBlockchain(ImmutableList<Block>.Empty);

    internal static Ledger WithBlockchain(params Block[] blocks) => 
        WithBlockchain(blocks.ToImmutableList());

    private static Ledger WithBlockchain(ImmutableList<Block> blocks) => 
        new(blocks);
}
