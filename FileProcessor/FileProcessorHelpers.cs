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
        };

        return source;
    }
    //public static string GetDestinationPath(string filename, string destinationPath) => Path.Combine(destinationPath, filename);

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

}
