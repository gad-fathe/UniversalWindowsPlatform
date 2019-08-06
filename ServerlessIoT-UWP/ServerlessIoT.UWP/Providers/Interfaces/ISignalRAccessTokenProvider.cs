using ServerlessIoT.UWP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessIoT.UWP.Providers.Interfaces
{
    public interface ISignalRAccessTokenProvider
    {
        Task<SignalRConnectionInfo> AcquireSignalRConnectionInfoAsync();
    }
}
