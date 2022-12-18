using System.Collections.Immutable;
using System.Globalization;

using AndrejKrizan.Common.ValueObjects.Pointables;

namespace AndrejKrizan.Hdf.Entities.Types
{
    public class HdfDateTimeType : HdfStringType, IHdfType<DateTime>
    {
        // Constructors
        public HdfDateTimeType() : base() { }

        // Methods
        public override string Describe()
            => "datetime string";

        public Pointable CreatePointable(DateTime value)
            => CreatePointable(Stringify(value));

        public Pointable CreatePointable(IReadOnlyCollection<DateTime> collection)
            => CreatePointable(collection.Select(Stringify).ToImmutableArray());

        public Pointable CreatePointable(IReadOnlyCollection<IReadOnlyCollection<DateTime>> matrix)
            => CreatePointable(matrix
                .Select(row => (IReadOnlyCollection<string>)row
                    .Select(Stringify)
                    .ToImmutableArray()
                )
                .ToImmutableArray());

        // Static methods
        public static string Stringify(DateTime value)
            => value.ToString(Format, CultureInfo.InvariantCulture);

        // Constants
        public const string Format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";
    }
}
