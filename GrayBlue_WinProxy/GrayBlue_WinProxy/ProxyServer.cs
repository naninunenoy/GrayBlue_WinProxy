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
    class ProxyServer {
        private readonly HttpListener httpListener;
        private readonly List<WebSocket> clients;
        private readonly List<IDisposable> clientDisposables;
        private IDisposable clientWaitDisposable;

        public ProxyServer(string host, int port) {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"http://{host}:{port}/");
            clients = new List<WebSocket>();
            clientDisposables = new List<IDisposable>();
        }

        public void Start() {
            httpListener.Start();
        }

        public void RunAsync() {
            // wait for client
            clientWaitDisposable = Observable
                .Timer(TimeSpan.Zero)
                .Subscribe(async _ => {
                    var listenerContext = await httpListener.GetContextAsync();
                    if (listenerContext.Request.IsWebSocketRequest) {
                        /// httpのハンドシェイクがWebSocketならWebSocket接続開始
                        await WebSocketProcessRequest(listenerContext).ConfigureAwait(false);
                    } else {
                        /// httpレスポンスを返す
                        listenerContext.Response.StatusCode = 400;
                        listenerContext.Response.Close();
                    }
                });
        }

        public void Close() {
            clientWaitDisposable.Dispose();
            httpListener.Close();
            clientDisposables.ForEach(x => x.Dispose());
            clientDisposables.Clear();
            clients.ForEach(x => x.Dispose());
            clients.Clear();
        }

        async Task WebSocketProcessRequest(HttpListenerContext listenerContext) {
            Console.WriteLine("{0}:New Session:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());

            // WebSocketの接続完了を待機してWebSocketオブジェクトを取得する
            var ws = (await listenerContext.AcceptWebSocketAsync(subProtocol: null)).WebSocket;

            // 新規クライアントを追加
            clients.Add(ws);

            // WebSocketの送受信ループ
            var buffer = new ArraySegment<byte>(new byte[1024]);
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
                            Debug.WriteLine("{0}:String Received:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());
                            Debug.WriteLine("Message={0}", Encoding.UTF8.GetString(buffer.Take(data.Count).ToArray()));
                            // 自分以外の各クライアントへ配信
                            Parallel.ForEach(clients,
                                c => {
                                    if (ws != c) {
                                        c.SendAsync(
                                            new ArraySegment<byte>(buffer.Take(data.Count).ToArray()),
                                            WebSocketMessageType.Text,
                                            true,
                                            CancellationToken.None);
                                    }
                                });
                            isOpne = (ws.State == WebSocketState.Open);
                        } else if (data.MessageType == WebSocketMessageType.Close) {
                            // close
                            Debug.WriteLine("{0}:Session Close:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());
                            CloseAndRemove();
                        } else {
                            // Do Nothing
                        }
                    } catch (Exception ex) {
                        // 例外 (クライアントが異常終了)
                        Debug.WriteLine("{0}:Session Abort:{1} {2}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString(), ex.Message);
                        CloseAndRemove();
                    }
                });
            clientDisposables.Add(disposable);
        }
    }
}
