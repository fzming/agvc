using AgvcWorkFactory.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AgvcWorkFactory
{
    public class AgvcConfiguration : IAgvcConfiguration
    {
        private readonly AgvcConfig agvConfig;

        public  AgvcConfiguration(IConfiguration configuration)
        {
            this.agvConfig = configuration.GetSection("AGVC").Get<AgvcConfig>();
        }

        public AgvcConfig GetConfig()
        {
            return agvConfig;
        }
    }
}