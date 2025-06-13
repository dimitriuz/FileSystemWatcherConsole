using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace FileSystemWatcherConsole.FileProcessor;

public class FileProcessorService
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<RuntimeConfig> _runtimeConfig;
    private readonly ILogger<FileProcessorService> _logger;

    public FileProcessorService(IConfiguration configuration,
    IOptions<RuntimeConfig> runtimeConfig,
    ILogger<FileProcessorService> logger)
    {
        _configuration = configuration;
        _runtimeConfig = runtimeConfig;
        _logger = logger;
    }

    public async Task Run()
    {
        if (string.IsNullOrEmpty(_runtimeConfig.Value.ActionsFile) || !File.Exists(_runtimeConfig.Value.ActionsFile))
        {
            _logger.LogWarning("Actions file is not specified or does not exist: {ActionsFile}", _runtimeConfig.Value.ActionsFile);
            return;
        }

        var actions = await GetActionsFromFile(_runtimeConfig.Value.ActionsFile);
        if (actions is null) return;

        foreach (var action in actions)
        {
            if (!action.Enabled) continue;

            var stopwatch = new Stopwatch();
            _logger.LogInformation(action.StartMessage);
            stopwatch.Start();
            await action.Handle();
            stopwatch.Stop();
            _logger.LogInformation("{FinishMessage}. Elapsed {ElapsedMilliseconds} ms", action.FinishMessage.Trim(), stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task<List<ProcessorAction>?> GetActionsFromFile(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            var actions = await JsonSerializer.DeserializeAsync<List<ProcessorAction>>(stream);
            if (actions is null)
            {
                _logger.LogWarning("No actions found in the file: {ActionsFile}", path);
                return null;
            }
            foreach (var action in actions)
            {
                action.Init(_logger);
            }
            return actions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Parsing actions file error");
            return null;
        }
    }

    public void WaitKey()
    {
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}