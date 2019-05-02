using System;
using System.Threading.Tasks;

namespace GrayBlue_WinProxy {
    class Program {
        const string hostName = "localhost";
        const int portNo = 12345;

        static void Main(string[] args) {
            // server setup
            var server = new GrayBlueWebSocketServer(hostName, portNo);
            server.Start();
            Task.Run(() => server.RunAsync());
            // add client
            var ws = new GrayBlueWebSocket(hostName, portNo);
            Task.Run(() => ws.ConnectAsync());

            Console.ReadKey();
            server.Close();
            ws.Dispose();
        }
    }
}
