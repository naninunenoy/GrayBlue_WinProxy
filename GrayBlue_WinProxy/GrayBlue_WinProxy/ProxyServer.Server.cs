using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace GrayBlue_WinProxy {
    /**
     * 参考：http://kimux.net/?p=956
     **/
    partial class ProxyServer {
        private readonly HttpListener httpListener;
        private readonly List<WebSocket> clients;
        private readonly List<IDisposable> clientDisposables;
        private IDisposable clientWaitDisposable;

        public void Start() {
            httpListener.Start();
        }

        public void RunAsync() {
            // wait for client
            clientWaitDisposable = Observable
                .Timer(TimeSpan.Zero)
                .SubscribeOn(ThreadPoolScheduler.Instance)
                .Subscribe(async _ => {
                    var listenerContext = await httpListener.GetContextAsync();
                    if (listenerContext.Request.IsWebSocketRequest) {
                        // httpのハンドシェイクがWebSocketならWebSocket接続開始
                        await WebSocketProcessRequest(listenerContext).ConfigureAwait(false);
                    } else {
                        // httpレスポンスを返す
                        listenerContext.Response.StatusCode = 400;
                        listenerContext.Response.Close();
                    }
                });
        }

        public void Close() {
            clientWaitDisposable?.Dispose();
            httpListener?.Close();
            clientDisposables.ForEach(x => x.Dispose());
            clientDisposables.Clear();
            clients.ForEach(x => x.Dispose());
            clients.Clear();
        }

        async Task WebSocketProcessRequest(HttpListenerContext listenerContext) {
            Debug.WriteLine($"New Session:{listenerContext.Request.RemoteEndPoint.Address}");

            // WebSocketの接続完了を待機してWebSocketオブジェクトを取得する
            var ws = (await listenerContext.AcceptWebSocketAsync(subProtocol: null)).WebSocket;
            Debug.WriteLine($"ws State:{ws.State} {ws.SubProtocol}");

            // 新規クライアントを追加
            clients.Add(ws);

            // WebSocketの送受信ループ
            var buffer = new ArraySegment<byte>(new byte[1024]);
            var address = listenerContext.Request.RemoteEndPoint.Address;
            var isOpne = (ws.State == WebSocketState.Open);

            // エラー時の処理をまとめる
            void CloseAndRemove() {
                isOpne = false;
                // クライアントを除外する
                clients.Remove(ws);
                ws.Dispose();
            };

            var disposable = Observable
                .Timer(TimeSpan.Zero)
                .TakeWhile(_ => isOpne)
                .SubscribeOn(ThreadPoolScheduler.Instance)
                .Subscribe(async _ => {
                    // 受信待機
                    try {
                        var data = await ws.ReceiveAsync(buffer, CancellationToken.None);
                        if (data.MessageType == WebSocketMessageType.Text) {
                            Debug.WriteLine($"String Received:{address}");
                            var rawData = buffer.Take(data.Count).ToArray();
                            var message = Encoding.UTF8.GetString(rawData);
                            Debug.WriteLine($"Message:{message}");

                            // jsonを解析し、MethodならBLEの操作を行う
                            requestAgent.OnReceiveJson(message);

                            isOpne = (ws.State == WebSocketState.Open);
                        } else if (data.MessageType == WebSocketMessageType.Close) {
                            // close
                            Debug.WriteLine($"Session Close:{address}");
                            CloseAndRemove();
                        } else {
                            // Do Nothing
                        }
                    } catch (Exception ex) {
                        // 例外 (クライアントが異常終了)
                        Debug.WriteLine($"Session Abort:{address} {ex.Message}");
                        CloseAndRemove();
                    }
                });
            clientDisposables.Add(disposable);
        }

        private async Task Broadcast(string message) {
            var buff = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            var tasks = clients
                .Where(x => x.State == WebSocketState.Open)
                .Select(x => x.SendAsync(buff, WebSocketMessageType.Text, true, CancellationToken.None));
            await Task.WhenAll(tasks);
        }
    }
}
