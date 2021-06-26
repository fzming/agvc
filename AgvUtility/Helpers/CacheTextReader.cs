using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utility.Extensions;

namespace Utility.Helpers
{
    /// <summary>
    ///     带缓存的文本读取
    ///     带文件改变监控：文件变化时将自动移除文本缓存
    /// </summary>
    public static class CacheTextReader
    {
        /// <summary>
        ///     文本缓存
        /// </summary>
        public static ConcurrentDictionary<string, string> Caches { get; set; } =
            new();

        /// <summary>
        ///     目录监视对象
        /// </summary>
        public static ConcurrentDictionary<string, FileSystemWatcher> DirectoryWatchers { get; set; } =
            new();

        /// <summary>
        ///     创建目录监控
        /// </summary>
        /// <param name="path"></param>
        private static void CreateWatcher(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (DirectoryWatchers.ContainsKey(directory ?? throw new InvalidOperationException())) return;
            var watcher = new FileSystemWatcher(directory)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName,
                EnableRaisingEvents = true
            };
            watcher.Changed += Watcher_Changed;
            DirectoryWatchers.TryAdd(directory, watcher);
        }

        /// <summary>
        ///     清理不必要的目录监视
        /// </summary>
        private static void TryCleanWatcher()
        {
            var cacheFilePaths = Caches.Keys;
            var cacheDirectories = cacheFilePaths.Select(Path.GetDirectoryName).GroupBy(p => p).Select(p => p.Key);
            var noNeedWatchFolders = DirectoryWatchers.Where(p => !cacheDirectories.Contains(p.Key)).Select(p => p.Key);
            foreach (var noNeedWatchFolder in noNeedWatchFolders)
            {
                if (!DirectoryWatchers.TryRemove(noNeedWatchFolder, out var fsWatcher)) continue;
                //dispose watcher
                fsWatcher.EnableRaisingEvents = false;
                fsWatcher.Dispose();
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            if (Caches.ContainsKey(path)) //移除缓存
            {
                //remove cache 
                Caches.TryRemove(path, out _);
                TryCleanWatcher();
            }
        }


        /// <summary>
        ///     异步读取文本内容
        /// </summary>
        /// <param name="fileName">文件名 如:cost.json</param>
        /// <param name="dir">文件目录</param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string fileName, string dir = "~/xlsx")
        {
            var textCache = string.Empty;
            var filePath = PathHelper.MapPath($"{dir.TrimEnd('/')}/{fileName}");
            if (!Caches.ContainsKey(filePath))
            {
                if (!File.Exists(filePath)) return textCache;
                textCache = await filePath.ReadAsync();
                if (textCache.IsNullOrEmpty()) return textCache;
                //write cache
                Caches.TryAdd(filePath, textCache);
                CreateWatcher(filePath);
                return textCache;
            }

            //read cache
            Caches.TryGetValue(filePath, out textCache);
            return textCache;
        }
    }
}