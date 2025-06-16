using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class LockFileProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.LockFile;
    
    public string Source { get; init; } = string.Empty;
    public int LockDurationMs { get; init; } = 0;
    
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, lock duration: {LockDurationMs}ms";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(RootFolder, Source);
        try
        {
            await Task.Run(async () =>
            {
                if (!File.Exists(source))
                {
                    throw new FileNotFoundException($"File not found: {source}");
                }

                // Open the file with FileShare.None to obtain an exclusive lock
                using var fileStream = new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                
                LogInformation("File locked: {Source}", source);
                
                if (LockDurationMs > 0)
                {
                    await Task.Delay(LockDurationMs);
                }
                
                // File will be automatically unlocked when the stream is disposed
                LogInformation("File unlocked: {Source}", source);
            });
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to lock file: {Source}", source);
        }
    }
}
