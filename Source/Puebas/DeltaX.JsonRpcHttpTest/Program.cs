using DeltaX.Connections.HttpServer;
using DeltaX.JsonRpc;
using DeltaX.JsonRpc.HttpConnection;
using DeltaX.JsonRpc.Interfaces;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DeltaX.JsonRpcHttpTest
{

    public class CustObj
    {
        public int a { get; set; }
        public string[] b { get; set; }
        public object[] c { get; set; }
    }


    public interface IExampleService
    {  
        public int Sumar(int a, int b); 
        public string Concatenar(string format, int a, int b);
        public string FuncDemo(int a, string b = "pepe", CustObj obj = default);
        public CustObj FuncCustObj(int a, string b = "pepe", CustObj obj = default);
        Task<float> TaskIntAsync(int a, int b);
        Task TaskVoidAsync(int a, int b);
        void TaskVoid(int a, int b);
    }

    class Program
    {
        static void Main(string[] args)
        {

            var identity = new System.Security.Principal.GenericIdentity("Sima");
            Thread.CurrentPrincipal = new System.Security.Principal.GenericPrincipal(identity, new[] { "administrator"});

            Console.WriteLine("Hello World!");

            var t = TestRpcServer();

            var t2 = Task.Run(TestRpcClient);

            Task.WaitAll(t, t2);
        }


        static void TestApp()
        {

            // Listener listener = new Listener(8081, "172.17.36.142");
            Listener listener = new Listener(8080);
            App app = new App(listener);

            DateTime dateStart = DateTime.Now;
            app.Routes.Get("/", async (match, req, res) =>
            {
                await res.SendAsync("<p>Live server since " + (DateTime.Now - dateStart).ToString(@"dd\.hh\:mm\:ss") + " time.</p>", "text/html");
            });

            app.Routes.Get("/json", async (match, req, res) =>
            {
                await res.SendAsync("Ok");
            });


            app.Routes.Post("/crear", async (match, req, res) =>
            {
                var json = await req.GetBodyAsync();
                var obj = JsonSerializer.Deserialize<CustObj>(json);

                await res.CloseAsync(HttpStatusCode.Created);
            });


            Regex rxFile = new Regex(@"/Downloads/(?<filepath>([^\s/]+/)*[^\s/]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            app.Routes.Get(rxFile, async (match, req, res) =>
            {
                string filepath = match.Groups["filepath"].Value;
                string p = Path.Combine(Environment.CurrentDirectory, filepath);
                await res.SendFileAsync(p);
            });

            app.RunAsync();
        }




        public class ExampleService : IExampleService
        {
            private readonly Rpc rpc;

            public ExampleService(Rpc rpc)
            {
                this.rpc = rpc;
            }

            public string Concatenar(string format, int a, int b)
            {
                return $"Formated format:{format} a:{a} b:{b}";
            }

            public int Sumar(int a, int b)
            {
                rpc.NotifyAsync("NotifySumar", a, b);
                return a + b;
            }

            public void TaskVoid(int a, int b)
            {
                Console.WriteLine("TaskVoid {0} {1}", a, b); 
            } 
            
            public Task TaskVoidAsync(int a, int b)
            {
                Console.WriteLine("TaskVoidAsync {0} {1}", a, b);
                return Task.CompletedTask;
            }

            public async Task<float> TaskIntAsync(int a, int b)
            {
                Console.WriteLine("SumarAsync {0} {1}", a, b);
                await Task.Delay(500);
                Console.WriteLine("SumarAsync {0} {1} ... ", a, b);
                return (float)(a + b + 1.2323);
            }

            public string FuncDemo(int a, string b = "pepe", CustObj obj = default)
            {
                return $"FuncDemo: a:{a} - b:{b} {JsonSerializer.Serialize(obj)}";
            }

            public string FuncDemo2(int a, string b = "pepe", CustObj obj = default)
            {
                return $"FuncDemo 2 ... A:{a} - B:{b} Obj:{JsonSerializer.Serialize(obj)}";
            }

            public CustObj FuncCustObj(int a, string b = "pepe", CustObj obj = default)
            {
                return new CustObj()
                {
                    a = obj?.a ?? a,
                    b = new string[] { b, "obj_a: " + obj?.a },
                    c = new object[] { a, b }
                };
            }
        }


        static Task TestRpcServer()
        {
            var listener = new Listener(8081); 
            var rpc = new Rpc(new JsonRcpHttpConnection(listener));

            var functions = new ExampleService(rpc);
            rpc.Dispatcher.RegisterService(functions);
            
            rpc.UpdateRegisteredMethods();

            return listener.StartAsync();
        }


        public static void NotifySumar(IMessage message)
        {
            Console.WriteLine("RECIBIO NotifySumar ***** message:{message}", message.Serialize());
        }
         

        static void TestRpcClient()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(100);
            client.BaseAddress = new Uri("http://localhost:8081/");
             
            var rpcClient = new Rpc(new JsonRcpHttpConnectionClient(client));

            var service = rpcClient.GetServices<IExampleService>();
             
             rpcClient.Dispatcher.RegisterFunction("NotifySumar", (message) =>
             {
                 Console.WriteLine("************************ RECIBIO NotifySumar ***** message:{0} params:{1}",
                     message.Serialize(), message.GetParameters<JsonElement>());
             
                 var param = message.GetParameters<JsonElement[]>();
                 var arg1 = param[0].GetInt32();
                 var arg2 = param[1].GetInt32();
             });
              
             rpcClient.UpdateRegisteredMethods();
             
            try
            {
                Console.WriteLine("************************");
                service.TaskVoid(12, 2);
                var res31 = service.TaskVoidAsync(100, 200);
                var res32 = service.TaskIntAsync(100, 200);
                var res33 = service.TaskIntAsync(100, 200);
                var res34 = service.TaskIntAsync(100, 200);
                Console.WriteLine("************************"); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
             
            while (true)
            {
                try
                {
                    var res = rpcClient.Call<int>("Sumar", 12, 123);
                    Console.WriteLine("Sumar return:" + res);

                    var res2 = service.Sumar(10, 123);
                    Console.WriteLine("service Sumar return:" + res2);

                    string rCall1 = service.Concatenar("ASDF", 1, 3);
                    Console.WriteLine("GetCaller Concatenar Result: {0}", rCall1);

                    int resSuma1 = service.Sumar(1, 3);
                    Console.WriteLine("GetCaller Sumar Result: {0}", resSuma1);

                    CustObj resObj1 = service.FuncCustObj(1234, "hola FuncCustObj");
                    Console.WriteLine("GetCaller FuncCustObj Result: {0}", JsonSerializer.Serialize(resObj1));

                    resObj1 = service.FuncCustObj(1234, "hola FuncCustObj", new CustObj { a = 23, b = new string[] { "sadf" } });
                    Console.WriteLine("GetCaller FuncCustObj Result: {0}", JsonSerializer.Serialize(resObj1));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception  :" + e);
                }

                Thread.Sleep(20000);
            }
        } 
    }
}

