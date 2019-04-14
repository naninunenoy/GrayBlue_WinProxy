using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicOnion;

namespace GrayBlue_WinProxy.API {
    interface IClientRequest : IStreamingHub<IClientRequest, IServerNotify> {
        Task<bool> CheckBLEAvailableAsync();
        Task<string[]> ScanAsync();
        Task<bool> ConnectAsync(string deviceId);
        void Disconnect(string deviceId);
    }
}
