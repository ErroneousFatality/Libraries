using System.Collections.Immutable;

using AndrejKrizan.DotNet.Strings;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters;

public class StringListToStringConverter : ValueConverter<List<string>, string>
{
    public StringListToStringConverter(ConverterMappingHints? mappingHints = null)
        : base(
            (domainValue) => string.Join(Delimeter, domainValue),
            (dataValue) => dataValue.SplitToEnumerable(Delimeter, StringSplitOptions.None).ToList(),
            mappingHints
        )
    { }

    public static ValueConverterInfo DefaultInfo { get; }
        = new ValueConverterInfo(typeof(ImmutableArray<string>), typeof(string), info => new StringImmutableArrayToStringConverter(info.MappingHints));

    private const char Delimeter = (char)31;
}
