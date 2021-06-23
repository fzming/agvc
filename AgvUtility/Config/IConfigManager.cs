using System.IO;

namespace Utility.Config
{
    /// <summary>
    /// 统一配置读写接口
    /// </summary>
    public interface IConfigManager : ISingletonDependency
    {
        /// <summary>
        /// 是否是开发模式
        /// </summary>
        bool DevMode { get; set; }
        /// <summary>
        /// 配置根目录
        /// 一般为：\Configurations目录
        /// </summary>
        DirectoryInfo BaseDirectory { get; }
        /// <summary>
        /// 读取配置实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <param name="cached"></param>
        /// <returns></returns>
        T GetConfig<T>(string configName, bool cached = true)
            where T : new();
        /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="key"></param>
        void RemoveConfig(string key);
        /// <summary>
        /// 映射配置文件夹的相对路径文件为物理路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string MapPath(string path);
    }
}