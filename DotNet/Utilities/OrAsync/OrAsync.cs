using AndrejKrizan.DotNet.Utilities.OrAsync.Conditions;

namespace AndrejKrizan.DotNet.Utilities
{
    public static partial class Utils
    {
#pragma warning disable CA1068 // CancellationToken parameters must come last
        public static Task<bool> OrAsync(CancellationToken cancellationToken, Condition condition1, Condition condition2, params Condition[] additionalConditions)
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1), cancellationToken);
#pragma warning restore CA1068 // CancellationToken parameters must come last

        public static Task<bool> OrAsync(Condition condition1, Condition condition2, params Condition[] additionalConditions)
            => OrAsync(additionalConditions.Prepend(condition2).Prepend(condition1));

        public static Task<bool> OrAsync(IEnumerable<Condition> conditions, CancellationToken cancellationToken = default)
            => OrAsyncInternal(
                (cancellationToken) => conditions.Select(condition => condition.ToTask(cancellationToken)),
                cancellationToken
            );


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
                (cancellationToken) => conditions.Select(
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
                (cancellationToken) => conditions.Select(
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
                (cancellationToken) => conditions.Select(
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
                (cancellationToken) => conditions,
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
    }
}
