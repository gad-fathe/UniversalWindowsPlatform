using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationsApp.UWP.Model
{
    public class Notification : DeviceRegistration
    {
        public string Content { get; set; }
    }
}
