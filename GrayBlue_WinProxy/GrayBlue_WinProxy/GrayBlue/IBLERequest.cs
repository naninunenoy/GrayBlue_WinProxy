using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace GrayBlue_WinProxy.GrayBlue {
    interface IBLERequest {
        Task<bool> CheckBLEAsync();
        Task<string[]> ScanAsync();
        Task<bool> ConnetAsync(string deviceId);
        Task Disconnect(string deviceId);
        Task Disconnect();
    }
}
