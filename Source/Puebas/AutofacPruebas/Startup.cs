using System;
using Microsoft.Extensions.DependencyInjection;

namespace AutofacPruebas
{

    class Depend2
    {
        public Depend2(SimpleLongPollingLinq<string> items)
        {
            items.Push("Depend2 added");
        }
    }

    class Depend3
    {
        public Depend3(Depend2 depend2, SimpleLongPollingLinq<string> items)
        {
            items.Push("Depend3 added... depend2:" + depend2.GetHashCode());
        }
    }


    class Startup 
    {
        private IOutput output;
        private SimpleLongPollingLinq<string> items;

        public Startup(IServiceProvider scope,Depend3 depend3, IOutput output, SimpleLongPollingLinq<string> items)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.items = items;
            Console.WriteLine("Startup activated");


            items.Push("items.Push");
            using (var s = scope.CreateScope())
            {
                var items2 = s.ServiceProvider.GetService<SimpleLongPollingLinq<string>>();
                items2.Push("items2.Push");
            }
        }

        public void Start()
        {

            items.Push("Startup.Start");

            Console.WriteLine("Startup started");

            output.Write("Startup started injected: \n - " + string.Join("\n - ", items.TakeAll()));

        }
    }
}

