using System.Globalization;

using AndrejKrizan.Common.Extensions;
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
            => CreatePointable(collection.Convert(Stringify));

        public Pointable CreatePointable(IReadOnlyCollection<IReadOnlyCollection<DateTime>> matrix)
            => CreatePointable(matrix.Convert(row =>
                    (IReadOnlyCollection<string>)row.Convert(Stringify)
                )
            );

        // Static methods
        public static string Stringify(DateTime value)
            => value.ToString(Format, CultureInfo.InvariantCulture);

        // Constants
        public const string Format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";
    }
}
