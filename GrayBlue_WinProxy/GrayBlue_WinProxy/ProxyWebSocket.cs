using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.WebSockets;
using GrayBlue_WinProxy.GrayBlue;

namespace GrayBlue_WinProxy {
    class ProxyWebSocket : IBLENotify {
        private readonly Uri uri;
        private readonly ClientWebSocket client;
        private readonly IBLERequest requestTo;

        public ProxyWebSocket(string host, int port, IBLERequest request) {
            uri = new Uri($"ws://{host}:{port}");
            client = new ClientWebSocket();
            requestTo = request;
        }

        public async Task ConnectAsync() {
            await client.ConnectAsync(uri, CancellationToken.None);
        }

        public void Dispose() {
            client.Dispose();
        }

        void IBLENotify.OnButtonOperation(string devceId, bool isPush, string button, float time) {
            throw new NotImplementedException();
        }

        void IBLENotify.OnDeviceLost(string deviceId) {
            throw new NotImplementedException();
        }

        void IBLENotify.OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            throw new NotImplementedException();
        }
    }
}
