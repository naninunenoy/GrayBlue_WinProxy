using System;
using System.Threading.Tasks;

namespace GrayBlue_WinProxy {
    class Program {
        const string hostName = "localhost";
        const int portNo = 12345;

        static void Main(string[] args) {
            var server = new GrayBlueWebSocketServer(hostName, portNo);
            Task.Run(() => server.WakeUpAsync());

            Console.ReadKey();
            server.Close();
        }
    }
}
