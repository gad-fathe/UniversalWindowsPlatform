using Microsoft.AspNetCore.SignalR.Client;
using ServerlessIoT.UWP.Providers.Interfaces;
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
        private ISignalRAccessTokenProvider _signalRAccessTokenProvider;

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

        public ClientSignalR(ISignalRAccessTokenProvider signalRAccessTokenProvider)
        {
            _signalRAccessTokenProvider = signalRAccessTokenProvider;
        }

        //TODO: Decide how to handle token refresh and url config:
        public async Task Initialize(string connectionUrl)
        {
            _connectionUrl = connectionUrl;

            _hub = new HubConnectionBuilder()
                .WithUrl(_connectionUrl, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        var accessToken = await _signalRAccessTokenProvider.AcquireSignalRConnectionInfoAsync();
                        return accessToken?.AccessToken;
                    };
                })
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
