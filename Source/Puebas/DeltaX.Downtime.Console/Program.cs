using Autofac; 
using Dapper;
using DeltaX.Database;
using DeltaX.Downtime.Application;
using DeltaX.Downtime.DapperRepository;
using DeltaX.Downtime.Domain.ProcessAggregate;
using DeltaX.Repository.DapperRepository; 
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Data; 

namespace DeltaX.Downtime.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            LoggerConfig.Configure();

            var log = Log.ForContext<Program>();
            log.Information("Init...");

            SqliteTypeHandler.SetSqliteTypeHandler();

            int i = 1;
            Class1.GetPropertyInfo<ProductSpecification, bool>(f => f.Code.Like("%PEPE%")); 
            Class1.GetPropertyInfo<ProductSpecification, bool>((f) => f.Id  == i);
            
            Class1.GetPropertyInfo<ProductSpecification, bool>((f) => f.Id  == 1); 
            Class1.GetPropertyInfo<ProductSpecification, bool>((f) => f.Id >= 1 && f.Id<5 ); 
            Class1.GetPropertyInfo<ProductSpecification, bool>((f) => f.StandarDuration == 2 && f.Code == null); 
            
            Class1.GetPropertyInfo<ProductSpecification, bool>((f) => f.StandarDuration == 2 && f.Code == "%PEPE%"); 
            Class1.GetPropertyInfo<ProductSpecification, bool>((f) => f.StandarDuration == 2 || f.StandarDuration > 2);
            Class1.GetPropertyInfo<ProductSpecification, bool>((f) => f.StandarDuration == 2);
            

            try
            {
                log.Information("Configure Autofac...");
                var builder = new ContainerBuilder();
                // Logger
                builder.RegisterInstance(new LoggerFactory().AddSerilog()).As<ILoggerFactory>();
                builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
                builder.Register(c => c.Resolve<ILoggerFactory>().CreateLogger(""))
                    .As<Microsoft.Extensions.Logging.ILogger>().InstancePerLifetimeScope(); 

                // Database
                builder.Register(c => new DbConnectionFactory<SqliteConnection>(new[] { "Filename=Downtime.sqlite3" }))
                    .SingleInstance();
                builder.Register<IDbConnection>(c => c.Resolve<DbConnectionFactory<SqliteConnection>>().GetConnection())
                    .InstancePerLifetimeScope();

                // Downtime Repository Module
                builder.RegisterModule<DowntimeRepositoryModule>();
                
                // Downtime Application Module
                builder.RegisterModule<DowntimeApplicationModule>();

                // Startup
                builder.RegisterType<Startup>().SingleInstance();

                log.Information("Start...");
                var container = builder.Build();
                 

                using (var scope = container.BeginLifetimeScope())
                {   
                    var applicationService = scope.Resolve<DowntimeApplicationService>();
                    applicationService.PruebaInsertAndUpdate();

                    scope.Dispose();
                }

                var startup = container.Resolve<Startup>();
                startup.Start();   
                container.Dispose(); 
            }
            catch (Exception e)
            {
                log.Fatal(e, "Startup Error");
            }
            finally
            {
                log.Information("Finish!");
            }
        }
    }

   
}
