using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureSignalRTransportApp.SignalR
{
    public class ClientSignalR
    {
        private HubConnection _hub;
        public HubConnection Hub
        {
            get
            {
                return _hub;
            }
        }

        private string _connectionUrl;
        public string ConnectionUrl
        {
            get
            {
                return _connectionUrl;
            }
        }

        public ClientSignalR()
        {
        }

        public async Task Initialize(string connectionUrl)
        {
            _connectionUrl = connectionUrl;

            _hub = new HubConnectionBuilder()
                .WithUrl(_connectionUrl)
                .Build();

            await _hub.StartAsync();
        }


        public void SubscribeHubMethod(string methodName)
        {
            _hub.On<string>(methodName, (message) =>
            {
                OnMessageReceived?.Invoke(message);
            });
        }

        public async Task SendHubMessage(string methodName, string[] parameters)
        {
            await _hub?.InvokeAsync(methodName, parameters);
        }

        public async Task CloseConnection()
        {
            await _hub.DisposeAsync();
        }

        public event Action<string> OnMessageReceived;
    }
}
