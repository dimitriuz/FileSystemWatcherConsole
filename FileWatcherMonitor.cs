// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;

public class FileWatcherMonitor
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileWatcherMonitor> _logger;
    private FileSystemWatcher _fileSystemWatcher;

    public FileWatcherMonitor(IConfiguration configuration, ILogger<FileWatcherMonitor> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await StartFileWatcher(_configuration.GetSection("path").Value);

        cancellationToken.Register(() => 
        { 
            _fileSystemWatcher.Dispose(); 
        });
    }

    private async Task StartFileWatcher(string path)
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

        _fileSystemWatcher.Created += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Changed += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Renamed += ProcessFileSystemWatcherRenamedEvent;
        _fileSystemWatcher.Deleted += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Error += ProcessFileSystemError;

        _logger.LogInformation("Starting to watch folder {path}", path);
        _fileSystemWatcher.EnableRaisingEvents = true;
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