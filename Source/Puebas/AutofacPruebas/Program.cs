using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AutofacPruebas
{



    class Program
    {
        static void Main(string[] args)
        {
            /// /// ONLY AUTOFAC
            /// var builder = new ContainerBuilder();
            /// builder.RegisterType<OutputService>().As<IOutput>();
            /// builder.RegisterType<Depend2>().As<Depend2>();
            /// builder.RegisterType<Depend3>().As<Depend3>();
            /// builder.RegisterGeneric(typeof(SimpleLongPollingLinq<>))
            ///     .As(typeof(SimpleLongPollingLinq<>))
            ///     // .SingleInstance()
            ///     .InstancePerLifetimeScope();
            /// builder.RegisterType<Startup>().As<IStartable>().SingleInstance();





            //setup our DI
            var serviceCollection = new ServiceCollection()

                .AddSingleton<IOutput, OutputService>()
                .AddScoped<Depend2>()
                .AddScoped<Depend3>()
                .AddScoped<Startup>()
                .AddScoped(typeof(SimpleLongPollingLinq<>));

            // builder.Populate(serviceCollection);
            // serviceCollection.AddAutofac();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var autofacScope = serviceProvider.GetService<ILifetimeScope>();
            var lista = serviceProvider.GetService<SimpleLongPollingLinq<string>>();
            lista.Push("From MAIN");
            var listaInt = serviceProvider.GetService<SimpleLongPollingLinq<int>>();
            listaInt.Push(234);

            var output = serviceProvider.GetService<IOutput>();
            output.Write("All done!");

            var startup = serviceProvider.GetService<Startup>();
            startup.Start();

            // builder.Build();
        }
    }
}
