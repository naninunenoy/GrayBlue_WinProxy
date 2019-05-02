using System;
using System.Diagnostics;
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
            webSocket = new WebSocket($"ws://{host}:{port}");
            bleProxy = new BLEProxy();
        }

        public void Open() {
            bleProxy.Validate(this);
            webSocket.OnOpen += OnOpenWebSocket;
            webSocket.OnMessage += OnMessageWebSocket;
            webSocket.OnError += OnErrorWebSocket;
            webSocket.OnClose += OnCloseWebSocket;
            webSocket.Connect();
        }

        public void Close() {
            bleProxy.Unvalidate();
            bleProxy.Disconnect().Wait();
            webSocket.OnOpen -= OnOpenWebSocket;
            webSocket.OnMessage -= OnMessageWebSocket;
            webSocket.OnError -= OnErrorWebSocket;
            webSocket.OnClose -= OnCloseWebSocket;
            webSocket.Close();
        }

        // WebSocket callback

        private void OnOpenWebSocket(object sender, EventArgs e) {
            Debug.WriteLine($"OnOpenWebSocket");
        }

        private void OnMessageWebSocket(object sender, MessageEventArgs e) {
            Debug.WriteLine($"OnMessageWebSocket {e.Data}");
        }

        private void OnCloseWebSocket(object sender, CloseEventArgs e) {
            Debug.WriteLine($"OnCloseWebSocket {e.Reason}");
        }

        private void OnErrorWebSocket(object sender, ErrorEventArgs e) {
            Debug.WriteLine($"OnErrorWebSocket {e.Message}");
        }

        // IServerNotify

        void IServerNotify.OnSensorUpdateNotify(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            //Debug.WriteLine($"OnSensorUpdateNotify");
            //throw new NotImplementedException();
        }

        void IServerNotify.OnButtonUpdateNotify(string deviceId, bool isPressed, string button, float time) {
            Debug.WriteLine($"OnButtonUpdateNotify");
            //throw new NotImplementedException();
        }

        void IServerNotify.OnDeviceLost(string deviceId) {
            Debug.WriteLine($"OnDeviceLost");
            //throw new NotImplementedException();
        }
    }
}
