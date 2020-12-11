using DeltaX.BaseConfiguration;
using DeltaX.Connections;
using DeltaX.JsonRpc;
using DeltaX.JsonRpc.Interfaces;
using DeltaX.JsonRpc.MqttConnection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DeltaX.JsonRpcMqtt
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
        Task SendBroadcastAsync(object message);
    }


    public class ExampleService : IExampleService
    {
        private readonly Rpc rpc;
        private readonly ILogger logger;

        public ExampleService(Rpc rpc)
        {
            this.rpc = rpc;
            this.logger = LoggerConfig.DefaultLogger;
        }

        public async Task SendBroadcastAsync(object message)
        {
            logger.LogInformation("*** SendBroadcastAsync Receive: {@0}", message);
            await Task.Delay(2000); 
            await rpc.NotifyAsync("NotificationBroadcast", message);

            logger.LogInformation("*** SendBroadcastAsync END");
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


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            LoggerConfig.SetSerilog();

            var server = TestRpcServer();
            var client = TestRpcClient();
            Task.WaitAll(server, client);


            var config = new MqttConfiguration("Mqtt");
            config.ClientId = "Test JsonRpcMqtt" + Guid.NewGuid();
            config.Username = "sima";
            config.Password = "sima";

            var mqtt = new MqttClientHelper(config);
            var taskRun = mqtt.RunAsync();
            TestMqtt(mqtt);

            taskRun.Wait();
            mqtt.Disconnect();
        }

        static Task TestRpcServer()
        {
            var config = new MqttConfiguration("Mqtt");
            config.ClientId = "Test JsonRpcMqtt" + Guid.NewGuid();
            config.Username = "sima";
            config.Password = "sima";

            var mqtt = new MqttClientHelper(config);


            var rpc = new Rpc(new JsonRpcMqttConnection(mqtt.Client, "test/" , clientId: "TestRpcServer") );

            var functions = new ExampleService(rpc);
            rpc.Dispatcher.RegisterService(functions);

            mqtt.OnConnectionChange += (s, connected) => { if (connected) { rpc.UpdateRegisteredMethods(); } };
            
            return mqtt.RunAsync();
        }

        static async Task TestRpcClient()
        {
            var logger = LoggerConfig.DefaultLogger;

            var config = new MqttConfiguration("Mqtt");
            config.ClientId = "Test JsonRpcMqtt" + Guid.NewGuid();
            config.Username = "sima";
            config.Password = "sima";

            var mqtt = new MqttClientHelper(config);
            mqtt.ConnectAsync().Wait();

            var rpc = new Rpc(new JsonRpcMqttConnection(mqtt.Client, "test/", clientId: "TestRpcClient"));
            mqtt.OnConnectionChange += (s, connected) => { if (connected) { rpc.UpdateRegisteredMethods(); } };

            var service = rpc.GetServices<IExampleService>();

            while (mqtt.IsRunning)
            { 
                var taskBroadcast = rpc.CallAsync("SendBroadcastAsync", "hola mundo " + DateTime.Now);
                var taskBroadcast2 = service.SendBroadcastAsync("hola mundo " + DateTime.Now);
                logger.LogInformation("SendBroadcastAsync taskBroadcast:{0} {1}", taskBroadcast.Status, taskBroadcast2.Status);

                var res = service.Sumar(1, DateTime.Now.Second);
                logger.LogInformation("Sumar result:{0}", res);

                var t = service.TaskVoidAsync(1, DateTime.Now.Second);
                logger.LogInformation("TaskVoidAsync t.Status:{0}", t?.Status);
                t.Wait();
                logger.LogInformation("TaskVoidAsync t.Wait() t.Status:{0}", t?.Status);

                var t2 = service.TaskIntAsync(1, DateTime.Now.Second);
                logger.LogInformation("TaskIntAsync t2.Result:{0}", t2.Result);

                string rCall1 = service.Concatenar("ASDF", 1, 3);
                logger.LogInformation("GetCaller Concatenar Result: {0}", rCall1);

                CustObj resObj1 = service.FuncCustObj(1234, "hola FuncCustObj");
                logger.LogInformation("GetCaller FuncCustObj Result: {@0}", resObj1);

                resObj1 = service.FuncCustObj(1234, "hola FuncCustObj", new CustObj { a = 23, b = new string[] { "sadf" } });
                logger.LogInformation("GetCaller FuncCustObj Result: {@0} {1}", resObj1, resObj1.c[0]);

                await Task.Delay(5000);
            }
        }


        static void TestMqtt(MqttClientHelper mqtt)
        {
            mqtt.Client.MqttMsgPublishReceived += TestMqttReceive; 
            mqtt.OnConnectionChange += (s, connected) =>
            {
                if (connected)
                {
                    mqtt.Client.Subscribe(new[] { "prueba/+" }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                }
            };

            new Thread(() =>
            {
                while (mqtt.IsRunning)
                {
                    if (mqtt.IsConnected)
                    {
                        var msg = Encoding.UTF8.GetBytes("Hola mundo " + DateTime.Now);
                        var r = mqtt.Client.Publish("prueba/Nada", msg);

                        LoggerConfig.DefaultLogger.LogInformation($"Publish r:{r}");
                    }
                    Task.Delay(2000).Wait();
                }
            }).Start();


        }

        private static void TestMqttReceive(object sender, MqttMsgPublishEventArgs e)
        {
            LoggerConfig.DefaultLogger.LogInformation($"Receiv:{e.Topic} - Message:{Encoding.UTF8.GetString(e.Message)}");
        }
    }
}
