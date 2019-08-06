using Newtonsoft.Json;
using ServerlessIoT.UWP.Config;
using ServerlessIoT.UWP.Data;
using ServerlessIoT.UWP.Providers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessIoT.UWP.Providers
{
    public class SignalRAccessTokenProvider : ISignalRAccessTokenProvider
    {
        private HttpClient _httpClient;

        public SignalRAccessTokenProvider()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(AppConfig.DeviceDataBroadcastFunctionUrl)
            };
        }

        public async Task<SignalRConnectionInfo> AcquireSignalRConnectionInfoAsync()
        {
            var responseMessage = await _httpClient.GetAsync("negotiate");
            var responseContent = await responseMessage
            .Content
            .ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                var signalRConnectionInfo = JsonConvert.DeserializeObject<SignalRConnectionInfo>(responseContent);

                return signalRConnectionInfo;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No success for acquiring SignalR service token in method: {nameof(AcquireSignalRConnectionInfoAsync)}");
                System.Diagnostics.Debug.WriteLine($"Http status code: {responseMessage.StatusCode} message: {responseContent}");
                return null;
            }
        }
    }
}
