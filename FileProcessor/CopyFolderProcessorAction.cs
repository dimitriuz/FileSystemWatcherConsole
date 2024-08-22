using System.Text.Json.Serialization;

namespace FileSystemWatcherConsole.FileProcessor;

public class CopyFolderProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.CopyFolder;
    public string Source { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public bool Recursive { get; init; } = false;
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, destination: {Destination}";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(Source);
        var destination = FileProcessorHelpers.GetPath(Destination);
        try
        {
            await CopyFolder(Source, Destination, Recursive);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task CopyFolder(string source, string destination, bool recursive)
    {

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
                await CopyFolder(subDir.FullName, newDestinationDir, true);
            }
        }
    }

}