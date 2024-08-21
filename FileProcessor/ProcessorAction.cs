using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "processorType", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
[JsonDerivedType(typeof(CopyFileProcessorAction), typeDiscriminator: nameof(ProcessorType.CopyFile))]
public abstract class ProcessorAction
{
    //protected readonly ILogger<ProcessorStep> _logger;
    public ProcessorAction()
    {
        //_logger = logger;
    }
    [JsonIgnore]
    public abstract ProcessorType ProcessorType { get; }
    public string Name { get; set; } = string.Empty;
    public abstract Task Handle();
    public virtual string StartMessage => $"Starting action {ProcessorType}{(string.IsNullOrEmpty(Name) ? "" : $"({Name})")}";
    public virtual string FinishMessage => $"Finished action {ProcessorType}{(string.IsNullOrEmpty(Name) ? "" : $"({Name})")}";
}
