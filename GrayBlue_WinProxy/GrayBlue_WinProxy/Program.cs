using System;
using System.Threading.Tasks;

namespace GrayBlue_WinProxy {
    class Program {
        const string hostName = "localhost";
        const int portNo = 12345;
        static readonly GrayBlue.BLEProxy bleProxy = new GrayBlue.BLEProxy();

        static void Main(string[] args) {
            // server setup
            var server = new ProxyServer(hostName, portNo);
            server.Start();
            Task.Run(() => server.RunAsync());
            // add client
            var ws = new ProxyWebSocket(hostName, portNo, bleProxy);
            bleProxy.BLENotifyDelegate = ws;
            Task.Run(() => ws.ConnectAsync());
            // finish with Enter key
            Console.ReadKey();
            server.Close();
            ws.Dispose();
            bleProxy.Dispose();
        }
    }
}
