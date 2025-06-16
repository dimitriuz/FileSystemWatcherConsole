using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor.Actions;

public class WaitKeyProcessorAction : ProcessorAction
{
    [JsonIgnore]
    public override ProcessorType ProcessorType => ProcessorType.WaitKey;

    public string Message { get; init; } = "Press any key to continue...";
    
    public override string StartMessage => $"{base.StartMessage}, message: {Message}";

    public override async Task Handle()
    {
        try
        {
            LogInformation("{Message}", Message);
            
            // Wait for key press in a non-blocking way
            await Task.Run(() =>
            {
                Console.Read();
            });
            
            LogInformation("Key pressed, continuing execution");
        }
        catch (Exception ex)
        {
            LogError(ex, "Error while waiting for key press");
        }
    }
}
