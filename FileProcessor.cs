using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class FileProcessor
{
    IConfiguration _configuration;
    private readonly ILogger<FileProcessor> _logger;
    private string destination;
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
        File.Copy(source, Path.Combine(destination, filename));
    }

    public void DeleteFile(string filename)
    {
        if (File.Exists(filename))
        {
            _logger.LogInformation("Deleting file {filename}", filename);
            File.Delete(Path.Combine(destination, filename));
        }
    }

    public void WaitKey()
    {
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}