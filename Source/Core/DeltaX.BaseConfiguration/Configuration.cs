namespace DeltaX.BaseConfiguration
{
    using Microsoft.Extensions.Configuration;  
    using System.Collections.Generic;
    using Microsoft.Extensions.Primitives;

    public class Configuration : IConfiguration
    {
        public IConfiguration Config { get; set; }


        public Configuration(IConfiguration configuration)
        {
            Config = configuration;
        }

        public Configuration(string section, string configFileName = null, bool optional = true)
        {
            var builder = GetConfigurationBuilder(configFileName, optional);

            if (string.IsNullOrEmpty(section))
            {
                Config = builder?.Build();
            }
            {
                Config = builder?.Build().GetSection(section);
            } 
        }

        public static IConfigurationBuilder GetConfigurationBuilder(string configFile = null, bool optional = false)
        {
            string configFileName;
            if (string.IsNullOrEmpty(configFile))
            {
                configFileName = CommonSettings.GetPathConfigFileByProcessName();
            }
            else
            {
                configFileName = CommonSettings.GetPathConfigFile(configFile);
            }

            if (string.IsNullOrEmpty(configFileName))
                return null;

            return new ConfigurationBuilder()
               .AddJsonFile(configFileName, optional: optional);
        }

        public IConfigurationSection GetSection(string key)
        {
            return Config.GetSection(key);
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return Config.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return Config.GetReloadToken();
        }

        public string this[string key] { get => Config[key]; set => Config[key] = value; }
    }
}
