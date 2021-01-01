using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaX.GenericReportDb.Configuration
{

    public class CrudConfiguration
    {
        public CrudConfiguration()
        {
        }

        public DateTime GetModification(string configName)
        {
            string configFile = $"Cfg/{configName}.json";
            if (!File.Exists(configFile))
                return DateTime.MinValue;
            return File.GetLastWriteTime(configFile);
        }

        public EndpointConfiguration Read(string configName)
        {
            string configFile = $"Cfg/{configName}.json";

            var jsonRaw = File.ReadAllText(configFile);
            var endpoint = JsonSerializer.Deserialize<EndpointConfiguration>(jsonRaw);
            endpoint.Name = configName;
            endpoint.DisplayName ??= configName;

            // File.WriteAllText(configFile + "_bkp.json", JsonSerializer.Serialize(endpoint), Encoding.UTF8);  

            return endpoint;
        }

        public string GetConfigName(string filename)
        {
            filename = Path.GetFileName(filename);
            string extension = Path.GetExtension(filename);
            string result = filename.Substring(0, filename.Length - extension.Length);
            return result;
        }

        public IEnumerable<string> GetCrudConfigs()
        {
            var fs = Directory.GetFiles("Cfg").ToArray();
            return fs.Select(f => GetConfigName(f));
        }
    }
}