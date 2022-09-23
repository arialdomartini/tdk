using System.Collections.Immutable;

namespace NeatCoinKata.Dojo.DataTypes;

internal record Ledger(ImmutableList<Block> Blocks)
{
    internal static Ledger Empty => 
        WithBlocks();

    internal static Ledger WithBlocks(params Block[] blocks) => 
        WithBlocks(blocks.ToImmutableList());
    
    internal static Ledger WithBlocks(ImmutableList<Block> blocks) => 
        new(blocks);
}
