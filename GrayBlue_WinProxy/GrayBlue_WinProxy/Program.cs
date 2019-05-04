﻿using System;
using System.Threading.Tasks;

namespace GrayBlue_WinProxy {
    class Program {
        const string hostName = "127.0.0.1";
        const int portNo = 12345;
        static readonly GrayBlue.BLEProxy bleProxy = new GrayBlue.BLEProxy();

        static void Main(string[] args) {
            // server setup
            Console.WriteLine("Awake");
            var server = new ProxyServer(hostName, portNo, bleProxy);
            server.Start();
            server.RunAsync();
            Console.WriteLine("Server Open");
            // attach to bleProxy
            bleProxy.BLENotifyDelegate = server;
            // finish with Enter key
            Console.WriteLine("Put [Enter] to finish");
            Console.ReadKey();
            server.Close();
            bleProxy.Dispose();
        }
    }
}
