using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace FileSystemWatcherConsole.FileWatcher;
public class FileWatcherService
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<RuntimeConfig> _runtimeConfig;
    private readonly ILogger<FileWatcherService> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private FileSystemWatcher _fileSystemWatcher;

    public FileWatcherService(IConfiguration configuration, IOptions<RuntimeConfig> runtimeConfig, ILogger<FileWatcherService> logger, IHostApplicationLifetime lifetime)
    {
        _configuration = configuration;
        _runtimeConfig = runtimeConfig;
        _logger = logger;
        _lifetime = lifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartFileWatcher(_runtimeConfig.Value.Path);

        cancellationToken.Register(() =>
        {
            _fileSystemWatcher?.Dispose();
        });
    }

    private async Task StartFileWatcher(string path)
    {
        try
        {
            _fileSystemWatcher = new FileSystemWatcher
            {
                Path = path,

                NotifyFilter = NotifyFilters.Attributes |
                          NotifyFilters.DirectoryName |
                          NotifyFilters.FileName |
                          NotifyFilters.LastWrite |
                          NotifyFilters.Security |
                          NotifyFilters.Size,

                IncludeSubdirectories = true,
                InternalBufferSize = 64 * 1024
            };
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, ex.Message);
            _lifetime.StopApplication();
            return;
        }

        _fileSystemWatcher.Created += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Changed += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Renamed += ProcessFileSystemWatcherRenamedEvent;
        _fileSystemWatcher.Deleted += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Error += ProcessFileSystemError;

        _fileSystemWatcher.EnableRaisingEvents = true;

        _logger.LogInformation("Start monitoring the folder {path}", path);
    }

    private void ProcessFileSystemError(object sender, ErrorEventArgs e)
    {
        _logger.LogError("FileSystemWatcher exception raised {exception}", e.GetException());
    }

    private void ProcessFileSystemWatcherEvent(object e, FileSystemEventArgs args)
    {
        _logger.LogInformation("{ChangeType} event fired for file {path}", args.ChangeType, args.FullPath);
    }

    private void ProcessFileSystemWatcherRenamedEvent(object e, RenamedEventArgs args)
    {
        _logger.LogInformation("{ChangeType} event fired for file {OldFullPath}, new name {FullPath}", args.ChangeType, args.OldFullPath, args.FullPath);
    }
}