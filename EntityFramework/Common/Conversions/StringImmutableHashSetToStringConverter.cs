using System.Collections.Immutable;

using AndrejKrizan.DotNet.Strings;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Conversions;

public class StringImmutableHashSetToStringConverter : ValueConverter<ImmutableHashSet<string>, string>
{
    public StringImmutableHashSetToStringConverter(ConverterMappingHints? mappingHints = null)
        : base(
            (domainValue) => string.Join(Delimeter, domainValue),
            (dataValue) => dataValue.SplitToEnumerable(Delimeter, StringSplitOptions.None).ToImmutableHashSet(),
            mappingHints
        )
    { }

    public static ValueConverterInfo DefaultInfo { get; }
        = new ValueConverterInfo(typeof(ImmutableHashSet<string>), typeof(string), info => new StringImmutableHashSetToStringConverter(info.MappingHints));

    private const char Delimeter = (char)31;
}
