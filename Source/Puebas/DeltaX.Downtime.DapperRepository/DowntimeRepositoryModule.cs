namespace DeltaX.Downtime.DapperRepository
{
    using Autofac;  
    using DeltaX.Domain.Common.Repositories;
    using DeltaX.Downtime.Domain;

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
            builder.RegisterType<DowntimeUnitOfWork>().As<IUnitOfWork>()
                .InstancePerLifetimeScope()
                .OnActivated(a =>
                {
                    a.Instance.BeginTransaction();
                })
                .OnRelease(a =>
                {
                    try
                    {
                        a.SaveChanges();
                    }
                    finally
                    {
                        a.Dispose();
                    }
                });
            builder.RegisterType<DowntimeRepository>().As<IDowntimeRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
