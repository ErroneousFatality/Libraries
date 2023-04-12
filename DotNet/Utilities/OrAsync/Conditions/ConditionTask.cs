namespace AndrejKrizan.DotNet.Utilities.OrAsync.Conditions;

public class ConditionTask : Condition
{
    // Properties
    public Task<bool> Task { get; }

    // Constructors
    public ConditionTask(Task<bool> task)
    {
        Task = task;
    }

    // Methods
    public override Task<bool> ToTask(CancellationToken cancellationToken = default)
        => Task;

    // Operators
    public static implicit operator ConditionTask(Task<bool> task) => new(task);
    public static implicit operator Task<bool>(ConditionTask condition) => condition.Task;
}
