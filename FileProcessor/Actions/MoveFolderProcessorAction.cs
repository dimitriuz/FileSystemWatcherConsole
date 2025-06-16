using System.IO;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class MoveFolderProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.MoveFolder;
    public string Source { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, destination: {Destination}";

    public override Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(RootFolder, Source);
        var destination = FileProcessorHelpers.GetPath(RootFolder, Destination);
        try
        {
            LogInformation("Moving folder from {Source} to {Destination}", source, destination);
            MoveFolder(source, destination);
            LogInformation("Folder moved successfully");
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to move folder from {Source} to {Destination}", source, destination);
        }
        return Task.CompletedTask;
    }

    private void MoveFolder(string sourcePath, string destinationPath)
    {
        if (!Directory.Exists(sourcePath))
        {
            throw new DirectoryNotFoundException($"Source directory not found: {sourcePath}");
        }

        if (Directory.Exists(destinationPath))
        {
            LogInformation("Destination exists, deleting: {Destination}", destinationPath);
            Directory.Delete(destinationPath, true);
        }

        Directory.Move(sourcePath, destinationPath);
    }
}