namespace DeltaX.Downtime.DapperRepository
{
    using Autofac;  
    using DeltaX.Domain.Common.Repositories;

    public class DowntimeRepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        { 
            builder.RegisterType<DowntimeTableQueryFactory>()
                .SingleInstance();
            builder.RegisterType<DowntimeRepositoryMapper>()
                .SingleInstance();
            builder.RegisterType<DowntimeTableCreator>().As<IStartable>()
                .SingleInstance();
            builder.RegisterType<DowntimeUnitOfWork>() 
                .InstancePerLifetimeScope();
            builder.RegisterType<DowntimeRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
