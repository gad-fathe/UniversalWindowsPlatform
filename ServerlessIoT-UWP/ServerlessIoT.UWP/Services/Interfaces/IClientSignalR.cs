using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessIoT.UWP.Services.Interfaces
{
    public interface IClientSignalR
    {
        Task Initialize();
        void SubscribeHubMethod(string methodName);
        Task SendHubMessage(string methodName, string data);
        Task CloseConnection();
        event Action<string> OnMessageReceived;
    }

}
