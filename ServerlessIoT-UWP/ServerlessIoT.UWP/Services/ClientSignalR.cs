using Microsoft.AspNetCore.SignalR.Client;
using ServerlessIoT.UWP.Config;
using ServerlessIoT.UWP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessIoT.UWP.Services
{
    public class ClientSignalR : IClientSignalR
    {
        private HubConnection _hub;
        public HubConnection Hub
        {
            get
            {
                return _hub;
            }
        }

        public ClientSignalR()
        {
        }

        public async Task Initialize()
        {
            _hub = new HubConnectionBuilder()
                .WithUrl(AppConfig.DeviceDataBroadcastFunctionUrl)
                .Build();

            await _hub.StartAsync();
        }


        public void SubscribeHubMethod(string methodName)
        {
            _hub.On<string>(methodName, (data) =>
            {
                OnMessageReceived?.Invoke(data);
            });
        }

        public async Task SendHubMessage(string methodName, string data)
        {
            await _hub?.InvokeAsync(methodName, data);
        }

        public async Task CloseConnection()
        {
            await _hub.DisposeAsync();
        }

        public event Action<string> OnMessageReceived;
    }
}
