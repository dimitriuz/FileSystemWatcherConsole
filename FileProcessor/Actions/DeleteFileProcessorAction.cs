using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class DeleteFileProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.DeleteFile;
    
    public string Source { get; init; } = string.Empty;
    
    public override string StartMessage => $"{base.StartMessage}, source: {Source}";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(RootFolder, Source);
        try
        {
            await Task.Run(() =>
            {
                if (!File.Exists(source))
                {
                    LogInformation("File not found: {Source}", source);
                    return;
                }

                LogInformation("Deleting file: {Source}", source);
                // Use FileOptions.DeleteOnClose for more efficient deletion
                using var fileStream = new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete, 4096, FileOptions.DeleteOnClose);
                LogInformation("File deleted successfully: {Source}", source);
            });
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to delete file: {Source}", source);
        }
    }
}
