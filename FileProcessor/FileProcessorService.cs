using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace FileSystemWatcherConsole.FileProcessor;
public class FileProcessorService
{
    IConfiguration _configuration;
    private readonly IOptions<RuntimeConfig> _runtimeConfig;
    private readonly ILogger<FileProcessorService> _logger;

    public FileProcessorService(IConfiguration configuration, IOptions<RuntimeConfig> runtimeConfig, ILogger<FileProcessorService> logger)
    {
        _configuration = configuration;
        _runtimeConfig = runtimeConfig;
        _logger = logger;
    }

    public async Task Run()
    {
        var actions = await GetActionsFromFile(_runtimeConfig.Value.ActionsFile);
        if (actions is null) return;

        foreach (var action in actions)
        {
            if (!action.Enabled) continue;

            var stopwatch = new Stopwatch();
            _logger.LogInformation(action.StartMessage);
            stopwatch.Start();
            await action.Handle();
            stopwatch.Stop();
            _logger.LogInformation("{FinishMessage}. Elapsed {ElapsedMilliseconds} ms", action.FinishMessage.Trim(), stopwatch.ElapsedMilliseconds);
        };
    }

    private async Task<List<ProcessorAction>?> GetActionsFromFile(string path)
    {
        using var stream = new FileStream(_runtimeConfig.Value.ActionsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            return await JsonSerializer.DeserializeAsync<List<ProcessorAction>>(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Parsing actions file error");
            return null;
        }
    }

    // public async Task MoveFolder(string sourceDir, string? destinationFilename = null)
    // {
    //     var source = GetSourcePath(sourceDir);

    //     _logger.LogInformation("Moving folder {source}", source);

    //     var destination = GetDestinationPath(destinationFilename is null ? sourceDir : destinationFilename);

    //     Directory.Move(source, destination);
    // }

    // public async Task CopyFileWithLock(string filename, string? destinationFilename = null, TimeSpan delay = default)
    // {
    //     var source = GetSourcePath(filename);
    //     var destination = GetDestinationPath(destinationFilename is null ? filename : destinationFilename);
    //     _logger.LogInformation("Copying file with lock {source}", filename);

    //     using var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.None);
    //     using var destinationStream = new FileStream(destination, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    //     await sourceStream.CopyToAsync(destinationStream);
    //     await Task.Delay(delay);
    //     _logger.LogInformation("File unlocked {source}", destination);
    // }


    // public Task DeleteFolder(string path)
    // {
    //     var source = GetSourcePath(path);
    //     if (Directory.Exists(source))
    //     {
    //         _logger.LogInformation("Deleting directory {sourceDir}", source);
    //         Directory.Delete(source, true);
    //     }

    //     return Task.CompletedTask;
    // }


    // public async Task LockFileUntilKeyPressed(string filename)
    // {
    //     var file = GetDestinationPath(filename);
    //     var _lock = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
    //     _logger.LogInformation("File locked {sourceDir}", filename);
    //     WaitKey();
    //     if (_lock != null)
    //     {
    //         _lock.Dispose();
    //         _logger.LogInformation("File unlocked {sourceDir}", filename);
    //     }
    // }

    // public async Task LockFile(string filename, TimeSpan time)
    // {
    //     var file = GetDestinationPath(filename);
    //     var _lock = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
    //     _logger.LogInformation("File locked {sourceDir}", filename);
    //     await Task.Delay(time);
    //     if (_lock != null)
    //     {
    //         _lock.Dispose();
    //         _logger.LogInformation("File unlocked {sourceDir}", filename);
    //     }
    // }

    public void WaitKey()
    {
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}