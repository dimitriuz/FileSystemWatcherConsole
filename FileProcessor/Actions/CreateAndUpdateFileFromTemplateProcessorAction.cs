using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class CreateAndUpdateFileFromTemplateProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.CreateAndUpdateFileFromTemplate;
    public string SourceForCreate { get; init; } = string.Empty;
    public string SourceForUpdate { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public int DelayInMs { get; init; } = 0;

    public override string StartMessage => $"{base.StartMessage}, source for create: {SourceForCreate}, source for update: {SourceForUpdate}, destination: {Destination}";

    public override async Task Handle()
    {
        var sourceForCreatePath = FileProcessorHelpers.GetPath(RootFolder, SourceForCreate);
        var sourceForUpdatePath = FileProcessorHelpers.GetPath(RootFolder, SourceForUpdate);
        var destinationPath = FileProcessorHelpers.GetPath(RootFolder, Destination);

        try
        {
            await Task.Run(async () =>
            {
                LogInformation("Creating file from template. Source: {SourceForCreate}, Destination: {Destination}", 
                    sourceForCreatePath, destinationPath);

                using var sourceForCreateStream = new FileStream(sourceForCreatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var destinationStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                await sourceForCreateStream.CopyToAsync(destinationStream);
                destinationStream.Close();
                
                LogInformation("Initial file created. Waiting {DelayMs}ms before update", DelayInMs);
                await Task.Delay(DelayInMs);

                LogInformation("Updating file from template. Source: {SourceForUpdate}, Destination: {Destination}", 
                    sourceForUpdatePath, destinationPath);
                    
                using var sourceForUpdateStream = new FileStream(sourceForUpdatePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var destinationStreamForUpdate = new FileStream(destinationPath, FileMode.Open, FileAccess.Write, FileShare.Write);
                await sourceForUpdateStream.CopyToAsync(destinationStreamForUpdate);
                destinationStreamForUpdate.Close();

                LogInformation("File successfully created and updated: {Destination}", destinationPath);
            });
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed to create/update file from template. Create source: {SourceForCreate}, Update source: {SourceForUpdate}, Destination: {Destination}", 
                sourceForCreatePath, sourceForUpdatePath, destinationPath);
        }
    }
}