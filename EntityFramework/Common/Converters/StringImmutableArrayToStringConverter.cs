using System.Collections.Immutable;

using AndrejKrizan.Common.Extensions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters
{
    public class StringImmutableArrayToStringConverter : ValueConverter<ImmutableArray<string>, string>
    {
        public StringImmutableArrayToStringConverter(ConverterMappingHints? mappingHints = null)
            : base(
                (strings) => string.Join(Delimeter, strings),
                (strings) => strings.Split(Delimeter, StringSplitOptions.RemoveEmptyEntries).ToImmutableArray(),
                mappingHints
            )
        { }

        public static ValueConverterInfo DefaultInfo { get; }
            = new ValueConverterInfo(typeof(ImmutableArray<string>), typeof(string), info => new StringImmutableArrayToStringConverter(info.MappingHints));

        private const char Delimeter = (char)31;
    }
}
