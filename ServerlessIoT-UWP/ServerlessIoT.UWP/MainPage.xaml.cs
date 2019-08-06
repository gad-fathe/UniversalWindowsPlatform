using ServerlessIoT.UWP.Providers;
using ServerlessIoT.UWP.Providers.Interfaces;
using ServerlessIoT.UWP.Services;
using ServerlessIoT.UWP.Services.Interfaces;
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
        private ISignalRAccessTokenProvider _signalRAccessTokenProvider;
        private IClientSignalR _hubClient;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await InitializeSignalRClient();
        }

        private async Task InitializeSignalRClient()
        {
            try
            {
                _signalRAccessTokenProvider = new SignalRAccessTokenProvider();
                _hubClient = new ClientSignalR(_signalRAccessTokenProvider);

                await _hubClient.Initialize();

                _hubClient.SubscribeHubMethod("newMessage");
                _hubClient.OnMessageReceived += async (data) =>
                {
                    System.Diagnostics.Debug.WriteLine("SignalR Connection got message: " + data);

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                    {
                        DataTextBlock.Text = data;
                    });
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SignalR Connection error: " + ex.Message);
            }
        }
    }
}
