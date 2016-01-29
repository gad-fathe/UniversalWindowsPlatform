using PlacesGeofencing.Pages;
using PlacesGeofencingRuntimeComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PlacesGeofencing
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            InitializeComponent();
            Suspending += OnSuspending;
        }

        //Background geofence task:
        private async void requestLocationAccess()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    // Create Geolocator and define perodic-based tracking (2 second interval).
                    System.Diagnostics.Debug.WriteLine("Access to location is allowed.");
                    break;

                case GeolocationAccessStatus.Denied:
                    System.Diagnostics.Debug.WriteLine("Access to location is denied.");
                    break;

                case GeolocationAccessStatus.Unspecified:
                    System.Diagnostics.Debug.WriteLine("Unspecificed error!");
                    break;
            }
        }

        private const string SampleBackgroundTaskName = "GeofenceBackgroundTask";
        private IBackgroundTaskRegistration _geofenceTask = null;
        async private void registerBackgroundTask()
        {
            try
            {
                // Get permission for a background task from the user. If the user has already answered once,
                // this does nothing and the user must manually update their preference via PC Settings.
                BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

                // Regardless of the answer, register the background task. If the user later adds this application
                // to the lock screen, the background task will be ready to run.
                // Create a new background task builder
                BackgroundTaskBuilder geofenceTaskBuilder = new BackgroundTaskBuilder();

                geofenceTaskBuilder.Name = SampleBackgroundTaskName;
                geofenceTaskBuilder.TaskEntryPoint = typeof(GeofenceBackgroundTask).FullName;

                // Create a new location trigger
                var trigger = new LocationTrigger(LocationTriggerType.Geofence);

                // Associate the locationi trigger with the background task builder
                geofenceTaskBuilder.SetTrigger(trigger);

                // If it is important that there is user presence and/or
                // internet connection when OnCompleted is called
                // the following could be called before calling Register()
                //SystemCondition condition = new SystemCondition(SystemConditionType.UserPresent | SystemConditionType.InternetAvailable);
                //geofenceTaskBuilder.AddCondition(condition);

                // Register the background task
                _geofenceTask = geofenceTaskBuilder.Register();

                // Associate an event handler with the new background task
                _geofenceTask.Completed += OnCompleted;

                switch (backgroundAccessStatus)
                {

                    case BackgroundAccessStatus.Unspecified:
                    case BackgroundAccessStatus.Denied:
                        System.Diagnostics.Debug.WriteLine("Not able to run in background. Application must be added to the lock screen.");
                        break;

                    default:
                        // BckgroundTask is allowed
                        System.Diagnostics.Debug.WriteLine("Geofence background task registered.");

                        // Need to request access to location
                        // This must be done with the background task registeration
                        // because the background task cannot display UI.
                        requestLocationAccess();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            try
            {
                e.CheckResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }


        private void unregisterBackgroundTask()
        {
            // Unregister the background task
            if (_geofenceTask!=null)
            {
                _geofenceTask.Unregister(true);
                _geofenceTask = null;
            }
            System.Diagnostics.Debug.WriteLine("Geofence background task unregistered");
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.Navigated += (s, navigationArgs) =>
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               ((Frame)s).CanGoBack ?
               AppViewBackButtonVisibility.Visible :
               AppViewBackButtonVisibility.Collapsed;
                };

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(PlacesMapPage), e.Arguments);
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, navigationArgs) =>
            {
                rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null)
                    return;
                if (rootFrame.CanGoBack && navigationArgs.Handled == false)
                {
                    navigationArgs.Handled = true;
                    rootFrame.GoBack();
                }
            };

            // Ensure the current window is active
            Window.Current.Activate();
            requestLocationAccess();
            registerBackgroundTask();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
