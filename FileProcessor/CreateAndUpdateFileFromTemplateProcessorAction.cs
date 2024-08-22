using System.Text.Json.Serialization;

namespace FileSystemWatcherConsole.FileProcessor;

public class CreateAndUpdateFileFromTemplateProcessorAction : ProcessorAction
{
    public CreateAndUpdateFileFromTemplateProcessorAction() : base()
    {
    }
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.CreateAndUpdateFileFromTemplate;
    public string SourceForCreate { get; init; } = string.Empty;
    public string SourceForUpdate { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public int DelayInMs { get; init; } = 0;

    public override string StartMessage => $"{base.StartMessage}, source for create: {SourceForCreate}, source for update: {SourceForUpdate}, destination: {Destination}";

    public override async Task Handle()
    {
        //_logger.LogInformation("Copying file {source}", Source);

        var sourceForCreatePath = FileProcessorHelpers.GetPath(SourceForCreate);
        var sourceForUpdatePath = FileProcessorHelpers.GetPath(SourceForUpdate);
        var destinationPath = FileProcessorHelpers.GetPath(Destination);

        try
        {
            await Task.Run(async () =>
            {
                using var sourceForCreateStream = new FileStream(sourceForCreatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sourceForUpdateStream = new FileStream(sourceForUpdatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var destinationStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                sourceForCreateStream.CopyTo(destinationStream);
                destinationStream.Close();
                await Task.Delay(DelayInMs);
                using var destinationStreamForUpdate = new FileStream(destinationPath, FileMode.Open, FileAccess.Write, FileShare.Write);
                sourceForUpdateStream.CopyTo(destinationStreamForUpdate);
                destinationStreamForUpdate.Close();
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}