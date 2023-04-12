namespace AndrejKrizan.DotNet.Utilities.OrAsync.Conditions;

public class ConditionValue : Condition
{
    // Properties
    public bool Value { get; }

    // Constructors
    public ConditionValue(bool value)
    {
        Value = value;
    }

    // Methods
    public override Task<bool> ToTask(CancellationToken cancellationToken = default)
        => Task.FromResult(Value);

    // Operators
    public static implicit operator ConditionValue(bool value) => new(value);
    public static implicit operator bool(ConditionValue condition) => condition.Value;
}
