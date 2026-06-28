namespace AndrejKrizan.Hosting.Applications;

public interface IApplication
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
