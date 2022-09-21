namespace NeatCoinKata.Dojo.DataTypes;

internal record Account(string name)
{
    internal static Account From(string name) => new(name);
}
