using System.CommandLine;
using FileSystemWatcherConsole;
using FileSystemWatcherConsole.FileProcessor;
using FileSystemWatcherConsole.FileWatcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;

string logPath = Path.Combine(@"logs\log_.json");
string consoleLogMessageTemplate = "[{@t:HH:mm:ss} {@l:u3} ({ SourceContext}){@x}] {@m}\n"; //({ SourceContext}){@x}

var loggerConfig = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Async(w => w.File(logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                rollOnFileSizeLimit: true), bufferSize: 500)
            .WriteTo.Async(w => w.Console(new ExpressionTemplate(consoleLogMessageTemplate, theme: TemplateTheme.Code)));

Log.Logger = loggerConfig.CreateLogger();

var pathArgument = new Argument<string>(
      name: "path",

      description: "The directory to watch.");

var processFileOption = new Option<string?>(
           name: "--actions",
           description: "The json file to process actions.",
           parseArgument: result =>
           {
               string? path = result.Tokens.SingleOrDefault()?.Value;

               if (result.Tokens.Count == 0 || string.IsNullOrEmpty(path) || !File.Exists(path))
               {
                   result.ErrorMessage = "Actions file does not exist";
                   return null;
               }

               else
               {
                   return path;
               }
           }
           );

var rootCommand = new RootCommand();
rootCommand.Add(pathArgument);
rootCommand.Add(processFileOption);

Log.Information("Application started. Press Ctrl+C to exit");

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", true);

rootCommand.SetHandler((pathArgumentValue, processFileOptionValue) =>
    {
        RuntimeConfig runtimeConfig = new RuntimeConfig()
        {
            Path = pathArgumentValue,
            ActionsFile = processFileOptionValue,
        };
        builder.Services.AddSingleton(Options.Create<RuntimeConfig>(runtimeConfig));
    }, pathArgument, processFileOption
    );



builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.Services.AddSingleton<FileProcessorService>();
builder.Services.AddSingleton<FileWatcherService>();

builder.Services.AddHostedService<App>();

bool commandLineValidated = await rootCommand.InvokeAsync(args) == 0;

if (commandLineValidated)
{
    IHost host = builder.Build();
    host.Run();
}
