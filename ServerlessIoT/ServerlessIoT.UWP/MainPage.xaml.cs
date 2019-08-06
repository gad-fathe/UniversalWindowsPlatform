using ServerlessIoT.UWP.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ServerlessIoT.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ClientSignalR _hubClient;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task InitializeSignalRClient()
        {
            try
            {
                _hubClient = new ClientSignalR();
                await _hubClient.Initialize("https://serverless-iot.service.signalr.net/client/?hub=devicedata");
                _hubClient.SubscribeHubMethod("newMessage");
                _hubClient.OnMessageReceived += (data) =>
                {
                    System.Diagnostics.Debug.WriteLine("SignalR Connection got message: " + data);
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SignalR Connection error: " + ex.Message);
            }
        }
    }
}
