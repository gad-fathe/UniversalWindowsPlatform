using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Storage;
using Windows.UI.Notifications;

namespace PlacesGeofencingRuntimeComponent
{
    public sealed class GeofenceBackgroundTask : IBackgroundTask
    {

        void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try
            {
                GetGeofenceStateChangedReports();
            }
            catch (UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine("Location permissions are disabled. Enable access through the settings.");
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void GetGeofenceStateChangedReports()
        {
            GeofenceMonitor monitor = GeofenceMonitor.Current;
            Geoposition posLastKnown = monitor.LastKnownGeoposition;

            string geofenceItemEvent = null;

            // Retrieve a vector of state change reports
            var reports = GeofenceMonitor.Current.ReadReports();

            foreach (var report in reports)
            {
                GeofenceState state = report.NewState;
                geofenceItemEvent = report.Geofence.Id;

                if (state == GeofenceState.Removed)
                {
                    GeofenceRemovalReason reason = report.RemovalReason;
                    if (reason == GeofenceRemovalReason.Expired)
                    {
                        geofenceItemEvent += " (Removed/Expired)";
                    }
                    else if (reason == GeofenceRemovalReason.Used)
                    {
                        geofenceItemEvent += " (Removed/Used)";
                    }
                }
                else if (state == GeofenceState.Entered)
                {
                    geofenceItemEvent += " (Entered)";
                }
                else if (state == GeofenceState.Exited)
                {
                    geofenceItemEvent += " (Exited)";
                }
            }
            // NOTE: Other notification mechanisms can be used here, such as Badge and/or Tile updates.
            DoToast(geofenceItemEvent);
        }

        private void DoToast(string eventName)
        {
            // pop a toast for each geofence event
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();

            // Create a two line toast and add audio reminder

            // Here the xml that will be passed to the 
            // ToastNotification for the toast is retrieved
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            // Set both lines of text
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode("Geolocation Sample"));

            //if (1 == numEventsOfInterest)
            //{
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(eventName));
            //}
            //else
            //{
            //    string secondLine = "There are " + numEventsOfInterest + " new geofence events";
            //    toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(secondLine));
            //}

            // now create a xml node for the audio source
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotifier.Show(toast);
        }
    }
}
