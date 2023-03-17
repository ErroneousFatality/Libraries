namespace AndrejKrizan.DotNet.Utilities.OrAsync.Conditions
{
    public class ConditionAsyncFunction : Condition
    {
        // Properties
        public Func<Task<bool>> AsyncFunction { get; }

        // Constructors
        public ConditionAsyncFunction(Func<Task<bool>> asyncFunction)
        {
            AsyncFunction = asyncFunction;
        }

        // Methods
        public override Task<bool> ToTask(CancellationToken cancellationToken = default)
            => Task.Run(AsyncFunction, cancellationToken);

        // Operators
        public static implicit operator ConditionAsyncFunction(Func<Task<bool>> asyncFunction) => new(asyncFunction);
        public static implicit operator Func<Task<bool>>(ConditionAsyncFunction condition) => condition.AsyncFunction;
    }
}
