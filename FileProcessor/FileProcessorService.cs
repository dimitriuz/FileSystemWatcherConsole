using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace FileSystemWatcherConsole.FileProcessor;
public class FileProcessorService
{
    IConfiguration _configuration;
    private readonly IOptions<RuntimeConfig> _runtimeConfig;
    private readonly ILogger<FileProcessorService> _logger;
    private string _destinationPath;
    //private List<ProcessorStep> _processorSteps = new();

    public FileProcessorService(IConfiguration configuration, IOptions<RuntimeConfig> runtimeConfig, ILogger<FileProcessorService> logger)
    {
        _configuration = configuration;
        this._runtimeConfig = runtimeConfig;
        _logger = logger;
        _destinationPath = runtimeConfig.Value.Path;


        //generate temp file

        //_processorSteps.Add(new CopyFileProcessorStep() { Source = @"c:\temp", Destination = @"d:\tmp" });
        //_processorSteps.Add(new CopyFileProcessorStep() { Source = @"d:\temp", Destination = @"d:\tmp2" });
        //var s = JsonSerializer.Serialize<ProcessorStep[]>(_processorSteps.ToArray());
        //var a = JsonSerializer.Deserialize<ProcessorStep[]>(s);

    }

    public async Task Run()
    {
        var actions = await GetActionsFromFile(_runtimeConfig.Value.ActionsFile);
        if (actions is null) return;

        foreach (var action in actions)
        {
            if (!action.Enabled) continue;

            var stopwatch = new Stopwatch();
            Console.WriteLine(action.StartMessage);
            stopwatch.Start();
            await action.Handle();
            stopwatch.Stop();
            Console.WriteLine($"{action.FinishMessage.Trim()}. Elapsed {stopwatch.ElapsedMilliseconds} ms");
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

    private string GetSourcePath(string filename)
    {
        string source;

        if (!Path.IsPathFullyQualified(filename))
        {
            source = @$"files\{filename}";
        }
        else
        {
            source = filename;
        };

        return source;
    }
    private string GetDestinationPath(string filename) => Path.Combine(_destinationPath, filename);


    public async Task CopyFile(string filename, string? destinationFilename = null)
    {
        _logger.LogInformation("Copying file {source}", filename);

        var source = GetSourcePath(filename);
        var destination = GetDestinationPath(destinationFilename is null ? filename : destinationFilename);
        File.Copy(source, destination);
    }

    public async Task CopyFolder(string sourceDir, string? destinationFilename = null, bool recursive = true)
    {
        var source = GetSourcePath(sourceDir);

        _logger.LogInformation("Copying folder {source}", source);

        var destination = GetDestinationPath(destinationFilename is null ? sourceDir : destinationFilename);

        var dir = new DirectoryInfo(source);

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destination);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destination, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destination, subDir.Name);
                CopyFolder(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    public async Task MoveFolder(string sourceDir, string? destinationFilename = null)
    {
        var source = GetSourcePath(sourceDir);

        _logger.LogInformation("Moving folder {source}", source);

        var destination = GetDestinationPath(destinationFilename is null ? sourceDir : destinationFilename);

        Directory.Move(source, destination);
    }

    public async Task CopyFileWithLock(string filename, string? destinationFilename = null, TimeSpan delay = default)
    {
        var source = GetSourcePath(filename);
        var destination = GetDestinationPath(destinationFilename is null ? filename : destinationFilename);
        _logger.LogInformation("Copying file with lock {source}", filename);

        using var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.None);
        using var destinationStream = new FileStream(destination, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        await sourceStream.CopyToAsync(destinationStream);
        await Task.Delay(delay);
        _logger.LogInformation("File unlocked {source}", destination);
    }


    public Task DeleteFolder(string path)
    {
        var source = GetSourcePath(path);
        if (Directory.Exists(source))
        {
            _logger.LogInformation("Deleting directory {sourceDir}", source);
            Directory.Delete(source, true);
        }

        return Task.CompletedTask;
    }


    public async Task LockFileUntilKeyPressed(string filename)
    {
        var file = GetDestinationPath(filename);
        var _lock = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
        _logger.LogInformation("File locked {sourceDir}", filename);
        WaitKey();
        if (_lock != null)
        {
            _lock.Dispose();
            _logger.LogInformation("File unlocked {sourceDir}", filename);
        }
    }

    public async Task LockFile(string filename, TimeSpan time)
    {
        var file = GetDestinationPath(filename);
        var _lock = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
        _logger.LogInformation("File locked {sourceDir}", filename);
        await Task.Delay(time);
        if (_lock != null)
        {
            _lock.Dispose();
            _logger.LogInformation("File unlocked {sourceDir}", filename);
        }
    }

    public void WaitKey()
    {
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}