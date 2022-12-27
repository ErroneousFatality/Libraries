using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.Common
{
    public static class Utils
    {
        #region RetryWithDelays
        public static async Task RetryWithDelaysAsync(
            Func<int, CancellationToken, Task> asyncAction,
            Action<int, Exception> exceptionHandler,
            int[] millisecondDelays,
            CancellationToken cancellationToken = default
        )
        {
            int tryNumber;
            for (int i = 0; i < millisecondDelays.Length; i++)
            {
                tryNumber = i + 1;
                try
                {
                    await asyncAction(tryNumber, cancellationToken);
                    return;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception exception)
                {
                    exceptionHandler(tryNumber, exception);
                    await Task.Delay(millisecondDelays[i], cancellationToken);
                }
            }
            tryNumber = millisecondDelays.Length + 1;
            try
            {
                await asyncAction(tryNumber, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception exception)
            {
                exceptionHandler(tryNumber, exception);
                throw;
            }
        }
        public static async Task RetryWithDelaysAsync(
            Func<int, CancellationToken, Task> asyncAction,
            Action<int, Exception> exceptionHandler,
            params int[] millisecondDelays
        )
            => await RetryWithDelaysAsync(asyncAction, exceptionHandler, millisecondDelays, cancellationToken: default);


        public static async Task RetryWithDelaysAsync(
            Func<int, Task> asyncAction,
            Action<int, Exception> exceptionHandler,
            int[] millisecondDelays,
            CancellationToken cancellationToken = default
        )
            => await RetryWithDelaysAsync(
                asyncAction: (tryNumber, cancellationToken) => asyncAction(tryNumber),
                exceptionHandler,
                millisecondDelays,
                cancellationToken
            );
        public static async Task RetryWithDelaysAsync(
            Func<int, Task> asyncAction,
            Action<int, Exception> exceptionHandler,
            params int[] millisecondDelays
        )
            => await RetryWithDelaysAsync(asyncAction, exceptionHandler, millisecondDelays, cancellationToken: default);


        public static async Task RetryWithDelaysAsync(Func<int, Task> asyncAction, int[] millisecondDelays, CancellationToken cancellationToken = default)
        {
            List<Exception> exceptions = new(millisecondDelays.Length + 1);
            try
            {
                await RetryWithDelaysAsync(
                    asyncAction,
                    (tryNumber, exception) => exceptions.Add(exception),
                    millisecondDelays,
                    cancellationToken
                );
            }
            catch
            {
                throw new AggregateException(exceptions);
            }
        }
        public static async Task RetryWithDelaysAsync(
            Func<int, Task> asyncAction,
            params int[] millisecondDelays
        )
            => await RetryWithDelaysAsync(asyncAction, millisecondDelays, cancellationToken: default);
        #endregion RetryWithDelays

        public static void ListInitializeOrAdd<T>([NotNull] ref List<T>? list, T item)
        {
            if (list == null)
            {
                list = new List<T> { item };
            }
            else
            {
                list.Add(item);
            }
        }

        public static T Min<T>(T first, T second, Comparer<T> comparer)
            => comparer.Compare(first, second) <= 0 ? first : second;
        public static T Min<T>(T first, T second)
            => Min(first, second, Comparer<T>.Default);

        public static T Max<T>(T first, T second, Comparer<T> comparer)
            => comparer.Compare(first, second) >= 0 ? first : second;
        public static T Max<T>(T first, T second)
            => Max(first, second, Comparer<T>.Default);
    }
}
