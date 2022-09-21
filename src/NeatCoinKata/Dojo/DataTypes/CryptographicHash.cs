using System;
using System.Security.Cryptography;

namespace NeatCoinKata.Dojo.DataTypes;

internal static class CryptographicHash
{
    internal static Hash Hash<T>(T? o) =>
        Convert.ToBase64String(
            ComputeSHA256(o?.ToJson()));

    private static byte[] ComputeSHA256(string? @string) =>
        SHA256.Create()
            .ComputeHash((@string ?? string.Empty).ToByteArray());
}
