using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.DotNet.Utilities.Func;

namespace AndrejKrizan.DotNet.Utilities;

public static partial class Utils
{
    // Methods
    public static TResult CallFunc<T, TResult>(Func<T, TResult> func)
        => func(Mock<T>());

    public static TResult CallFunc<T, TResult>(string name, params object?[] parameters)
        => CallFunc<T, TResult>(typeof(T).GetMethod(name, BindingFlags.NonPublic)!, parameters);

    public static TResult CallFunc<T, TResult>(Expression<Func<T, Func<TResult>>> navigation, params object?[] parameters)
        => CallFunc<T, TResult>(new MethodInfoFinder().FindIn(navigation), parameters);

    public static TResult CallFunc<T, TResult>(MethodInfo info, params object?[] parameters)
        => (TResult)info.Invoke(Mock<T>(), parameters)!;

    // Private methods
    private static T Mock<T>()
        => (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;

}
