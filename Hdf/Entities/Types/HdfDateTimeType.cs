using System.Globalization;

using AndrejKrizan.DotNet.Allocations;

namespace AndrejKrizan.Hdf.Entities.Types;

public class HdfDateTimeType : HdfStringType, IHdfType<DateTime>
{
    // Constructors
    public HdfDateTimeType() : base() { }

    // Methods
    public override string Describe()
        => "datetime string";

    public Allocation Allocate(DateTime value)
        => base.Allocate(value: Stringify(value));

    public Allocation Allocate(IEnumerable<DateTime> collection)
        => base.Allocate(collection: collection.Select(Stringify));

    public new Allocation Allocate<TRow>(IEnumerable<TRow> matrix)
        where TRow : IEnumerable<DateTime>
        => base.Allocate(matrix: matrix.Select(row => row.Select(Stringify)));

    // Static methods
    public static string Stringify(DateTime value)
        => value.ToString(Format, CultureInfo.InvariantCulture);

    // Constants
    public const string Format = "yyyy-MM-ddTHH:mm:ss.ffffffzzz";
}
