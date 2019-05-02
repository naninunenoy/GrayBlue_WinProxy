using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.WebSockets;

namespace GrayBlue_WinProxy {
    class GrayBlueWebSocket {
        private readonly Uri uri;
        private readonly ClientWebSocket client;

        public GrayBlueWebSocket(string host, int port) {
            uri = new Uri($"ws://{host}:{port}");
            client = new ClientWebSocket();
        }

        public async Task ConnectAsync() {
            await client.ConnectAsync(uri, CancellationToken.None);
        }

        public void Dispose() {
            client.Dispose();
        }
    }
}
