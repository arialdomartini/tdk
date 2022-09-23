using System.Text;
using System.Text.Json;

namespace NeatCoinKata.Dojo.DataTypes;

internal static class JsonExtensions
{
    internal static string ToJson(this object o) =>
        JsonSerializer.Serialize(o);

    internal static byte[] ToByteArray(this string @string) =>
        Encoding.Default.GetBytes(@string);
}
