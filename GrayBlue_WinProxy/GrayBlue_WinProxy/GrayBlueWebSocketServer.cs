using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.WebSockets;
using System.Net;

namespace GrayBlue_WinProxy {
    /**
     * 参考：http://kimux.net/?p=956
     **/
    class GrayBlueWebSocketServer {
        private readonly HttpListener httpListener;
        private bool isFinish = false;
        static List<WebSocket> clients = new List<WebSocket>();

        public GrayBlueWebSocketServer(string host, int port) {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"http://{host}:{port}/");
        }

        public async Task WakeUpAsync() {
            httpListener.Start();
            // wait for client
            while (!isFinish) {
                var listenerContext = await httpListener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest) {
                    /// httpのハンドシェイクがWebSocketならWebSocket接続開始
                    ProcessRequest(listenerContext);
                } else {
                    /// httpレスポンスを返す
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }

        public void Close() {
            isFinish = true;
            httpListener.Close();
            Parallel.ForEach(
                clients,
                ws => { ws.Dispose(); }
                );
            clients.Clear();
        }

        static async void ProcessRequest(HttpListenerContext listenerContext) {
            Console.WriteLine("{0}:New Session:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());

            /// WebSocketの接続完了を待機してWebSocketオブジェクトを取得する
            var ws = (await listenerContext.AcceptWebSocketAsync(subProtocol: null)).WebSocket;

            /// 新規クライアントを追加
            clients.Add(ws);

            /// WebSocketの送受信ループ
            while (ws.State == WebSocketState.Open) {
                try {
                    var buff = new ArraySegment<byte>(new byte[1024]);

                    /// 受信待機
                    var ret = await ws.ReceiveAsync(buff, CancellationToken.None);

                    if (ret.MessageType == WebSocketMessageType.Text) {
                        /// テキスト
                        Debug.WriteLine("{0}:String Received:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());
                        Debug.WriteLine("Message={0}", Encoding.UTF8.GetString(buff.Take(ret.Count).ToArray()));

                        /// 各クライアントへ配信
                        Parallel.ForEach(clients,
                            p => p.SendAsync(new ArraySegment<byte>(buff.Take(ret.Count).ToArray()),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None));
                    } else if (ret.MessageType == WebSocketMessageType.Close) {
                        /// クローズ
                        Debug.WriteLine("{0}:Session Close:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());
                        break;
                    }
                } catch {
                    /// 例外 クライアントが異常終了しやがった
                    Debug.WriteLine("{0}:Session Abort:{1}", DateTime.Now.ToString(), listenerContext.Request.RemoteEndPoint.Address.ToString());
                    break;
                }
            }

            /// クライアントを除外する
            clients.Remove(ws);
            ws.Dispose();
        }
    }
}
