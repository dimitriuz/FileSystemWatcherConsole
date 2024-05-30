using Microsoft.Extensions.Hosting;

namespace FileSystemWatcherConsole
{
    public class App : IHostedService, IHostedLifecycleService
    {
        private readonly FileProcessor _fileProcessor;
        private readonly FileWatcherMonitor _fileMonitor;
        private CancellationToken ct = new CancellationToken();

        public App(FileProcessor fileProcessor, IHostApplicationLifetime lifetime, FileWatcherMonitor fileMonitor)
        {
            _fileProcessor = fileProcessor;
            _fileMonitor = fileMonitor;
            lifetime.ApplicationStarted.Register(OnStarted);
        }

        private async void OnStarted()
        {
            await Task.Delay(500);
            await _fileMonitor.StartAsync(ct);
            _fileProcessor.DeleteFile("1.stl");
            _fileProcessor.CopyFile("1.stl");
            _fileProcessor.WaitKey();
            _fileProcessor.DeleteFile("1.stl");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StartedAsync(CancellationToken cancellationToken)
        {
            return; // Task.CompletedTask;
            await Task.Delay(500);
            _fileProcessor.DeleteFile("1.stl");
            _fileProcessor.CopyFile("1.stl");
            _fileProcessor.WaitKey();
            _fileProcessor.DeleteFile("1.stl");
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
