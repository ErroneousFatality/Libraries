using System.Collections.Immutable;

using AndrejKrizan.DotNet.Strings;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters;

public class ImmutableArrayToStringConverter<T> : ValueConverter<ImmutableArray<T>, string>
{
    public ImmutableArrayToStringConverter(Func<string, T> selector, ConverterMappingHints? mappingHints = null)
        : base(
            (domainValue) => string.Join(Delimeter, domainValue),
            (dataValue) => dataValue.SplitToEnumerable(Delimeter, StringSplitOptions.None).Select(selector).ToImmutableArray(),
            mappingHints
        )
    { }

    private const char Delimeter = (char)31;
}
