namespace NeatCoinKata.Dojo.DataTypes;

internal record Amount(int Value)
{
    internal static Amount Of(int value) => new(value);
    internal static Amount Zero => Of(0);
}
