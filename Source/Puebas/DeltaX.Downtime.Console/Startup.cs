using Autofac; 
using DeltaX.Downtime.DapperRepository;
using DeltaX.Downtime.Domain;
using DeltaX.Downtime.Domain.ProcessAggregate;
using DeltaX.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace DeltaX.Downtime.Console
{
    class Startup  
    {
        IDowntimeRepository repository;
        DowntimeRepositoryMapper mapper;
        ILogger logger;

        public Startup(IDowntimeRepository repository, DowntimeRepositoryMapper mapper, ILogger<Startup> logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;

            logger.LogInformation("Hola mundo info");
            logger.LogError("Hola mundo error");
            logger.LogWarning("Hola mundo warning");
        }

        public void Start()
        {  
            try
            { 
                var id = PruebaInsertAndUpdate();
                PruebaGetUpdate();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Start Error");
                throw e;
            }
        }
        
        private Guid PruebaInsertAndUpdate()
        {
            var product1 = new ProductSpecification("Codigo 1");
            var processHistory = new ProcessHistory(Guid.NewGuid(), DateTime.Now.AddMinutes(-20),  product1);
             
            var inserted = repository.InsertAsync(processHistory).Result;

            var product2 = new ProductSpecification("Codigo 2");
            inserted.SetProductSpecification(product2);
            inserted.FinishProcess(DateTime.Now);

            var interruptionDisabled = inserted.Copy();

            var updated = repository.UpdateAsync(inserted).Result;

            return updated.Id;
        }


        private Guid PruebaGetUpdate()
        {
            var items = repository.GetListAsync().Result;
            var item = items.First();

            var product3 = new ProductSpecification("Codigo 3");
            item.SetProductSpecification(product3);
            item.FinishProcess(DateTime.Now);

            var updated = repository.UpdateAsync(item).Result;

            return updated.Id;
        }

    }
}
