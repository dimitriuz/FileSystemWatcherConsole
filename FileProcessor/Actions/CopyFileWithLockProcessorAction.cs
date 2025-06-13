using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class CopyFileWithLockProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.CopyFileWithLock;
    
    public string Source { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public int LockDurationMs { get; init; } = 0;
    
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, destination: {Destination}, lock duration: {LockDurationMs}ms";

    public override async Task Handle()
    {
        var sourcePath = FileProcessorHelpers.GetPath(Source);
        var destinationPath = FileProcessorHelpers.GetPath(Destination);

        try
        {
            await Task.Run(async () =>
            {
                LogInformation("Starting file copy with lock from {Source} to {Destination}", sourcePath, destinationPath);
                
                // Open source with FileShare.Read to allow other processes to read but not write
                using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                // Open destination with FileShare.None to fully lock the file
                using var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                
                LogInformation("File locked: {Destination}", destinationPath);
                await sourceStream.CopyToAsync(destinationStream);
                LogInformation("File copy completed, maintaining lock for {Duration}ms", LockDurationMs);
                
                // Keep the file locked for specified duration
                if (LockDurationMs > 0)
                {
                    await Task.Delay(LockDurationMs);
                }
                
                LogInformation("File unlocked: {Destination}", destinationPath);
                // File will be automatically unlocked when the streams are disposed
            });
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to copy file with lock from {Source} to {Destination}", sourcePath, destinationPath);
        }
    }
}
