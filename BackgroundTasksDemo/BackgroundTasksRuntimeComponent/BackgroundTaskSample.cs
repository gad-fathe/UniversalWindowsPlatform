using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace BackgroundTasksRuntimeComponent
{
    public sealed class BackgroundTaskSample : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
             GetStringFromURL(taskInstance);
        }

        async void GetStringFromURL(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            HttpClient client = new HttpClient();
            using (HttpResponseMessage response = await client.GetAsync("http://jsonplaceholder.typicode.com/posts/1"))
            using (HttpContent content = response.Content)
            {
                string result = await content.ReadAsStringAsync();
                DoToast(result);
            };
            _deferral.Complete();
        }

        //Show toast notification with retrieved string content:
        private void DoToast(string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode("Background tasks Sample"));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotifier.Show(toast);
        }
    }
}
