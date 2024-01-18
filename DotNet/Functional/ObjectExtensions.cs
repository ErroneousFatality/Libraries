using System.Collections;

using AndrejKrizan.DotNet.Nullables;

namespace AndrejKrizan.DotNet.Functional;

public static class ObjectExtensions
{
    public static T Do<T>(this T source, Action<T> action)
    {
        action(source);
        return source;
    }

    public static T Apply<T>(this T source, Func<T, T> transform)
        => transform(source);

    public static TResult Transform<T, TResult>(this T source, Func<T, TResult> transform)
        => transform(source);

    #region ConditionallyDo
    /// <summary>Will perform the action if the condition is true.</summary>
    public static T ConditionallyDo<T>(this T source,
        bool condition, Action<T> action
    )
    {
        if (condition)
        {
            action(source);
        }
        return source;
    }

    /// <summary>Will perform the action if the argument is not null nor an empty collection.</summary>
    public static T ConditionallyDo<T, TArgument>(this T source,
        TArgument? argument, Action<TArgument, T> action
    )
        where TArgument : struct
    {
        if (!argument.TryGetValue(out TArgument _argument) || (_argument is ICollection collection && collection.Count < 1))
        {
            return source;
        }
        action(_argument, source);
        return source;
    }

    /// <summary>Will perform the action if the argument is not null nor an empty collection.</summary>
    public static T ConditionallyDo<T, TArgument>(this T source,
        TArgument? argument, Action<TArgument, T> action
    )
         where TArgument : class
    {
        if (argument == null || (argument is ICollection collection && collection.Count < 1))
        {
            return source;
        }
        action(argument, source);
        return source;
    }

    /// <summary>Will perform the action if the arguments enumerable is not null nor empty.</summary>
    public static T ConditionallyDo<T, TArgument>(this T source,
        IEnumerable<TArgument>? arguments, Action<IEnumerable<TArgument>, T> action
    )
    {
        if (arguments == null || !arguments.Any())
        {
            return source;
        }
        action(arguments, source);
        return source;
    }
    #endregion

    #region ConditionallyApply
    /// <summary>Will apply the transformation if the condition is true.</summary>
    public static T ConditionallyApply<T>(this T source,
        bool condition, Func<T, T> transform
    )
    {
        if (!condition)
        {
            return source;
        }
        T result = transform(source);
        return result;
    }

    /// <summary>Will apply the transformation if the argument is not null nor an empty collection.</summary>
    public static T ConditionallyApply<T, TArgument>(this T source,
        TArgument? argument, Func<TArgument, T, T> transform
    )
        where TArgument : struct
    {
        if (!argument.TryGetValue(out TArgument _argument) || (_argument is ICollection collection && collection.Count < 1))
        {
            return source;
        }
        T result = transform(_argument, source);
        return result;
    }

    /// <summary>Will apply the transformation if the argument is not null nor an empty collection.</summary>
    public static T ConditionallyApply<T, TArgument>(this T source,
        TArgument? argument, Func<TArgument, T, T> transform
    )
         where TArgument : class
    {
        if (argument == null || (argument is ICollection collection && collection.Count < 1))
        {
            return source;
        }
        T result = transform(argument, source);
        return result;
    }

    /// <summary>Will apply the transformation if the arguments enumerable is not null nor empty.</summary>
    public static T ConditionallyApply<T, TArgument>(this T source,
        IEnumerable<TArgument>? arguments, Func<IEnumerable<TArgument>, T, T> transform
    )
    {
        if (arguments == null || !arguments.Any())
        {
            return source;
        }
        T result = transform(arguments, source);
        return result;
    }
    #endregion
}
