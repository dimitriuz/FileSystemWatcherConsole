using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class CopyFileProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.CopyFile;
    public string Source { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, destination: {Destination}";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(RootFolder, Source);
        var destination = FileProcessorHelpers.GetPath(RootFolder, Destination);
        try
        {
            LogInformation("Copying file from {Source} to {Destination}", source, destination);
            await FileProcessorHelpers.CopyFileAsync(source, destination);
            LogInformation("File copied successfully");
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to copy file from {Source} to {Destination}", source, destination);
        }
    }
}