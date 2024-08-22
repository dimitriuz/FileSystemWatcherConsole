namespace FileSystemWatcherConsole.FileProcessor;

public enum ProcessorType
{
    CopyFile,
    CopyFolder,
    MoveFolder,
    CopyFileWithLock,
    DeleteFile,
    DeleteFolder,
    LockFileUntilKeyPressed,
    LockFile,
    WaitKey,
    CreateAndUpdateFileFromTemplate
}