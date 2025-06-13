namespace FileSystemWatcherConsole.FileProcessor;

public static class FileProcessorHelpers
{
    public static string GetPath(string filename)
    {
        string source;

        if (!Path.IsPathFullyQualified(filename))
        {
            source = @$"files\{filename}";
        }
        else
        {
            source = filename;
        }
        ;

        return source;
    }

    public static async Task ClearFolder(string path)
    {
        foreach (var file in Directory.EnumerateFiles(path))
        {
            await DeleteFile(Path.GetFileName(file));
        }
    }

    public static Task DeleteFile(string filename)
    {
        var file = GetPath(filename);
        if (File.Exists(file))
        {
            File.Delete(file);
        }
        return Task.CompletedTask;
    }

    public static async Task CopyFileAsync(string sourceFile, string destinationFile)
    {
        const int bufferSize = 81920;
        
        using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
        
        await sourceStream.CopyToAsync(destinationStream, bufferSize);
    }
}
