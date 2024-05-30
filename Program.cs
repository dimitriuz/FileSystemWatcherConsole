using FileSystemWatcherConsole;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddCommandLine(args);
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss.fff ";
});

builder.Services.AddSingleton<FileProcessor>();
builder.Services.AddSingleton<FileWatcherMonitor>();

//builder.Services.AddHostedService<FileWatcherMonitor>();
builder.Services.AddHostedService<App>();

IHost host = builder.Build();

host.Run();
