using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services
{
    /// <summary>Watches plugin files and coalesces deployment changes into one reload.</summary>
    internal sealed class PluginHotReloadService : IDisposable
    {
        private static readonly HashSet<string> WatchedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".config",
            ".dll",
            ".json",
            ".pdb"
        };

        private readonly object _syncRoot = new();
        private readonly string _pluginsFolder;
        private readonly TimeSpan _reloadDelay;
        private readonly Func<CancellationToken, Task> _reload;
        private readonly CancellationToken _stoppingToken;
        private FileSystemWatcher _watcher;
        private CancellationTokenSource _debounceCancellation;
        private bool _disposed;

        /// <summary>Creates a debounced plugin folder watcher.</summary>
        public PluginHotReloadService(
            string pluginsFolder,
            TimeSpan reloadDelay,
            Func<CancellationToken, Task> reload,
            CancellationToken stoppingToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pluginsFolder);
            ArgumentNullException.ThrowIfNull(reload);

            _pluginsFolder = Path.GetFullPath(pluginsFolder);
            _reloadDelay = reloadDelay;
            _reload = reload;
            _stoppingToken = stoppingToken;
        }

        /// <summary>Starts watching the plugin directory.</summary>
        public void Start()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            Directory.CreateDirectory(_pluginsFolder);

            _watcher = new FileSystemWatcher(_pluginsFolder)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.DirectoryName |
                    NotifyFilters.FileName |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Size
            };
            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;
            _watcher.EnableRaisingEvents = true;

            Logger.Log.LogInformation(
                "Plugin hot reload is watching {PluginsFolder} with a {ReloadDelayMilliseconds} ms debounce",
                _pluginsFolder, _reloadDelay.TotalMilliseconds);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _watcher?.Dispose();
                _debounceCancellation?.Cancel();
                _debounceCancellation?.Dispose();
                _debounceCancellation = null;
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs args)
        {
            if (ShouldReload(args.FullPath))
            {
                ScheduleReload(args.ChangeType, args.FullPath);
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs args)
        {
            if (ShouldReload(args.FullPath) || ShouldReload(args.OldFullPath))
            {
                ScheduleReload(WatcherChangeTypes.Renamed, args.FullPath);
            }
        }

        private static bool ShouldReload(string path)
        {
            var extension = Path.GetExtension(path);
            return string.IsNullOrEmpty(extension) || WatchedExtensions.Contains(extension);
        }

        private void ScheduleReload(WatcherChangeTypes changeType, string path)
        {
            CancellationToken token;
            lock (_syncRoot)
            {
                if (_disposed || _stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                _debounceCancellation?.Cancel();
                _debounceCancellation?.Dispose();
                _debounceCancellation = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
                token = _debounceCancellation.Token;
            }

            Logger.Log.LogDebug(
                "Observed plugin file change {ChangeType} at {PluginPath}; scheduling reload",
                changeType, path);
            _ = ReloadAfterDelayAsync(token);
        }

        private async Task ReloadAfterDelayAsync(CancellationToken debounceToken)
        {
            try
            {
                await Task.Delay(_reloadDelay, debounceToken).ConfigureAwait(false);
                await _reload(_stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (debounceToken.IsCancellationRequested ||
                                                     _stoppingToken.IsCancellationRequested)
            {
                // A newer file event superseded this reload, or the worker is stopping.
            }
            catch (Exception exception)
            {
                Logger.Log.LogError(exception, "Unhandled error while hot reloading plugins");
            }
        }

        private void OnError(object sender, ErrorEventArgs args)
        {
            Logger.Log.LogError(args.GetException(),
                "Plugin file watcher reported an error; subsequent file changes may require a worker restart");
        }
    }
}
