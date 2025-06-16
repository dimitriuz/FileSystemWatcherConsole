using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class LockFileUntilKeyPressedProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.LockFileUntilKeyPressed;
    
    public string Source { get; init; } = string.Empty;
    
    public override string StartMessage => $"{base.StartMessage}, source: {Source}";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(RootFolder, Source);
        FileStream? fileStream = null;

        try
        {
            if (!File.Exists(source))
            {
                throw new FileNotFoundException($"File not found: {source}");
            }

            // Open the file with FileShare.None to obtain an exclusive lock
            fileStream = new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            LogInformation("File locked: {Source}. Press any key to unlock...", source);

            // Wait for key press in a non-blocking way
            await Task.Run(() =>
            {
                Console.ReadKey(intercept: true);
            });

            // Clean up and release the lock
            fileStream.Dispose();
            fileStream = null;
            LogInformation("File unlocked: {Source}", source);
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to lock file: {Source}", source);
        }
        finally
        {
            // Ensure we always dispose of the file stream if something goes wrong
            if (fileStream != null)
            {
                fileStream.Dispose();
                LogInformation("File lock released due to error: {Source}", source);
            }
        }
    }
}
