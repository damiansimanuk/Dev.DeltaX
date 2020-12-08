namespace DeltaX.Connections
{
    using DeltaX.BaseConfiguration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using uPLibrary.Networking.M2Mqtt;

    public class MqttClientHelper
    {
        ILogger logger;
        Task reconnectTask;
         

        AutoResetEvent isDisconnectedEvent;

        public event EventHandler<bool> OnConnectionChange;

        public MqttClient Client { get; private set; }

        public bool IsConnected
        {
            get
            {
                return isDisconnectedEvent?.WaitOne(0) == false
                    && Client?.IsConnected == true;
            }
        }

        public MqttConfiguration Config { get; set; }
        public bool IsRunning { get; set; }

        public MqttClientHelper(IConfiguration configuration, ILogger logger = null)
            : this(new MqttConfiguration(configuration), null, logger)
        {
        }

        public MqttClientHelper(MqttConfiguration config, MqttClient client = null, ILogger logger = null)
        {
            this.Config = config;
            this.logger = logger ?? LoggerConfig.DefaultLogger;
            this.IsRunning = true;
            isDisconnectedEvent = new AutoResetEvent(true);
            this.Client = client ?? new MqttClient(Config.Host, Config.Port, Config.Secure, null, null, MqttSslProtocols.None);
        }


        private async Task DoConnect(CancellationToken cancellationToken, TaskCompletionSource<bool> taskCompletionConnected = null)
        {
            Type prevException = null;

            while (!cancellationToken.IsCancellationRequested && IsRunning)
            {
                try
                {
                    logger.LogDebug($"Connecting with ClientId:{Config.ClientId}");
                    if (string.IsNullOrEmpty(Config.Username))
                    {
                        Client.Connect(Config.ClientId);
                    }
                    else
                    {
                        Client.Connect(Config.ClientId, Config.Username, Config.Password);
                    }

                    logger.LogInformation($"Connected ClientId:{Config.ClientId}");
                    isDisconnectedEvent.Reset();
                    OnConnectionChange?.Invoke(this, IsConnected);
                     
                    taskCompletionConnected?.SetResult(true);
                    taskCompletionConnected = null;
                    prevException = null;

                    // block while is connected
                    isDisconnectedEvent.WaitOne();                    
                }
                catch (Exception e)
                {
                    if (cancellationToken.IsCancellationRequested || !IsRunning)
                    {
                        break;
                    }

                    if (e.GetType() != prevException)
                    {
                        prevException = e.GetType();
                        logger.LogError(e, "Connect Error");
                        logger.LogWarning($"Mqtt ClientId:{Config.ClientId} Connect Failed, retry on {Config.ReconnectDealy / 1000} seconds");
                    }

                    await Task.Delay(Config.ReconnectDealy, cancellationToken);
                }
            }

            isDisconnectedEvent.Set();
            taskCompletionConnected?.SetCanceled();
            logger.LogInformation($"Mqtt ClientId:{Config.ClientId} Connection Closed!");
            OnConnectionChange?.Invoke(this, IsConnected);
        }

        private void OnConnectionClosed(object sender, EventArgs e)
        {
            isDisconnectedEvent.Set();
            logger.LogInformation($"Mqtt ClientId:{Config.ClientId} Connect Closed!");
            OnConnectionChange?.Invoke(this, IsConnected);
        }

        public Task<bool> ConnectAsync(CancellationToken? cancellationToken = null)
        {
            if (reconnectTask != null && IsRunning)
            {
                return Task.FromResult(IsConnected);
            }

            TaskCompletionSource<bool> firstConnected = new TaskCompletionSource<bool>();
            RunAsync(cancellationToken, firstConnected);
            return firstConnected.Task;
        }

        public Task RunAsync(CancellationToken? cancellationToken = null, TaskCompletionSource<bool> firstConnected = null)
        {
            if (reconnectTask != null && IsRunning)
            {
                return reconnectTask;
            }

            Client.ConnectionClosed -= OnConnectionClosed;
            Client.ConnectionClosed += OnConnectionClosed;
            reconnectTask = Task.Run(() => DoConnect(cancellationToken ?? CancellationToken.None, firstConnected));
            return reconnectTask;
        }

        public void Disconnect()
        {
            Client.ConnectionClosed -= OnConnectionClosed;
            IsRunning = false;
            Client?.Disconnect();
            isDisconnectedEvent.Set();
            // reconnectTask?.Dispose();
            reconnectTask?.Wait(5000);
            reconnectTask = null;
        }
    }


}
