using System;

namespace Utility.Config.Files
{
    /// <summary>
    /// A data type containing metadata about a configuration item.
    /// </summary>

    public class ConfigurationItem
    {
        /// <summary>
        /// The name of the configuration item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The raw configuration data.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The time the configuration file was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// A bool indicating whether the configuration item came from
        /// ConfigManager or a user-specified source delegate.
        /// </summary>
        public bool FromConfigManager { get; set; }
    }
}