using DeltaX.GenericReportDb.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DeltaX.GenericReportDb.Services
{
    public class CrudServicePool
    {
        private readonly IConfiguration configuration;
        private readonly CrudConfiguration crudConfiguration;
        private readonly IUserService userService;
        private readonly ILogger<CrudService> logger;
        private ConcurrentDictionary<string, CrudService> crudServices;

        public CrudServicePool(
            IConfiguration configuration,
            CrudConfiguration crudConfiguration,
            IUserService userService,
            ILogger<CrudService> logger = null)
        {
            this.configuration = configuration;
            this.crudConfiguration = crudConfiguration;
            this.userService = userService;
            this.logger = logger;
            this.crudServices = new ConcurrentDictionary<string, CrudService>();
            
            this.InitializeCrudServices();
        }
          
        public void InitializeCrudServices()
        {
            var configNames= crudConfiguration.GetCrudConfigs();
            
            logger?.LogInformation("InitializeCrudServices Count:{0}", configNames.Count());
            foreach(var configName in configNames)
            {
                GetService(configName);
            }
        }

        public IEnumerable<CrudService> GetAllServices()
        {
            return crudServices.Values.ToArray();
        }


        public CrudService GetService(string configName, bool forceRefresh = false)
        {
            CrudService service;
            if (crudServices.TryGetValue(configName, out service))
            {
                var modified = crudConfiguration.GetModification(configName);
                logger?.LogDebug("dateModification < crud.DateTimeInstanced: {0} < {1}",
                   modified, service.ReloadAt);

                if (modified < service.ReloadAt && forceRefresh == false)
                {
                    return service;
                }
            }

            var endpointConfig = crudConfiguration.Read(configName);
            if (service == null)
            {
                service = new CrudService(endpointConfig, logger);
            }
            else
            {
                service.SetConfiguration(endpointConfig);
            }

            // Create Roles 
            foreach (var roleName in service.Configuration.PermissionsRoles)
            {
                userService.CreateRoleIfNotExist(roleName);
            }

            crudServices[configName] = service;
            return service;
        }
    }
}
