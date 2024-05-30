using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

public class FileProcessor
{
    IConfiguration _configuration;
    private readonly ILogger<FileProcessor> _logger;
    private string destinationPath;
    private string GetSourcePath(string filename) => @$"files\{filename}";
    private string GetDestinationPath(string filename) => Path.Combine(destinationPath, filename);
    public FileProcessor(IConfiguration configuration, ILogger<FileProcessor> logger)
    {
        _configuration = configuration;
        _logger = logger;
        destinationPath = _configuration.GetSection("path").Value;
    }

    public async Task CopyFile(string filename, string? destinationFilename = null)
    {
        _logger.LogInformation("Copying file {source}", filename);

        var source = GetSourcePath(filename);
        var destination = GetDestinationPath(destinationFilename is null ? filename : destinationFilename);
        File.Copy(source, destination);
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
        _logger.LogInformation("File unlocked {source}", filename);
    }

    public Task DeleteFile(string filename)
    {
        var file = GetDestinationPath(filename);
        if (File.Exists(file))
        {
            _logger.LogInformation("Deleting file {filename}", filename);
            File.Delete(file);
        }
        return Task.CompletedTask;
    }

    public async Task LockFileUntilKeyPressed(string filename)
    {
        var file = GetDestinationPath(filename);
        var _lock = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
        _logger.LogInformation("File locked {filename}", filename);
        WaitKey();
        if (_lock != null)
        {
            _lock.Dispose();
            _logger.LogInformation("File unlocked {filename}", filename);
        }
    }

    public async Task LockFile(string filename, TimeSpan time)
    {
        var file = GetDestinationPath(filename);
        var _lock = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
        _logger.LogInformation("File locked {filename}", filename);
        await Task.Delay(time);
        if (_lock != null)
        {
            _lock.Dispose();
            _logger.LogInformation("File unlocked {filename}", filename);
        }
    }

    public void WaitKey()
    {
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}