using PlacesGeofencing.Classes;
using PlacesGeofencing.ContentDialogs;
using PlacesGeofencingRuntimeComponent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlacesGeofencing.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlacesMapPage : Page
    {
        IList<PlaceContentDialog> _dialogsList;
        IList<Geofence> _geofences;
        MapIcon _userLocationIcon;
        public PlacesMapPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            GeofenceMonitor.Current.GeofenceStateChanged -= OnGeofenceStateChanged;
            GeofenceMonitor.Current.StatusChanged -= OnGeofenceStatusChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            requestLocationAccess();
            _dialogsList = new List<PlaceContentDialog>();
        }

        private async void requestLocationAccess()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // Create Geolocator and define perodic-based tracking (2 second interval).
                    System.Diagnostics.Debug.WriteLine("Access to location is allowed.");
                    configureGeofenceMonitor();
                    getCurrentLocation();
                    break;

                case GeolocationAccessStatus.Denied:
                    System.Diagnostics.Debug.WriteLine("Access to location is denied.");
                    break;

                case GeolocationAccessStatus.Unspecified:
                    System.Diagnostics.Debug.WriteLine("Unspecificed error!");
                    break;
            }
        }

        private void getCurrentLocation()
        {
            Geolocator _geolocator = new Geolocator { ReportInterval = 2000 };

            // Subscribe to the PositionChanged event to get location updates.
            _geolocator.PositionChanged += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Position changed: " + e.Position.Coordinate.Point.Position.Latitude + " " + e.Position.Coordinate.Point.Position.Longitude);

                showUserLocation(e.Position.Coordinate.Point.Position.Latitude, e.Position.Coordinate.Point.Position.Longitude);
            };

            // Subscribe to StatusChanged event to get updates of location status changes.
            _geolocator.StatusChanged += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("StatusChanged: " + e.Status);

                if (e.Status == PositionStatus.Ready)
                {
                }
            };
        }

        private void configureGeofenceMonitor()
        {
            _geofences = GeofenceMonitor.Current.Geofences;
            GeofenceMonitor.Current.GeofenceStateChanged += OnGeofenceStateChanged;
            GeofenceMonitor.Current.StatusChanged += OnGeofenceStatusChanged;
            createGeofence();
        }

        private void OnGeofenceStatusChanged(GeofenceMonitor sender, object args)
        {
            System.Diagnostics.Debug.WriteLine(sender.Status + "");
        }

        private async void OnGeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (GeofenceStateChangeReport report in reports)
                    {
                        GeofenceState state = report.NewState;

                        Geofence geofence = report.Geofence;

                        if (state == GeofenceState.Removed)
                        {
                            // Remove the geofence from the geofences collection.
                            GeofenceMonitor.Current.Geofences.Remove(geofence);
                        }
                        else if (state == GeofenceState.Entered)
                        {
                            // Your app takes action based on the entered event.

                            // NOTE: You might want to write your app to take a particular
                            // action based on whether the app has internet connectivity.
                            System.Diagnostics.Debug.WriteLine("You have entered geofence!");
                            showNotificationPin(report.Geoposition.Coordinate.Point.Position.Latitude, report.Geoposition.Coordinate.Point.Position.Latitude);
                        }
                        else if (state == GeofenceState.Exited)
                        {
                            // Your app takes action based on the exited event.
                            // NOTE: You might want to write your app to take a particular
                            // action based on whether the app has internet connectivity.
                            System.Diagnostics.Debug.WriteLine("You have exited geofence!");
                            if(_dialogsList.Count>0)
                            _dialogsList[0].Hide();
                        }
                    }
                });
        }

        private void createGeofence()
        {
            ExtendedGeofenceFactory extendedGeofence = new ExtendedGeofenceFactory();
            Geofence createdGeofence = extendedGeofence.CreateGeofence("FENCE", <LATITUDE HERE>, <LONGITUDE HERE>, 0, 40, false, 2, 1);
            if (_geofences != null)
                if (!_geofences.Any(geofence => geofence.Id.Equals(createdGeofence.Id)))
                    _geofences.Add(createdGeofence);
        }

        private async void showUserLocation(double latitude, double longitude)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MessageMapControl.MapElements.Remove(_userLocationIcon);
                _userLocationIcon = new MapIcon();
                _userLocationIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/LocationPin.png"));
                Geopoint userLocationGeoPoint = new Geopoint(new BasicGeoposition()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = 0
                });

                _userLocationIcon.Location = userLocationGeoPoint;
                _userLocationIcon.NormalizedAnchorPoint = new Point(0, 0.5);
                _userLocationIcon.Title = "I am here";
                MessageMapControl.Center = userLocationGeoPoint;
                MapControl.SetLocation(_userLocationIcon, userLocationGeoPoint);
                MessageMapControl.MapElements.Add(_userLocationIcon);
                System.Diagnostics.Debug.WriteLine("Map elements count: " + MessageMapControl.MapElements.Count);
            });
        }

        private async void showNotificationPin(double latitude, double longitude)
        {
            PlaceContentDialog newPersonDialog = new PlaceContentDialog("PLACE TITLE", "This is sample text.");
            _dialogsList.Add(newPersonDialog);
            await newPersonDialog.ShowAsync();
        }
    }
}
