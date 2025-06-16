using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FileSystemWatcherConsole.FileProcessor;

public class FileProcessorService
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<AppArguments> _runtimeConfig;
    private readonly ILogger<FileProcessorService> _logger;

    public FileProcessorService(IConfiguration configuration,
    IOptions<AppArguments> runtimeConfig,
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

        var actions = await GetActionsFromFile(_runtimeConfig.Value);
        if (actions is null) return;

        foreach (var action in actions)
        {
            if (!action.Enabled)
            {
                _logger.LogInformation("Action {ActionName} is disabled, skipping", action.Name);
                continue;
            }

            var stopwatch = new Stopwatch();
            _logger.LogInformation(action.StartMessage);
            stopwatch.Start();
            await action.Handle();
            stopwatch.Stop();
            _logger.LogInformation("{FinishMessage}. Elapsed {ElapsedMilliseconds} ms", action.FinishMessage.Trim(), stopwatch.ElapsedMilliseconds);
        }
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.DeserializeAsync<TValue>(Stream, JsonSerializerOptions, CancellationToken)")]
    private async Task<List<ProcessorAction>?> GetActionsFromFile(AppArguments config)
    {
        using var stream = new FileStream(config.ActionsFile!, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            var actions = await JsonSerializer.DeserializeAsync<List<ProcessorAction>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            if (actions is null)
            {
                _logger.LogWarning("No actions found in the file: {ActionsFile}", config.ActionsFile);
                return null;
            }
            foreach (var action in actions)
            {
                action.Init(config.Path, _logger);
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