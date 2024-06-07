using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FileSystemWatcherConsole
{
    public class App : IHostedService, IHostedLifecycleService
    {
        private readonly FileProcessor _fileProcessor;
        private readonly FileWatcherMonitor _fileMonitor;
        private readonly ILogger<App> _logger;
        private CancellationTokenSource _ct = new CancellationTokenSource();

        public App(FileProcessor fileProcessor, IHostApplicationLifetime lifetime, FileWatcherMonitor fileMonitor, ILogger<App> logger)
        {
            _fileProcessor = fileProcessor;
            _fileMonitor = fileMonitor;
            _logger = logger;
        }

        private async Task ClearDestinationFolder()
        {
            foreach (var file in Directory.EnumerateFiles("files"))
            {
                await _fileProcessor.DeleteFile(Path.GetFileName(file));
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
      
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _ct.Cancel();
            return Task.CompletedTask;
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StartedAsync(CancellationToken cancellationToken)
        {
            await _fileMonitor.StartAsync(_ct.Token);
            //await ProcessFilesWithProcessor();
        }

        private async Task ProcessFilesWithProcessor()
        {
            await ClearDestinationFolder();
            await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403.stl");
            await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403_LowerJaw.stl");
            await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403_UpperJaw.stl");
            _fileProcessor.WaitKey();

            //await _fileProcessor.DeleteFile("1.stl");
            //await _fileProcessor.CopyFile("1.stl");
            //await _fileProcessor.LockFileUntilKeyPressed("1.stl");
            //_fileProcessor.WaitKey();
            //await _fileProcessor.DeleteFile("1.stl");
            await _fileProcessor.CopyFileWithLock("1.stl", "John, Dow 12.02.1987+01112023141403.stl", TimeSpan.FromSeconds(1));
            await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403.stl");

            await Task.WhenAll
                (
                _fileProcessor.CopyFileWithLock("2.stl", "John, Dow 12.02.1987+01112023141403_LowerJaw.stl", TimeSpan.FromSeconds(10)),
                _fileProcessor.CopyFileWithLock("3.stl", "John, Dow 12.02.1987+01112023141403_UpperJaw.stl")
                );

            //_logger.LogInformation("All tasks done. Press Ctrl+C to close the app");
        }

        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
