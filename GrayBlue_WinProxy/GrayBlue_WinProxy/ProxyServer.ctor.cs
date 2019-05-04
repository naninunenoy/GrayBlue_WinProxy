using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using GrayBlue_WinProxy.GrayBlue;

namespace GrayBlue_WinProxy {
    partial class ProxyServer {

        public ProxyServer(string host, int port, IBLERequest request) {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"http://{host}:{port}/");
            clients = new List<WebSocket>();
            clientDisposables = new List<IDisposable>();
            requestAgent = new RequestAgent(request, this);
        }
    }
}
