using System.Globalization;

using AndrejKrizan.DotNet.ValueObjects.Pointables;

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
            => base.CreatePointable(value: Stringify(value));

        public Pointable CreatePointable(IEnumerable<DateTime> collection)
            => base.CreatePointable(collection: collection.Select(Stringify));

        public new Pointable CreatePointable<TRow>(IEnumerable<TRow> matrix)
            where TRow : IEnumerable<DateTime>
            => base.CreatePointable(matrix: matrix.Select(row => row.Select(Stringify)));

        // Static methods
        public static string Stringify(DateTime value)
            => value.ToString(Format, CultureInfo.InvariantCulture);

        // Constants
        public const string Format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";
    }
}
