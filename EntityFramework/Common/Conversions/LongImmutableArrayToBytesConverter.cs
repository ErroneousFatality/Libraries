﻿using System.Collections.Immutable;

using AndrejKrizan.DotNet.Collections;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Conversions;

public class LongImmutableArrayToBytesConverter : ValueConverter<ImmutableArray<long>, byte[]>
{
    public LongImmutableArrayToBytesConverter(ConverterMappingHints? mappingHints = null)
        : base(
            (longs) => LongsToBytes(longs),
            (bytes) => BytesToLongs(bytes),
            mappingHints
        )
    { }

    // Methods
    public static ImmutableArray<long> BytesToLongs(byte[] bytes)
    {
        long[] buffer = new long[bytes.Length / 8];
        Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
        ImmutableArray<long> longs = buffer.AsImmutableArray();
        return longs;
    }

    public static byte[] LongsToBytes(ImmutableArray<long> longs)
        => longs.SelectMany(BitConverter.GetBytes).ToArray();
}
