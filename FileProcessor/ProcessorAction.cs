using System.Text.Json.Serialization;

namespace FileSystemWatcherConsole.FileProcessor;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "processorType", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
[JsonDerivedType(typeof(CopyFileProcessorAction), typeDiscriminator: nameof(ProcessorType.CopyFile))]
[JsonDerivedType(typeof(CopyFolderProcessorAction), typeDiscriminator: nameof(ProcessorType.CopyFolder))]
[JsonDerivedType(typeof(CreateAndUpdateFileFromTemplateProcessorAction), typeDiscriminator: nameof(ProcessorType.CreateAndUpdateFileFromTemplate))]
public abstract class ProcessorAction
{
    [JsonIgnore]
    public abstract ProcessorType ProcessorType { get; }
    public bool Enabled { get; set; }
    public string Name { get; set; } = string.Empty;
    public abstract Task Handle();
    public virtual string StartMessage => $"Action started: {ProcessorType}{(string.IsNullOrEmpty(Name) ? "" : $"({Name})")}";
    public virtual string FinishMessage => $"Action finished: {ProcessorType}{(string.IsNullOrEmpty(Name) ? "" : $"({Name})")}";
}
