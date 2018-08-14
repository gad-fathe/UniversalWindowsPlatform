using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempSensor.Services
{
    public class AzureIoTHubService
    {
        private DeviceClient _deviceClient;

        public AzureIoTHubService()
        {
            _deviceClient = DeviceClient.CreateFromConnectionString("<<CONNECTION STRING HERE>>", TransportType.Http1);
            _deviceClient.SetConnectionStatusChangesHandler((status, reason) =>
            {
                Debug.WriteLine("CONNECTION STATUS TO AZURE IoT HUB: " + status);
            });
        }

        public async Task<bool> SendDataToAzure(string data)
        {
            var messageString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            await _deviceClient.SendEventAsync(message);

            Debug.WriteLine("{0} > Sending telemetry: {1}", DateTime.Now, messageString);
            return true;
        }
    }
}
