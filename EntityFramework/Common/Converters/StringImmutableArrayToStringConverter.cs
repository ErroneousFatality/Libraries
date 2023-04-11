using System.Collections.Immutable;

using AndrejKrizan.DotNet.Extensions;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AndrejKrizan.EntityFramework.Common.Converters
{
    public class StringImmutableArrayToStringConverter : ValueConverter<ImmutableArray<string>, string>
    {
        public StringImmutableArrayToStringConverter(ConverterMappingHints? mappingHints = null)
            : base(
                (domainValue) => string.Join(Delimeter, domainValue),
                (dataValue) => dataValue.Split(Delimeter, StringSplitOptions.None).AsImmutableArray(),
                mappingHints
            )
        { }

        public static ValueConverterInfo DefaultInfo { get; }
            = new ValueConverterInfo(typeof(ImmutableArray<string>), typeof(string), info => new StringImmutableArrayToStringConverter(info.MappingHints));

        private const char Delimeter = (char)31;
    }
}
