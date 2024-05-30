using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class FileProcessor
{
    IConfiguration _configuration;
    private readonly ILogger<FileProcessor> _logger;
    private string destination;
    private string GetSourcePath(string source) => @$"files\{source}";
    public FileProcessor(IConfiguration configuration, ILogger<FileProcessor> logger)
    {
        _configuration = configuration;
        _logger = logger;
        destination = _configuration.GetSection("path").Value;
    }

    public void CopyFile(string source)
    {
        _logger.LogInformation("Copying file {source}", source);
        var filename = Path.GetFileName(source);
        File.Copy(GetSourcePath(source), Path.Combine(destination, filename));
    }

    public void DeleteFile(string filename)
    {
        var source = Path.Combine(destination, filename);
        if (File.Exists(source))
        {
            _logger.LogInformation("Deleting file {filename}", filename);
            File.Delete(source);
        }
    }

    public void WaitKey()
    {
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}