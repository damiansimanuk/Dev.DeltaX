namespace DeltaX.Connections
{
    using DeltaX.BaseConfiguration;
    using Microsoft.Extensions.Configuration;


    public class MqttConfiguration : Configuration
    { 
        public MqttConfiguration( string section, string configFileName = null)
            :base(section, configFileName) 
        {
            Initialize();
        }

        public MqttConfiguration(IConfiguration configuration)
            : base(configuration)
        {
            Initialize();
        } 

        private void Initialize()
        {
            if (Config == null)
            {
                return;
            }

            ClientId = Config.GetValue("ClientId", ClientId);
            Host = Config.GetValue("Host", Host);
            Port = Config.GetValue("Port", Port);
            Secure = Config.GetValue("Secure", Secure);
            Username = Config.GetValue("Username", Username);
            Password = Config.GetValue("Password", Password);
            ReconnectDealy = Config.GetValue("ReconnectDealy", ReconnectDealy);
        }

        public string ClientId { get; set; } = null;

        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 1883;

        public bool Secure { get; set; } = false;

        public string Username { get; set; } = null;

        public string Password { get; set; } = null;

        public int ReconnectDealy { get; set; } = 1000;

    }


}
