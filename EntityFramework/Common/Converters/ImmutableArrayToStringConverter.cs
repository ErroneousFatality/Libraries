using System.Collections.Immutable;

using AndrejKrizan.DotNet.Extensions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters
{
    public class ImmutableArrayToStringConverter<T> : ValueConverter<ImmutableArray<T>, string>
    {
        public ImmutableArrayToStringConverter(Func<string, T> parser, ConverterMappingHints? mappingHints = null)
            : base(
                (domainValue) => string.Join(Delimeter, domainValue),
                (dataValue) => dataValue
                    .SplitToEnumerable(Delimeter)
                    .Select(parser)
                    .ToImmutableArray(),
                mappingHints
            )
        { }

        private const char Delimeter = (char)31;
    }
}
