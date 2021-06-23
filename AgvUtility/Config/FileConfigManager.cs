using System.IO;
using System.Linq;
using Utility.Config.Files;

namespace Utility.Config
{
    /// <summary>
    /// 文件配置管理类
    /// </summary>
    public class FileConfigManager:IConfigManager
    {
        /// <summary>
        /// 是否是开发模式
        /// </summary>
        public bool DevMode { get; set; }

        public DirectoryInfo BaseDirectory
        {
            get
            {
                var paths =JsonYamlConfigManager.GetConfig<ConfigPathsConfig>(
                    JsonYamlConfigManager.ConfigKeys.ConfigPathsConfig.ToString()).Paths;
                return paths.First();
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <typeparam name="T">返回配置项实体</typeparam>
        /// <param name="configName">配置名称</param>
        /// <param name="cached">是</param>
        /// <returns></returns>

        public T GetConfig<T>(string configName, bool cached = true) where T : new()
        {
            return JsonYamlConfigManager.GetCreateConfig<T>(configName);
        }
        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="key"></param>
        public void RemoveConfig(string key)
        {  
            JsonYamlConfigManager.RemoveConfig(key);
        }

        public string MapPath(string path)
        {
            return Path.Combine(BaseDirectory.FullName, path);
        }
    }
}