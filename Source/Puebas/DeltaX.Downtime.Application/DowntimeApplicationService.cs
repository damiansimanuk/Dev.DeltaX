using DeltaX.Cache;
using DeltaX.Domain.Common;
using DeltaX.Downtime.Domain;
using DeltaX.Downtime.Domain.ProcessAggregate; 
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Transactions;

namespace DeltaX.Downtime.Application
{
    public class DowntimeApplicationService : IApplicationService
    {
        IDowntimeRepository repository;
        ILogger logger;
        TransactionItems<string> items;

        public DowntimeApplicationService(
            IDowntimeRepository repository,
            ILogger<DowntimeApplicationService> logger, 
            TransactionItems<string> items)
        {
            this.repository = repository;
            this.logger = logger;
            this.items = items;

            logger.LogInformation("Hola mundo info");
            logger.LogError("Hola mundo error");
            logger.LogWarning("Hola mundo warning");
        }

        public Guid? PruebaInsertAndUpdate()
        {
            ProcessHistory updated = null;
            using (var scope = new TransactionScope())
            {
                logger.LogInformation("PruebaInsertAndUpdate");
                items.Add(a => false, "Pepe");
                var r = items.GetAll();

                var product1 = new ProductSpecification("Codigo 1");
                var processHistory = new ProcessHistory(Guid.NewGuid(), DateTime.Now.AddMinutes(-20), product1);

                var inserted = repository.InsertAsync(processHistory).Result;

                var product2 = new ProductSpecification("Codigo 2");
                inserted.SetProductSpecification(product2);
                inserted.FinishProcess(DateTime.Now);

                updated = repository.UpdateAsync(inserted).Result;
                
                scope.Complete();
            }

            var r2 = items.GetAll(); 
            return updated?.Id;
        }


        public Guid PruebaGetUpdate()
        {
            logger.LogInformation("PruebaGetUpdate");

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
