using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GrayBlue_WinProxy.API {
    class GrayBlueServer : IServerNotify {
        private readonly WebSocket webSocket;
        private readonly BLEProxy bleProxy;

        public GrayBlueServer(string host, int port) {
            webSocket = new WebSocket($"ws://{host}:{port}/");
            bleProxy = new BLEProxy();
        }

        public void Open() {
            bleProxy.Validate(this);
            webSocket.OnOpen += WebSocket_OnOpen;
            webSocket.OnMessage += WebSocket_OnMessage;
            webSocket.OnError += WebSocket_OnError;
            webSocket.OnClose += WebSocket_OnClose;
            webSocket.Connect();
        }

        public void Close() {
            bleProxy.Unvalidate();
            bleProxy.Disconnect().Wait();
            webSocket.OnOpen -= WebSocket_OnOpen;
            webSocket.OnMessage -= WebSocket_OnMessage;
            webSocket.OnError -= WebSocket_OnError;
            webSocket.OnClose -= WebSocket_OnClose;
            webSocket.Close();
        }

        // WebSocket callback

        private void WebSocket_OnClose(object sender, CloseEventArgs e) {
            throw new NotImplementedException();
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e) {
            throw new NotImplementedException();
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e) {
            throw new NotImplementedException();
        }

        private void WebSocket_OnOpen(object sender, EventArgs e) {
            throw new NotImplementedException();
        }

        // IServerNotify

        void IServerNotify.OnSensorUpdateNotify(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            throw new NotImplementedException();
        }

        void IServerNotify.OnButtonUpdateNotify(string deviceId, bool isPressed, string button, float time) {
            throw new NotImplementedException();
        }

        void IServerNotify.OnDeviceLost(string deviceId) {
            throw new NotImplementedException();
        }
    }
}
