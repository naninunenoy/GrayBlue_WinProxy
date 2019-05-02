using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.WebSockets;
using GrayBlue_WinProxy.GrayBlue;

namespace GrayBlue_WinProxy {
    class ProxyWebSocket : IBLENotify {
        private readonly Uri uri;
        private readonly ClientWebSocket client;
        private readonly IBLERequest requestTo;
        private bool isFinish = false;

        private static readonly ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);

        public ProxyWebSocket(string host, int port, IBLERequest request) {
            uri = new Uri($"ws://{host}:{port}");
            client = new ClientWebSocket();
            requestTo = request;
        }

        public async Task ConnectAsync() {
            try {
                await client.ConnectAsync(uri, CancellationToken.None);
                Task.Run(WaitForUpdate);
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task CloseAsync() {
            isFinish = true;
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", CancellationToken.None);
        }

        public void Dispose() {
            isFinish = true;
            client.Dispose();
        }
        
        private async Task WaitForUpdate() {
            while (!isFinish) {
                if (client.State != WebSocketState.Open) {
                    isFinish = true;
                    Debug.Write("WebSocket has closed");
                }

                var result = await client.ReceiveAsync(buffer, CancellationToken.None);
                var str = (new UTF8Encoding()).GetString(buffer.Take(result.Count).ToArray());
                Debug.WriteLine($"str={str}");
            }
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
