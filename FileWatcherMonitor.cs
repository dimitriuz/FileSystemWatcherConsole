// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;

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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return StartFileWatcher(_configuration.GetSection("path").Value);
    }

    public async Task StartFileWatcher(string path)
    {
        _fileSystemWatcher = new FileSystemWatcher
        {
            Path = path,

            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,

            IncludeSubdirectories = true,

            InternalBufferSize = 64 * 1024
        };

        _fileSystemWatcher.Created += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Changed += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Renamed += ProcessFileSystemWatcherEvent;
        _fileSystemWatcher.Deleted += ProcessFileSystemWatcherEvent;

        _logger.LogInformation("Starting to watch folder {path}", path);
        _fileSystemWatcher.EnableRaisingEvents = true;
    }

    void ProcessFileSystemWatcherEvent(object e, FileSystemEventArgs args)
    {
        _logger.LogInformation("{ChangeType} event fired for file {path}", args.ChangeType, args.FullPath);
    }
}