using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace GrayBlue_WinProxy.API {
    class GrayBlueServer : IServerNotify {
        private readonly Uri uri;
        private readonly MessageWebSocket webSocket;
        private readonly BLEProxy bleProxy;

        public GrayBlueServer(string host, int port) {
            uri = new Uri($"ws://{host}:{port}");
            webSocket = new MessageWebSocket();
            webSocket.Control.MessageType = SocketMessageType.Utf8;
            bleProxy = new BLEProxy();
        }

        public async Task Open() {
            bleProxy.Validate(this);
            webSocket.MessageReceived += OnWebSocketMessageReceive;
            webSocket.Closed += OnWebSocketClose;
            try {
                await webSocket.ConnectAsync(uri).AsTask();
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public void Close() {
            bleProxy.Unvalidate();
            bleProxy.Disconnect().Wait();
            webSocket.MessageReceived -= OnWebSocketMessageReceive;
            webSocket.Closed -= OnWebSocketClose;
            webSocket.Close(1000, "close");
            webSocket.Dispose();
        }

        // MessageWebSocket callback
        private void OnWebSocketClose(IWebSocket sender, WebSocketClosedEventArgs args) {
            Debug.WriteLine($"OnWebSocketClose {args.Code} {args.Reason}");
        }

        private void OnWebSocketMessageReceive(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args) {
            try {
                using (DataReader dataReader = args.GetDataReader()) {
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    Debug.WriteLine($"OnWebSocketMessageReceive {message}");
                }
            } catch (Exception ex) {
                Windows.Web.WebErrorStatus webErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine($"OnWebSocketMessageReceive Exception {webErrorStatus} {ex.Message}");
            }
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
