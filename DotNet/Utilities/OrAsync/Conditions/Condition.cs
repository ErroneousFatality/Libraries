namespace AndrejKrizan.DotNet.Utilities.OrAsync.Conditions
{
    public abstract class Condition
    {
        // Methods
        public abstract Task<bool> ToTask(CancellationToken cancellationToken = default);

        // Operators
        public static implicit operator Condition(bool value)
            => new ConditionValue(value);

        public static implicit operator Condition(Task<bool> task)
            => new ConditionTask(task);

        public static implicit operator Condition(Func<bool> function)
            => new ConditionFunction(function);

        public static implicit operator Condition(Func<Task<bool>> asyncFunction)
            => new ConditionAsyncFunction(asyncFunction);

        public static implicit operator Condition(Func<CancellationToken, Task<bool>> cancellableAsyncFunction)
            => new ConditionCancellableAsyncFunction(cancellableAsyncFunction);
    }
}
