using FileSystemWatcherConsole.FileProcessor;
using FileSystemWatcherConsole.FileWatcher;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileSystemWatcherConsole;

public class App : IHostedService, IHostedLifecycleService
{
    private readonly FileProcessorService _fileProcessor;
    private readonly FileWatcherService _fileWatcher;
    private readonly IOptions<AppArguments> _runtimeConfig;
    private readonly ILogger<App> _logger;
    private CancellationTokenSource _ct = new CancellationTokenSource();

    public App(
        FileProcessorService fileProcessor,
        FileWatcherService fileWatcher,
        IOptions<AppArguments> runtimeConfig,
        ILogger<App> logger
    )
    {
        _fileProcessor = fileProcessor;
        _fileWatcher = fileWatcher;
        _runtimeConfig = runtimeConfig;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application started. Press Ctrl+C to exit");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _ct.Cancel();
        return Task.CompletedTask;
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        await _fileWatcher.StartAsync(_ct.Token);
        if (!string.IsNullOrEmpty(_runtimeConfig.Value.ActionsFile))
        {
            await ProcessFilesWithProcessor();
        }
    }

    private async Task ProcessFilesWithProcessor()
    {
        await _fileProcessor.Run();
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping application...");
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
