using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using GrayBlue_WinProxy.GrayBlue;

namespace GrayBlue_WinProxy {
    class ProxyWebSocket : IBLENotify {
        private readonly Uri uri;
        private readonly ClientWebSocket client;
        private readonly RequestAgent requestAgent;
        private readonly ArraySegment<byte> buffer;
        private readonly UTF8Encoding utf8;
        private IDisposable waitForUpdateDisposable;

        public ProxyWebSocket(string host, int port, IBLERequest request) {
            uri = new Uri($"ws://{host}:{port}");
            client = new ClientWebSocket();
            requestAgent = new RequestAgent(request, this);
            buffer = new ArraySegment<byte>(new byte[1024]);
            utf8 = new UTF8Encoding();
        }

        public async Task ConnectAsync() {
            try {
                await client.ConnectAsync(uri, CancellationToken.None);
                waitForUpdateDisposable = WaitForUpdate();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task CloseAsync() {
            waitForUpdateDisposable.Dispose();
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", CancellationToken.None);
        }

        public void Dispose() {
            waitForUpdateDisposable.Dispose();
            client.Dispose();
        }
        
        private IDisposable WaitForUpdate() {
            return Observable
                .Timer(TimeSpan.Zero)
                .TakeWhile(_ => client.State == WebSocketState.Open)
                .Subscribe(async _ => {
                    // receive json
                    try {
                        var result = await client.ReceiveAsync(buffer, CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text) {
                            var json = utf8.GetString(buffer.Take(result.Count).ToArray());
                            //requestAgent.OnReceiveJson(json);
                        }
                    } catch (Exception ex) {
                        Debug.WriteLine(ex.Message);
                    }
                });
        }

        void IBLENotify.OnRequestDone(string requestName, string requestParam, string response) {
            var json = JsonConverter.ToMethodResultJson(requestName, requestName, response);
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            if (client.State == WebSocketState.Open) {
                client.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        void IBLENotify.OnConnectSuccess(string deviceId) {
            var json = JsonConverter.ToConnectResultJson(deviceId, true);
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            if (client.State == WebSocketState.Open) {
                client.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        void IBLENotify.OnConnectFail(string deviceId) {
            var json = JsonConverter.ToConnectResultJson(deviceId, false);
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            if (client.State == WebSocketState.Open) {
                client.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        void IBLENotify.OnDeviceLost(string deviceId) {
            var json = JsonConverter.ToDeviceLostJson(deviceId);
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            if (client.State == WebSocketState.Open) {
                client.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        void IBLENotify.OnButtonOperation(string devceId, bool isPush, string button, float time) {
            var json = JsonConverter.ToButtonNotifyJson(devceId, isPush, button, time);
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            if (client.State == WebSocketState.Open) {
                client.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        void IBLENotify.OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            var json = JsonConverter.ToIMUNotifyJson(deviceId, acc, gyro, mag, quat);
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
            if (client.State == WebSocketState.Open) {
                client.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
