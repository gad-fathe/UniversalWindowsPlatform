using AzureSignalRTransportApp.UWP.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;

namespace AzureSignalRTransportApp.UWP.Utils
{
    public class MapManager
    {
        private MapControl _map;
        public Dictionary<string, MapIcon> LocationUpdatesDictionary { get; }

        public MapManager(MapControl map)
        {
            _map = map;
            LocationUpdatesDictionary = new Dictionary<string, MapIcon>();
        }

        /// <summary>
        /// Initialize Bing Map.
        /// </summary>
        public void InitializeMap()
        {
            BasicGeoposition centerPosition = new BasicGeoposition { Latitude = 52.2326, Longitude = 20.7810 };
            Geopoint centerPoint = new Geopoint(centerPosition);

            _map.ZoomLevel = 12;
            _map.Center = centerPoint;
        }

        /// <summary>
        /// Add map push pin with location from location update received from SignalR Service.
        /// </summary>
        /// <param name="locationUpdate"></param>
        public void AddMapPushPin(LocationUpdate locationUpdate)
        {
            var landMarks = new List<MapElement>();
            BasicGeoposition snPosition = new BasicGeoposition { Latitude = locationUpdate.Latitude, Longitude = locationUpdate.Longitude };
            Geopoint snPoint = new Geopoint(snPosition);

            var locationMapIcon = new MapIcon
            {
                Location = snPoint,
                NormalizedAnchorPoint = new Point(0.5, 1.0),
                ZIndex = 0,
                Title = locationUpdate.DriverName
            };

            if (locationUpdate.DriverName.Equals("Adam"))
                locationMapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/CarIcon1.png"));
            else
                locationMapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/CarIcon2.png"));

            landMarks.Add(locationMapIcon);

            var LandmarksLayer = new MapElementsLayer
            {
                ZIndex = 1,
                MapElements = landMarks
            };

            _map.Layers.Add(LandmarksLayer);

            _map.Center = snPoint;
            _map.ZoomLevel = 12;

            LocationUpdatesDictionary.Add(locationUpdate.DriverName, locationMapIcon);
        }

        /// <summary>
        /// Update existing map push pin location.
        /// </summary>
        /// <param name="locationUpdate"></param>
        public void UpdatePushPin(LocationUpdate locationUpdate)
        {
            MapIcon mapIcon;
            LocationUpdatesDictionary.TryGetValue(locationUpdate.DriverName, out mapIcon);
            if (mapIcon != null)
            {
                BasicGeoposition snPosition = new BasicGeoposition { Latitude = locationUpdate.Latitude, Longitude = locationUpdate.Longitude };
                Geopoint snPoint = new Geopoint(snPosition);
                mapIcon.Location = snPoint;
            }
        }
    }
}
