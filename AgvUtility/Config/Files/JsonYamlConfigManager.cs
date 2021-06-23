using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Utility.Config.Files
{

    /// <summary>
    /// JSON或YAML文件格式配置读取管理器
    /// 1.支持配置文件目录监控
    /// 2.支持读取异常或写入配置异常的日志回调接口
    /// 3.支持开发模式和正式模式下的文件配置版本
    /// 4.支持配置缓存
    /// </summary>
    public abstract class JsonYamlConfigManager
    {
        /// <summary>
        /// 配置缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, Configuration> Configs;
        /// <summary>
        /// 监控器
        /// </summary>
        private static List<FileSystemWatcher> _fsWatchers;
        private static readonly object FsWatcherLock = new();
        /// <summary>
        /// Yaml序列化工具
        /// </summary>
        private static readonly Deserializer YamlDeserializer =
                new();

        /// <summary>
        ///是否是开发模式
        ///开发模式下：会读取相应配置的.dev.conf,.dev.json,.dev.yaml文件
        /// </summary>
        public static bool DevMode { get; set; }

        /// <summary>
        /// 访问配置日志正常回调
        /// </summary>
        public static Action<string> Log { get; set; }

        /// <summary>
        /// 访问配置日志异常回调
        /// </summary>
        public static Action<string, Exception> LogException { get; set; }

        /// <summary>
        /// A Utility Enum to ensure that typos aren't make for
        /// global configuration files.
        /// </summary>
        public enum ConfigKeys
        {
            /// <summary>
            /// Maps to the string of the same name.
            /// </summary>
            [SuppressMessage("ReSharper", "InconsistentNaming")] ConfigPathsConfig
        }

        static JsonYamlConfigManager()
        {
            Configs = new ConcurrentDictionary<string, Configuration>();

            /*// TJ: Add the ConfigManagerConfig file default
            // to the class, then attempt to load 
            AddConfig<ConfigPathsConfig>(
                ConfigKeys.ConfigPathsConfig.ToString());
            AddConfig<ConfigPathsConfig>(
                ConfigKeys.ConfigPathsConfig.ToString(), update: true);*/

            SetupWatchDirectories();
        }

        /// <summary>
        /// A delegate settable by the user which says how to try
        /// and get the configuration before going to the file system.
        /// </summary>
        public static Func<string, ConfigurationItem> GetConfiguration { private get; set; }

        /// <summary>
        /// A delegate settable by the user which says what to do after
        /// getting the configuration from the file system.
        /// </summary>
        public static Action<ConfigurationItem> PutConfiguration { private get; set; }

        /// <summary>
        /// Adds a configuration to the manager.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the configuration object.
        /// </typeparam>
        /// <param name="configName">
        /// The name of the configuration object to add.
        /// </param>
        /// <param name="configPath">
        /// The file path (relative or absolute) to add.
        /// </param>
        /// <param name="update">
        /// Optional boolean argument that specifies
        /// the config should be updated if it exists.
        /// </param>
        public static void AddConfig<T>(
            string configName,
            string configPath = null,
            bool update = false)
            where T : new()
        {
            if (!update && Configs.ContainsKey(configName))
            {
                return;
            }

            if (configPath == null)
            {
                 var exts = new[] {".conf", ".json", ".yaml"};
                 foreach (var ext in exts)
                 {
                     var path = configName + ext;
                     if (!FileExists(path)) continue;
                     configPath= path;
                     break;
                 }
               
            }

            Configs.AddOrUpdate(configName,
                x => CreateConfig<T>(configName, configPath),
                (x, y) =>
                {
                    if (update)
                    {
                        y = CreateConfig<T>(configName, configPath);
                    }

                    return y;
                });
        }

        /// <summary>
        /// Adds and then gets the configuration of a given name.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the configuration object.
        /// </typeparam>
        /// <param name="configName">
        /// The name of the configuration object to add.
        /// </param>
        /// <param name="configPath">
        /// The file path (relative or absolute) to add.
        /// </param>
        /// <param name="update">
        /// Optional boolean argument that specifies the
        /// config should be updated if it exists.
        /// </param>
        /// <param name="cached"></param>
        /// <returns>
        /// An object of type T with the values in the specified config file.
        /// </returns>
        public static T GetCreateConfig<T>(
            string configName,
            string configPath = null,
            bool update = false,
            bool cached = true)
            where T : new()
        {
            AddConfig<T>(configName, configPath: configPath, update: update);
            return GetConfig<T>(configName, cached);
        }

        /// <summary>
        /// Creates a Configuration of the given type
        /// from the specified file path.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the configuration object.
        /// </typeparam>
        /// <param name="configName">
        /// The name of the configuration.
        /// </param>
        /// <param name="configPath">
        /// The file path (relative or absolute) of the configuration file.
        /// </param>
        /// <returns>
        /// An object of type T with the values in the specified config file.
        /// </returns>
        private static Configuration CreateConfig<T>(
            string configName, string configPath)
            where T : new()
        {
            var config = new Configuration();

            var path = GetPath(configPath);
            var fromFile = GetConfigFromFile(configName, path);
            var fromDelegate = GetConfigFromDelegate(configName);
            HandlePutConfig(fromFile, fromDelegate);

            var newest = fromFile.LastUpdated > fromDelegate.LastUpdated
                ? fromFile
                : fromDelegate;

            config.FilePath = path;
            config.LastUpdated = newest.LastUpdated;
            config.Raw = newest.Data;

            if (config.FilePath.Length >= 5)
            {
                var extension = config
                    .FilePath
                    .Substring(config.FilePath.Length - 5, 5)
                    .ToLowerInvariant();

                config.ConfigType = extension switch
                {
                    ".conf" or ".json" => ConfigFileType.Json,
                    ".yaml" => ConfigFileType.Yaml,
                    _ => ConfigFileType.Unknown,
                };
            }

            config.Parsed = ParseConfig<T>(config.Raw, config.ConfigType);

            return config;
        }

        /// <summary>
        /// Gets a configuration item from a file.
        /// </summary>
        /// <param name="name">The name of the configuration.</param>
        /// <param name="path">The path of the file.</param>
        /// <returns>
        /// The configuration item pulled from the specified file.
        /// </returns>
        private static ConfigurationItem GetConfigFromFile(
            string name, string path)
        {
            var configItem = new ConfigurationItem
            {
                Name = name,
                Data = ReadConfig(path),
                LastUpdated = GetConfigUpdateTime(path),
                FromConfigManager = true
            };


            return configItem;
        }

        /// <summary>
        /// Get a configuration item from a delegate.
        /// </summary>
        /// <param name="name">The name of the configuration.</param>
        /// <returns>The configuration item pulled from the delegate.</returns>
        private static ConfigurationItem GetConfigFromDelegate(string name)
        {
            ConfigurationItem configItem = null;

            if (null != GetConfiguration)
            {
                configItem = GetConfiguration(name);
            }

            configItem ??= new ConfigurationItem();
            

            configItem.Name = name;

            return configItem;
        }

        private static void HandlePutConfig(
            ConfigurationItem fromFile,
            ConfigurationItem fromDelegate)
        {
            if (null != PutConfiguration
                && fromFile.Name == fromDelegate.Name
                && fromFile.LastUpdated > fromDelegate.LastUpdated
                && !string.IsNullOrEmpty(fromFile.Data))
            {
                Task.Run(() => PutConfiguration(fromFile));
            }
        }

        private static string GetPath(string configPath)
        {
            string devConfigPath = null;
            if (DevMode)
            {
                devConfigPath = configPath
                    .Replace(".conf", ".dev.conf")
                    .Replace(".json", ".dev.json")
                    .Replace(".yaml", ".dev.yaml");
            }

            if (Path.IsPathRooted(configPath)) return configPath; //已经是物理路径，直接返回
            var paths = GetConfig<ConfigPathsConfig>(
                ConfigKeys.ConfigPathsConfig.ToString()).Paths;
            foreach (var path in paths)
            {
                if (path == null || !path.Exists)
                {
                    continue;
                }
                var fi = path.EnumerateFiles(
                    devConfigPath ?? configPath,
                    SearchOption.AllDirectories).FirstOrDefault();
                if (fi == null) continue;
                 return fi.FullName;
 
            }

            return configPath;
        }

        #region 配置文件修改监视

        /// <summary>
        /// 监控配置目录
        /// </summary>
        private static void SetupWatchDirectories()
        {
            lock (FsWatcherLock)
            {
                _fsWatchers = new List<FileSystemWatcher>();

                var paths = GetConfig<ConfigPathsConfig>(
                    ConfigKeys.ConfigPathsConfig.ToString()).Paths;

                var fsEventHandler =
                    new FileSystemEventHandler(HandleUpdate);
                var errorEventHandler =
                    new ErrorEventHandler(HandleError);
                var renamedEventHandler =
                    new RenamedEventHandler(HandleRename);

                string[] fileTypes = { "*.conf", "*.json", "*.yaml" };

                _fsWatchers.AddRange(paths
                    .Where(path => path is {Exists: true})
                    .Select(path =>
                    {
                        var fileWatchers =
                            new List<FileSystemWatcher>();

                        foreach (var fileType in fileTypes)
                        {
                            var fsWatcher = new FileSystemWatcher(
                                path.FullName, fileType)
                            {
                                NotifyFilter =
                                NotifyFilters.LastWrite
                                | NotifyFilters.FileName
                                | NotifyFilters.DirectoryName,

                                IncludeSubdirectories = true
                            };

                            fsWatcher.Changed += fsEventHandler;
                            fsWatcher.Created += fsEventHandler;
                            fsWatcher.Deleted += fsEventHandler;
                            fsWatcher.Error += errorEventHandler;
                            fsWatcher.Renamed += renamedEventHandler;

                            fsWatcher.EnableRaisingEvents = true;

                            fileWatchers.Add(fsWatcher);
                        }

                        return fileWatchers;
                    })
                    .SelectMany(fileWatchers => fileWatchers));
            }
        }
        /// <summary>
        /// 停止监控配置目录
        /// </summary>
        private static void TearDownWatchDirectories()
        {
            lock (FsWatcherLock)
            {
                if (null == _fsWatchers)
                {
                    return;
                }

                foreach (var fsWatcher in _fsWatchers)
                {
                    fsWatcher.Dispose();
                }

                _fsWatchers = null;
            }
        }
        private static void HandleUpdate(object sender, FileSystemEventArgs e)
        {
            RemoveConfig(e.Name);
        }

        private static void HandleError(object sender, ErrorEventArgs e)
        {
            LogException?.Invoke(
                "Exception thrown by FileSystemWatcher",
                e.GetException());
        }

        private static void HandleRename(object sender, RenamedEventArgs e)
        {
            RemoveConfig(e.OldName);
            RemoveConfig(e.Name);
        }

        #endregion
        /// <summary>
        /// Remove the specified config file from the ConfigManager
        /// </summary>
        /// <param name="key">The specified config file key</param>
        public static void RemoveConfig(string key)
        {
            key = key.Split('\\', '/').LastOrDefault();
            if (key==null)
            {
                return;
            }
            const string conf = ".conf";
            const string json = ".json";
            const string yaml = ".yaml";
            
            if (new[] { conf, json, yaml }.Any(_ => key.EndsWith(_)))
            {

                key = key.Substring(0, key.Length - 5);
            }

            if (DevMode)
            {
                var dev = ".dev";
                if (key.EndsWith(dev))
                {
                    key = key.Substring(0, key.Length - dev.Length);
                }
            }

            Configs.TryRemove(key, out _);

            if (ConfigKeys.ConfigPathsConfig.ToString() != key) return;
            TearDownWatchDirectories();
            SetupWatchDirectories();
        }

       

        private static bool FileExists(string path)
        {
            return File.Exists(GetPath(path));
        }

        /// <summary>
        /// Returns the configuration object of the specified type and name
        /// from the in-memory dictionary of the ConfigManager.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object.
        /// </typeparam>
        /// <param name="configName">The name of the configuration
        /// object to retrieve.</param>
        /// <param name="cached">是否默认使用缓存</param>
        /// <returns>An object of type T with the values in the
        /// ConfigManager.</returns>
        public static T GetConfig<T>(string configName, bool cached = true)
            where T : new()
        {
            Configs.TryGetValue(configName, out var configuration);

            return configuration == null
                ? new T()
                : cached
                    ? (T)configuration.Parsed
                    : ParseConfig<T>(
                        configuration.Raw,
                        configuration.ConfigType);
        }

        /// <summary>
        /// Returns the DateTime that the specified file was last written to.
        /// </summary>
        /// <param name="configPath">The file path.</param>
        /// <returns>
        /// The DateTime that the specified file was last written to.
        /// </returns>
        public static DateTime GetConfigUpdateTime(string configPath)
        {
            var t = DateTime.MinValue;
            try
            {
                t = File.GetLastWriteTime(configPath);
            }
            catch (Exception e)
            {
                LogException?.Invoke(
                    "ConfigManager Error: could not read file creation time: "
                    + configPath, e);
            }

            return t;
        }

        /// <summary>
        /// Reads a relative or absolute file path and returns the contents.
        /// Nonrooted file paths are checked relative to the configured
        /// search directories.
        /// </summary>
        /// <param name="configPath">The file path.</param>
        /// <returns>The contents of the file that was found, or empty string
        /// if no file present.</returns>
        public static string ReadConfig(string configPath)
        {
            var config = "";
            // TJ: Check the configPath
            try
            {
                config = File.ReadAllText(configPath);
            }
            /*catch (System.IO.FileNotFoundException)
            {
                // Ignore
            }*/
            catch (Exception e)
            {
                LogException?.Invoke(
                    "ConfigManager Error: could not read file: "
                    + configPath, e);
            }

            return config;
        }

        /// <summary>
        /// Parses json into the specified object type.
        /// </summary>
        /// <typeparam name="T">The object type to return.</typeparam>
        /// <param name="configRaw">The raw config contents.</param>
        /// <param name="configType">The type of config file.</param>
        /// <returns>
        /// The contents of the raw json as the specified object type.
        /// </returns>
        public static T ParseConfig<T>(
            string configRaw,
            ConfigFileType configType = ConfigFileType.Json)
            where T : new()
        {
            var config = default(T);
            if (configRaw == null)
            {
                Log?.Invoke($"ConfigManager Error: Cannot deserialize null string for Type {typeof(T).Name}");
            }
            else if (configRaw != string.Empty)
            {
                switch (configType)
                {
                    case ConfigFileType.Json:
                        config = JsonConvert
                            .DeserializeObject<T>(configRaw);
                        break;
                    case ConfigFileType.Yaml:
                        config = YamlDeserializer.Deserialize<T>(
                            new StringReader(configRaw));
                        break;
                }


                if (null == config)
                {
                    // Deserializing configraw failed, log configraw
                    Log?.Invoke("ConfigManager Error: " +
                        $@"Unable to deserialize string ""{configRaw}"" to Type {typeof(T).Name}");
                }
            }

            if (config == null)
            {
                config = new T();
            }
            return config;
        }
    }
}

