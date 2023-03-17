using System.Diagnostics.CodeAnalysis;

namespace AndrejKrizan.DotNet
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

        public static T Min<T>(T first, T second, IComparer<T> comparer)
            => comparer.Compare(first, second) <= 0 ? first : second;
        public static T Min<T>(T first, T second)
            => Min(first, second, Comparer<T>.Default);

        public static T Max<T>(T first, T second, IComparer<T> comparer)
            => comparer.Compare(first, second) >= 0 ? first : second;
        public static T Max<T>(T first, T second)
            => Max(first, second, Comparer<T>.Default);

        #region OrAsync

#pragma warning disable CA1068 // CancellationToken parameters must come last
        public static Task<bool> OrAsync(
            CancellationToken cancellationToken, 
            Func<CancellationToken, Task<bool>> condition1, 
            Func<CancellationToken, Task<bool>> condition2, 
            params Func<CancellationToken, Task<bool>>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1), cancellationToken);
#pragma warning restore CA1068 // CancellationToken parameters must come last

        public static Task<bool> OrAsync(
            Func<CancellationToken, Task<bool>> condition1, 
            Func<CancellationToken, Task<bool>> condition2, 
            params Func<CancellationToken, Task<bool>>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1));

        public static Task<bool> OrAsync(IEnumerable<Func<CancellationToken, Task<bool>>> conditions, CancellationToken cancellationToken = default)
            => OrAsyncInternal(
                (CancellationToken cancellationToken) => conditions.Select(
                    conditionTaskFunc => Task.Run(() => conditionTaskFunc(cancellationToken), cancellationToken)
                ),
                cancellationToken
            );


#pragma warning disable CA1068 // CancellationToken parameters must come last
        public static Task<bool> OrAsync(
            CancellationToken cancellationToken,
            Func<Task<bool>> condition1,
            Func<Task<bool>> condition2,
            params Func<Task<bool>>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1), cancellationToken);
#pragma warning restore CA1068 // CancellationToken parameters must come last

        public static Task<bool> OrAsync(
            Func<Task<bool>> condition1,
            Func<Task<bool>> condition2,
            params Func<Task<bool>>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1));

        public static Task<bool> OrAsync(IEnumerable<Func<Task<bool>>> conditions, CancellationToken cancellationToken = default)
            => OrAsyncInternal(
                (CancellationToken cancellationToken) => conditions.Select(
                    conditionTaskFunc => Task.Run(conditionTaskFunc, cancellationToken)
                ),
                cancellationToken
            );


#pragma warning disable CA1068 // CancellationToken parameters must come last
        public static Task<bool> OrAsync(
            CancellationToken cancellationToken,
            Func<bool> condition1,
            Func<bool> condition2,
            params Func<bool>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1), cancellationToken);
#pragma warning restore CA1068 // CancellationToken parameters must come last

        public static Task<bool> OrAsync(
            Func<bool> condition1,
            Func<bool> condition2,
            params Func<bool>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1));

        public static Task<bool> OrAsync(IEnumerable<Func<bool>> conditions, CancellationToken cancellationToken = default)
            => OrAsyncInternal(
                (CancellationToken cancellationToken) => conditions.Select(
                    conditionTaskFunc => Task.Run(conditionTaskFunc, cancellationToken)
                ),
                cancellationToken
            );


#pragma warning disable CA1068 // CancellationToken parameters must come last
        public static Task<bool> OrAsync(
            CancellationToken cancellationToken,
            Task<bool> condition1,
            Task<bool> condition2,
            params Task<bool>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1), cancellationToken);
#pragma warning restore CA1068 // CancellationToken parameters must come last

        public static Task<bool> OrAsync(
            Task<bool> condition1,
            Task<bool> condition2,
            params Task<bool>[] additionalConditions
        )
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1));

        public static Task<bool> OrAsync(IEnumerable<Task<bool>> conditions, CancellationToken cancellationToken = default)
            => OrAsyncInternal(
                (CancellationToken cancellationToken) => conditions,
                cancellationToken
            );


        private static async Task<bool> OrAsyncInternal(Func<CancellationToken, IEnumerable<Task<bool>>> createConditionTasks, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            IEnumerable<Task<bool>> conditionTasks = createConditionTasks(cancellationSource.Token);
            HashSet<Task<bool>> conditionTaskSet = new(conditionTasks);
            while (conditionTaskSet.Count > 0)
            {
                Task<bool> completedConditionTask = await Task.WhenAny(conditionTaskSet);
                cancellationToken.ThrowIfCancellationRequested();
                if (completedConditionTask.Result)
                {
                    cancellationSource.Cancel();
                    return true;
                }
                conditionTaskSet.Remove(completedConditionTask);
            }
            return false;
        }
        #endregion
    }
}
