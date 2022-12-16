using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters
{
    public class LongsToBytesConverter : ValueConverter<ImmutableArray<long>, byte[]>
    {
        public LongsToBytesConverter(ConverterMappingHints? mappingHints = null)
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
            ImmutableArray<long> longs = Unsafe.As<long[], ImmutableArray<long>>(ref buffer);
            return longs;
        }

        public static byte[] LongsToBytes(ImmutableArray<long> longs)
            => longs.SelectMany(BitConverter.GetBytes).ToArray();
    }
}
