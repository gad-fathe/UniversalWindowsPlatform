using AzureMapsApp.UWP.Communication;
using AzureMapsApp.UWP.Model;
using AzureMapsApp.UWP.Services;
using AzureMapsApp.UWP.Utils;
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
using Windows.UI;
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

namespace AzureMapsApp.UWP
{
    public sealed partial class MainPage : Page
    {
        private ClientSignalR _hubClient;
        private MapManager _mapHelper;
        private MapService _mapService;

        public MainPage()
        {
            this.InitializeComponent();
            _mapHelper = new MapManager(TransportMap);
            _mapService = new MapService();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _mapHelper.InitializeMap();
            await InitializeSignalRClient();
            await GetDirections();
        }

        /// <summary>
        /// Initialize connection with TransportHub and subscribe to receive location updates
        /// </summary>
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

        /// <summary>
        /// Get directions from the API where Azure Maps service is used
        /// </summary>
        private async Task GetDirections()
        {
            var directions = await _mapService.GetDirections(new Model.DirectionsRequest { FromLatitude = 52.2329172, FromLongitude = 20.9911553, ToLatitude = 51.127057, ToLongitude = 16.9218251 });
            _mapHelper.DisplayRoute(directions);
        }
    }
}
