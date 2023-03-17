namespace AndrejKrizan.DotNet.Utilities.OrAsync.Conditions
{
    public class ConditionCancellableAsyncFunction : Condition
    {
        // Properties
        public Func<CancellationToken, Task<bool>> CancellableAsyncFunction { get; }

        // Constructors
        public ConditionCancellableAsyncFunction(Func<CancellationToken, Task<bool>> cancellableAsyncFunction)
        {
            CancellableAsyncFunction = cancellableAsyncFunction;
        }

        // Methods
        public override Task<bool> ToTask(CancellationToken cancellationToken = default)
            => Task.Run(() => CancellableAsyncFunction(cancellationToken), cancellationToken);

        // Operators
        public static implicit operator ConditionCancellableAsyncFunction(Func<CancellationToken, Task<bool>> cancellableAsyncFunction) => new(cancellableAsyncFunction);
        public static implicit operator Func<CancellationToken, Task<bool>>(ConditionCancellableAsyncFunction condition) => condition.CancellableAsyncFunction;
    }
}
