namespace NeatCoinKata.Dojo.DataTypes;

internal record Hash(string Value)
{
    internal static Hash Undefined => new("");
    
    public static implicit operator string(Hash hash) => hash.Value;
    public static implicit operator Hash(string value) => new(value);
}
