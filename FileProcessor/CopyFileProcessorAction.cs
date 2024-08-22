using System.Text.Json.Serialization;

namespace FileSystemWatcherConsole.FileProcessor;

public class CopyFileProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.CopyFile;
    public string Source { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public override string StartMessage => $"{base.StartMessage}, source: {Source}, destination: {Destination}";

    public override async Task Handle()
    {
        var source = FileProcessorHelpers.GetPath(Source);
        var destination = FileProcessorHelpers.GetPath(Destination);
        try
        {
            await Task.Run(() => File.Copy(source, destination));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}