namespace AndrejKrizan.Hosting.Services;

public interface IService
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
