using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Server.Hubs;

namespace GrayBlue_WinProxy.API {
    public class BLEProxy : StreamingHubBase<IClientRequest, IServerNotify>, IClientRequest {
        public Task<bool> CheckBLEAvailableAsync() {
            throw new NotImplementedException();
        }

        public Task<bool> ConnectAsync(string deviceId) {
            throw new NotImplementedException();
        }

        public void Disconnect(string deviceId) {
            throw new NotImplementedException();
        }

        public Task<string[]> ScanAsync() {
            throw new NotImplementedException();
        }
    }
}
