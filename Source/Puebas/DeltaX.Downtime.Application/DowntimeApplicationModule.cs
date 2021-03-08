using Autofac; 
using Autofac.Core.Resolving.Pipeline;
using DeltaX.Cache;
using DeltaX.Domain.Common;
using DeltaX.Domain.Common.Repositories; 
using System; 

namespace DeltaX.Downtime.Application
{
    public class DowntimeApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        { 
            builder.RegisterGeneric(typeof(CacheFilteredCollection<>))
                .As(typeof(CacheFilteredCollection<>))
                .SingleInstance();       
            builder.RegisterGeneric(typeof(TransactionItems<>))
                .As(typeof(TransactionItems<>))
                .SingleInstance(); 
            builder.RegisterType<DowntimeApplicationService>()
                .InstancePerDependency(); 

            // builder.RegisterServiceMiddleware<DowntimeApplicationService>(PipelinePhase.ServicePipelineEnd, (context, next) =>
            // {
            //     var uow = context.Resolve<IUnitOfWork>();
            //     Console.WriteLine("Requesting Service: {0}", context.Service);
            // 
            //     next(context);
            // 
            //     Console.WriteLine("Requesting Service: {0}", context.Service);
            // }); 
        }
    }
}
