using System;

namespace Utility.Config.Files
{
    /// <summary>
    /// A data type containing metadata about a configuration file.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The raw text read from the configuration file.
        /// </summary>
        public string Raw { get; set; }
        /// <summary>
        /// The parsed object
        /// </summary>
        public object Parsed { get; set; }
        /// <summary>
        /// The filepath to the configuration file.
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// The time the configuration file was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// The type of config file.
        /// </summary>
        public ConfigFileType ConfigType { get; set; }
    }
}