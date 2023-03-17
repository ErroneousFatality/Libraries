namespace AndrejKrizan.DotNet.Utilities.OrAsync.Conditions
{
    public class ConditionFunction : Condition
    {
        // Properties
        public Func<bool> Function { get; }

        // Constructors
        public ConditionFunction(Func<bool> function)
        {
            Function = function;
        }

        // Methods
        public override Task<bool> ToTask(CancellationToken cancellationToken = default)
            => Task.Run(Function, cancellationToken);

        // Operators
        public static implicit operator ConditionFunction(Func<bool> function) => new(function);
        public static implicit operator Func<bool>(ConditionFunction condition) => condition.Function;
    }
}
