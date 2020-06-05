using System;
using System.Linq;
using System.Threading.Tasks;
using UwpSamples.RealTimeAssetsTrackingWithSignalR.UWP.Model;
using UwpSamples.RealTimeAssetsTrackingWithSignalR.UWP.Services;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpSamples.RealTimeAssetsTrackingWithSignalR.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private LiveClientervice _liveTrackingService;
        private MapService _mapService;

        public MainPage()
        {
            this.InitializeComponent();
            _mapService = new MapService(TransportMap);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _mapService.InitializeMap();
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
                _liveTrackingService = new LiveClientervice();
                await _liveTrackingService.Initialize("http://localhost:5000/live-tracking");
                _liveTrackingService.SubscribeHubMethod("location-update");
                _liveTrackingService.OnMessageReceived += (locationUpdate) =>
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
                if (!_mapService.LocationUpdatesDictionary.Keys.Any(driverName => driverName.Equals(locationUpdate.DriverName, StringComparison.OrdinalIgnoreCase)))
                    _mapService.AddMapPushPin(locationUpdate);
                else
                    _mapService.UpdatePushPin(locationUpdate);
            });
        }
    }
}
