namespace AndrejKrizan.Hdf.Entities.Types;
internal static class HdfTypeFactory
{
    public static IHdfType<T> Create<T>()
        where T : notnull
    {
        Type type = typeof(T);
        IHdfType<T> hdfType = type switch
        {
            _ when type == typeof(string) => (IHdfType<T>)new HdfStringType(),
            _ when type == typeof(DateTime) => (IHdfType<T>)new HdfDateTimeType(),
            _ => new HdfType<T>(),
        };
        return hdfType;
    }
}
