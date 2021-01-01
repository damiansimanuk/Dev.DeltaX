using DeltaX.GenericReportDb.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DeltaX.GenericReportDb.Services
{
    public class CrudServiceBuilder
    {
        private readonly IConfiguration configuration;
        private readonly CrudConfiguration crudConfiguration;
        private readonly IUserService userService;
        private readonly ILogger<CrudService> logger;
        private ConcurrentDictionary<string, CrudService> crudServices;

        public CrudServiceBuilder(
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
                GetCrudService(configName);
            }
        }

        public IEnumerable<CrudService> GetAllServices()
        {
            return crudServices.Values.ToArray();
        }


        public CrudService GetCrudService(string configName, bool forceRefresh = false)
        {
            CrudService crud;
            if (crudServices.TryGetValue(configName, out crud))
            {
                var modified = crudConfiguration.GetModification(configName);
                logger?.LogDebug("dateModification < crud.DateTimeInstanced: {0} < {1}",
                   modified, crud.ReloadAt);

                if (modified < crud.ReloadAt && forceRefresh == false)
                {
                    return crud;
                }
            }

            var endpointConfig = crudConfiguration.Read(configName);
            if (crud == null)
            {
                crud = new CrudService(endpointConfig, logger);
            }
            else
            {
                crud.SetConfiguration(endpointConfig);
            }

            // Create Roles 
            foreach (var roleName in crud.Configuration.PermissionsRoles)
            {
                userService.CreateRoleIfNotExist(roleName);
            }

            crudServices[configName] = crud;
            return crud;
        }
    }
}
