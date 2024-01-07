using System.Linq.Expressions;
using System.Reflection;
using AndrejKrizan.DotNet.PropertyNavigations;

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

    #region GetFromStaticProperty
    public static T? GetFromStaticProperty<TObject, T>(PropertyNavigation<TObject, T> navigation)
        => (T?)navigation.Info.GetValue(null);

    public static T? GetFromStaticProperty<TObject, T>(Expression<Func<TObject, T>> selector)
        => GetFromStaticProperty(new PropertyNavigation<TObject, T>(selector));

    public static T? GetFromStaticProperty<TObject, T>(string name)
        => (T?)GetFromStaticProperty<TObject>(name);
    public static object? GetFromStaticProperty<TObject>(string name)
        => GetFromStaticProperty(typeof(TObject), name);

    public static T? GetFromStaticProperty<T>(Type objectType, string name)
        => (T?)GetFromStaticProperty(objectType, name);

    public static object? GetFromStaticProperty(Type objectType, string name)
    {
        PropertyInfo propertyInfo = objectType.GetProperty(name, BindingFlags.Public | BindingFlags.Static)
            ?? throw new ArgumentException($"There is no public static property {name} in {objectType.Name}", nameof(name));
        object? value = propertyInfo.GetValue(null);
        return value;
    }
    #endregion
}
