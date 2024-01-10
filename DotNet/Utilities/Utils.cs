using System.Globalization;

namespace AndrejKrizan.DotNet.Utilities;

public static partial class Utils
{
    public static T Min<T>(T first, T second, IComparer<T> comparer)
        => comparer.Compare(first, second) <= 0 ? first : second;
    public static T Min<T>(T first, T second)
        => Min(first, second, Comparer<T>.Default);

    public static T Max<T>(T first, T second, IComparer<T> comparer)
        => comparer.Compare(first, second) >= 0 ? first : second;
    public static T Max<T>(T first, T second)
        => Max(first, second, Comparer<T>.Default);

    public static T CreateDefaultInstance<T>()
    => (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;

    public static T GetFromDefaultInstance<TObject, T>(Func<TObject, T> selector)
        => selector(CreateDefaultInstance<TObject>());

    #region ConvertTo
    public static T ConvertTo<T>(object obj)
        => ConvertTo<T>(obj, CultureInfo.CurrentCulture);


    public static T ConvertTo<T>(object source, IFormatProvider? provider)
    {
        Type targetType = typeof(T);

        if (source is Guid guid)
        {
            source = guid.ToString();
        }

        if (targetType == typeof(Guid))
        {
            string? str = source.ToString();
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("The string representation of the source object is null or empty.", nameof(source));
            }
            guid = Guid.Parse(str);
            return (T)(object)guid;
        }

        T converted = (T)Convert.ChangeType(source, targetType, provider);
        return converted;
    }

    #endregion
}
