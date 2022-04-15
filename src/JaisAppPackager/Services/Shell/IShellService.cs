namespace JaisAppPackager.Services.Shell;

public interface IShellService
{
    void ExecuteWithoutResult(string command);

    Task<(int ExitCode, string? Output, string? ErrorOutput, Exception? Exception)> Execute(string command,
        Action<string>? outputReceived = null, Action<string>? errorOutputReceived = null);
}