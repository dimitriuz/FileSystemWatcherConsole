using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class CopyFolderProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.CopyFolder;
    public string Source { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public bool Recursive { get; init; } = false;
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, destination: {Destination}, recursive: {Recursive}";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(RootFolder, Source);
        var destination = FileProcessorHelpers.GetPath(RootFolder, Destination);
        try
        {
            if (!Directory.Exists(source))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {source}");
            }

            LogInformation("Copying folder from {Source} to {Destination} (Recursive: {Recursive})", source, destination, Recursive);
            await CopyFolderAsync(source, destination, Recursive);
            LogInformation("Folder copied successfully");
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to copy folder from {Source} to {Destination}", source, destination);
        }
    }

    private async Task CopyFolderAsync(string source, string destination, bool recursive)
    {
        var dir = new DirectoryInfo(source);

        // Create the destination directory
        Directory.CreateDirectory(destination);
        LogInformation("Created destination directory: {Destination}", destination);

        // Copy all files asynchronously
        var files = dir.GetFiles();
        LogInformation("Copying {Count} files from {Source}", files.Length, source);
        
        var fileCopyTasks = files.Select(async file =>
        {
            string targetFilePath = Path.Combine(destination, file.Name);
            await FileProcessorHelpers.CopyFileAsync(file.FullName, targetFilePath);
            LogInformation("Copied file: {Source} -> {Destination}", file.Name, targetFilePath);
        });

        // Wait for all file copies to complete
        await Task.WhenAll(fileCopyTasks);

        // If recursive, process subdirectories
        if (recursive)
        {
            var subdirs = dir.GetDirectories();
            LogInformation("Processing {Count} subdirectories recursively", subdirs.Length);
            
            var subdirCopyTasks = subdirs.Select(subDir =>
            {
                string newDestinationDir = Path.Combine(destination, subDir.Name);
                return CopyFolderAsync(subDir.FullName, newDestinationDir, true);
            });

            await Task.WhenAll(subdirCopyTasks);
        }
    }
}