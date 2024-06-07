using FileSystemWatcherConsole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

string logPath = Path.Combine(@"logs\log_.json");
string consoleLogMessageTemplate = "[{@t:HH:mm:ss} {@l:u3} ({SourceContext})] {@m}\n{@x}";


var config = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Information()
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Async(w => w.File(logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                rollOnFileSizeLimit: true), bufferSize: 500)
            .WriteTo.Async(w => w.Console(new ExpressionTemplate(consoleLogMessageTemplate, theme: TemplateTheme.Code)));
Log.Logger = config.CreateLogger();


HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args);
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

builder.Services.AddSingleton<FileProcessor>();
builder.Services.AddSingleton<FileWatcherMonitor>();

builder.Services.AddHostedService<App>();

IHost host = builder.Build();

host.Run();
