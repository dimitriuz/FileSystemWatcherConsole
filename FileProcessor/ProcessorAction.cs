using System.Text.Json.Serialization;
using FileSystemWatcherConsole.FileProcessor.Actions;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole.FileProcessor;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "processorType", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]
[JsonDerivedType(typeof(CopyFileProcessorAction), typeDiscriminator: nameof(ProcessorType.CopyFile))]
[JsonDerivedType(typeof(CopyFolderProcessorAction), typeDiscriminator: nameof(ProcessorType.CopyFolder))]
[JsonDerivedType(typeof(MoveFolderProcessorAction), typeDiscriminator: nameof(ProcessorType.MoveFolder))]
[JsonDerivedType(typeof(CopyFileWithLockProcessorAction), typeDiscriminator: nameof(ProcessorType.CopyFileWithLock))]
[JsonDerivedType(typeof(DeleteFileProcessorAction), typeDiscriminator: nameof(ProcessorType.DeleteFile))]
[JsonDerivedType(typeof(DeleteFolderProcessorAction), typeDiscriminator: nameof(ProcessorType.DeleteFolder))]
[JsonDerivedType(typeof(LockFileProcessorAction), typeDiscriminator: nameof(ProcessorType.LockFile))]
[JsonDerivedType(typeof(CreateAndUpdateFileFromTemplateProcessorAction), typeDiscriminator: nameof(ProcessorType.CreateAndUpdateFileFromTemplate))]
public abstract class ProcessorAction
{
    protected ILogger Logger{ get; private set; } = null!;

    internal void Init(ILogger logger)
    {
        Logger = logger;
    }

    [JsonIgnore]
    public abstract ProcessorType ProcessorType { get; }
    public bool Enabled { get; set; }
    public string Name { get; set; } = string.Empty;
    public abstract Task Handle();
    public virtual string StartMessage => $"Action started: {ProcessorType}{(string.IsNullOrEmpty(Name) ? "" : $"({Name})")}";
    public virtual string FinishMessage => $"Action finished: {ProcessorType}{(string.IsNullOrEmpty(Name) ? "" : $"({Name})")}";

    protected virtual void LogError(Exception ex, string message, params object[] args)
    {
        Logger.LogError(ex, message, args);
    }

    protected virtual void LogInformation(string message, params object[] args)
    {
        Logger.LogInformation(message, args);
    }
}
