using AzureSignalRTransportApp.UWP.Communication;
using AzureSignalRTransportApp.UWP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureSignalRTransportApp.UWP
{
    public sealed partial class MainPage : Page
    {
        private ClientSignalR _hubClient;
        private MapManager _mapHelper;

        public MainPage()
        {
            this.InitializeComponent();
            _mapHelper = new MapManager(TransportMap);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _mapHelper.InitializeMap();
            await InitializeSignalRClient();
        }

        /// <summary>
        /// Initialize connection with TransportHub and subscribe to receive location updates.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeSignalRClient()
        {
            try
            {
                _hubClient = new ClientSignalR();
                await _hubClient.Initialize("http://localhost:63369/transport");
                _hubClient.SubscribeHubMethod("broadcastMessage");
                _hubClient.OnMessageReceived += (locationUpdate) =>
                {
                    UpdateLocationOnMap(locationUpdate);
                };
            }
            catch (Exception ex)
            {
                InformationLabel.Text = "SignalR Connection error: " + ex.Message;
            }
        }

        private async void UpdateLocationOnMap(LocationUpdate locationUpdate)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!_mapHelper.LocationUpdatesDictionary.Keys.Any(driverName => driverName.Equals(locationUpdate.DriverName, StringComparison.OrdinalIgnoreCase)))
                    _mapHelper.AddMapPushPin(locationUpdate);
                else
                    _mapHelper.UpdatePushPin(locationUpdate);
            });
        }
    }
}
