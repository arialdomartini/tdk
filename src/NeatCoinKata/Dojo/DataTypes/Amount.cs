namespace NeatCoinKata.Dojo.DataTypes;

internal record Amount(int Value)
{
    internal static Amount Of(int value) => new(value);
    internal static Amount Zero => Of(0);
    internal Amount Negate => Of(- Value);
    
    public static Amount operator +(Amount a, Amount b) => Of(a.Value + b.Value);
    public static Amount operator -(Amount a, Amount b) => Of(a.Value - b.Value);
    
    public static implicit operator Amount(int value) => Of(value);
}
