using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class DeleteFolderProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.DeleteFolder;
    
    public string Source { get; init; } = string.Empty;
    public bool Recursive { get; init; } = true;
    
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, recursive: {Recursive}";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(RootFolder, Source);
        try
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(source))
                {
                    LogInformation("Directory not found: {Source}", source);
                    return;
                }

                LogInformation("Deleting directory: {Source} (Recursive: {Recursive})", source, Recursive);
                // If recursive is true, delete directory and all contents
                // If recursive is false and directory is not empty, this will throw an exception
                Directory.Delete(source, Recursive);
                LogInformation("Directory deleted successfully: {Source}", source);
            });
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to delete directory: {Source}", source);
        }
    }
}
