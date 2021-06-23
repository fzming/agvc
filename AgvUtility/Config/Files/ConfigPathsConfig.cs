using System;
using System.IO;
using System.Linq;

namespace Utility.Config.Files
{

    /// <summary>
    /// The strongly typed class to deserialize the
    /// global configuration file ConfigPaths.conf
    /// </summary>
    public class ConfigPathsConfig
    {
        /// <summary>
        /// An ordered list of search paths to search
        /// in for configuration files.
        /// </summary>
        public DirectoryInfo[] Paths { get; set; }

        public static DirectoryInfo Configurations { get; set; }
        /// <summary>
        /// Contructs the configuration with the default
        /// values for search paths, which are: the local
        /// application root directory, C:/Configs, and
        /// D:/Configs
        /// </summary>
        public ConfigPathsConfig()
        {
            if (Configurations!=null)
            {
                Paths= new[] { Configurations };
                return;
            }
            //反向查找Configurations目录
            var baseDirectory = new DirectoryInfo(
                AppDomain.CurrentDomain.BaseDirectory);
             
            
            while (Configurations==null)
            {
                Configurations = baseDirectory.GetDirectories("Configurations").FirstOrDefault();
                baseDirectory = baseDirectory.Parent;
                if (baseDirectory==null)
                {
                  break;
                }
        
            }

            Paths = new[] { Configurations };


        }
    }


}