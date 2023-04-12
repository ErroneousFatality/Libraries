using System.Collections.Immutable;

using AndrejKrizan.DotNet.Extensions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters;

public class DoubleImmutableArrayToBytesConverter : ValueConverter<ImmutableArray<double>, byte[]>
{
    public DoubleImmutableArrayToBytesConverter(ConverterMappingHints? mappingHints = null)
        : base(
            (doubles) => DoublesToBytes(doubles),
            (bytes) => BytesToDoubles(bytes),
            mappingHints
        )
    { }

    // Methods
    public static ImmutableArray<double> BytesToDoubles(byte[] bytes)
    {
        double[] buffer = new double[bytes.Length / 8];
        Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
        ImmutableArray<double> doubles = buffer.AsImmutableArray();
        return doubles;
    }

    public static byte[] DoublesToBytes(ImmutableArray<double> doubles)
        => doubles.SelectMany(BitConverter.GetBytes).ToArray();
}
