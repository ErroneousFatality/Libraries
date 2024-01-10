namespace AndrejKrizan.DotNet.Records;
public interface IRecordUnitOfWork
{
    Task ExecuteScheduledActionsAsync(CancellationToken cancellationToken = default);
}
