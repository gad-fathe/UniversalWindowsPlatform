using PushNotificationsApp.UWP.Commands;
using PushNotificationsApp.UWP.Model;
using PushNotificationsApp.UWP.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;

namespace PushNotificationsApp.UWP.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _status;
        private string _registrationId;
        private readonly RestService _restService;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string RegistrationId
        {
            get { return _registrationId; }
            set
            {
                _registrationId = value;
                OnPropertyChanged(nameof(_registrationId));
            }
        }

        public ICommand GetRegistrationIdCommand { get; private set; }

        public ICommand RegisterForPushNotifications { get; private set; }

        public ICommand UnregisterFromPushNotifications { get; private set; }

        public ICommand SendPushNotification { get; private set; }

        public MainViewModel()
        {
            _restService = new RestService();

            GetRegistrationIdCommand = new DelegateCommand(async (parameter) =>
            {
                RegistrationId = await _restService.GetPushRegistrationId();
                Status = "Registration ID obtained: " + RegistrationId;
            });

            RegisterForPushNotifications = new DelegateCommand(async(parameter) =>
            {
                var handle = await InitNotificationsAsync();

                var deviceUpdate = new DeviceRegistration()
                {
                    Handle = handle,
                    Platform = MobilePlatform.wns
                };

                var result = await _restService.EnablePushNotifications(RegistrationId, deviceUpdate);
                if (result)
                    Status = "Successfuly enabled push notifications";
                else
                    Status = "Failed to enable push notifications";
            });

            UnregisterFromPushNotifications = new DelegateCommand(async (parameter) =>
            {
                var result = await _restService.UnregisterFromNotifications(RegistrationId);
                if (result)
                    Status = "Successfuly unregistered from push notifications";
                else
                    Status = "Failed to unregister from push notifications";
            });

            SendPushNotification = new DelegateCommand(async(parameter) =>
            {
                var notification = new PushNotificationsApp.UWP.Model.Notification()
                {
                    Content = "<?xml version=\"1.0\" encoding=\"utf-8\"?><toast><visual><binding template = \"ToastText01\"><text id = \"1\"> Test message </text></binding></visual></toast>"
                };

                var result = await _restService.SendNotification(notification);
                if (result)
                    Status = "Successfuly sent push notifications";
                else
                    Status = "Failed to send push notifications";
            });
        }

        private async Task<string> InitNotificationsAsync()
        {
            // Get a channel URI from WNS.
            var channel = await PushNotificationChannelManager
                .CreatePushNotificationChannelForApplicationAsync();

            channel.PushNotificationReceived += Channel_PushNotificationReceived;

            return channel.Uri;
        }

        private void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            ShowToastNotification(args.ToastNotification);
        }

        private void ShowToastNotification(ToastNotification notification)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            notification.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(notification);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
