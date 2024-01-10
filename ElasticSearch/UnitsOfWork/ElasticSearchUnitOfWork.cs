using AndrejKrizan.DotNet.AsyncActions;
using AndrejKrizan.DotNet.Records;

namespace AndrejKrizan.ElasticSearch.UnitsOfWork;
public abstract class ElasticSearchUnitOfWork : IRecordUnitOfWork, IDisposable
{
    // Fields
    private readonly List<AsyncAction> ScheduledActions = [];

    // Constructors
    public ElasticSearchUnitOfWork() { }

    // Methods
    public async Task ExecuteScheduledActionsAsync(CancellationToken cancellationToken = default)
    {
        foreach (AsyncAction actAsync in ScheduledActions)
        {
            await actAsync(cancellationToken);
        }
        ScheduledActions.Clear();
    }

    public void Dispose()
    {
        if (ScheduledActions.Count > 0)
        {
            throw new Exception($"The text search unit of work contains {ScheduledActions.Count} unexecuted scheduled actions during its disposal.");
        }
        GC.SuppressFinalize(this);
    }

    // Internal methods
    protected internal void ScheduleAction(AsyncAction action)
        => ScheduledActions.Add(action);

    protected internal void ScheduleActions(IEnumerable<AsyncAction> actions)
        => ScheduledActions.AddRange(actions);
}
