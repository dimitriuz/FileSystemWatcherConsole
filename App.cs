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

        private async Task ClearFolder(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path))
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
            await ProcessFilesWithProcessor();
        }

        private async Task ProcessFilesWithProcessor()
        {
            // arrange
            await _fileProcessor.DeleteFolder("d:\\features\\BNB-499\\case\\2e6da928-99da-49ab-acaf-2ec0b8db5171\\TASK_f52a35d1-a757-4357-868c-20f0cbba0987\\VOL_MAR");
            _fileProcessor.WaitKey();

            //await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403.stl");
            //await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403_LowerJaw.stl");
            //await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403_UpperJaw.stl");

            /*
            ProcessFileEvent "I:\TW\5406ebc1-c9fc-4f11-a70e-776c01bd035f\.csi_data\.version_4.4\.tmp\8c2c98bc\NSP_NSP_20200111_022918\3DSlice500.dcm"
            ProcessFileEvent "I:\TW\5406ebc1-c9fc-4f11-a70e-776c01bd035f\.csi_data\.version_4.4\.tmp\8c2c98bc\NSP_NSP_20200111_022918\VOL_MAR\3DSlice500.dcm"
             */

            // act
            await _fileProcessor.CopyFolder("d:\\features\\BNB-499\\preparing\\VOL_MAR"
                , "d:\\features\\BNB-499\\case\\2e6da928-99da-49ab-acaf-2ec0b8db5171\\.csi_data\\.version_4.4\\.tmp\\VOL_MAR");

            await _fileProcessor.MoveFolder("d:\\features\\BNB-499\\case\\2e6da928-99da-49ab-acaf-2ec0b8db5171\\.csi_data\\.version_4.4\\.tmp\\VOL_MAR"
                , "d:\\features\\BNB-499\\case\\2e6da928-99da-49ab-acaf-2ec0b8db5171\\TASK_f52a35d1-a757-4357-868c-20f0cbba0987\\VOL_MAR");
            _fileProcessor.WaitKey();


            //await _fileProcessor.DeleteFile("1.stl");
            //await _fileProcessor.CopyFile("1.stl");
            //await _fileProcessor.LockFileUntilKeyPressed("1.stl");
            //await _fileProcessor.DeleteFile("1.stl");
            //await _fileProcessor.CopyFileWithLock("1.stl", "John, Dow 12.02.1987+01112023141403.stl", TimeSpan.FromSeconds(1));
            //await _fileProcessor.DeleteFile("John, Dow 12.02.1987+01112023141403.stl");

            //await Task.WhenAll
            //    (
            //    _fileProcessor.CopyFileWithLock("2.stl", "John, Dow 12.02.1987+01112023141403_LowerJaw.stl", TimeSpan.FromSeconds(10)),
            //    _fileProcessor.CopyFileWithLock("3.stl", "John, Dow 12.02.1987+01112023141403_UpperJaw.stl")
            //    );

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
