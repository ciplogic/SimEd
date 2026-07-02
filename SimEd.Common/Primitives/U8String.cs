using System.Text;

namespace U8;

/// <summary>
/// A lightweight, immutable UTF-8 string representation stored as raw bytes.
/// Replacement for the U8String NuGet package (which was delisted/unavailable).
/// </summary>
public readonly record struct U8String
{
    private readonly byte[] _utf8Bytes;

    public static readonly U8String Empty = new(Array.Empty<byte>());

    public U8String(string value)
    {
        _utf8Bytes = Encoding.UTF8.GetBytes(value);
    }

    private U8String(byte[] utf8Bytes)
    {
        _utf8Bytes = utf8Bytes;
    }

    public ReadOnlySpan<byte> Utf8Bytes => _utf8Bytes;

    public override string ToString() => Encoding.UTF8.GetString(_utf8Bytes);
}

public static class U8StringExtensions
{
    public static U8String ToU8String(this string value) => new(value);
}
