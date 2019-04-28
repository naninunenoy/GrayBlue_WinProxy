using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Grpc.Core;
using MagicOnion.Server;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace GrayBlue_WinProxy {
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page, IDisposable {
        Server server;
        string serverHostName = "localhost";
        int serverPortNo = 12345;

        public MainPage() {
            this.InitializeComponent();
            // set windows size
            ApplicationView.PreferredLaunchViewSize = new Size { Height = 320, Width = 369 };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            // server set up
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());
            var service = MagicOnionEngine.BuildServerServiceDefinition(isReturnExceptionStackTraceInErrorDetail: true);
            var port = new ServerPort(serverHostName, serverPortNo, ServerCredentials.Insecure);
            server = new Server {
                Services = { service },
                Ports = { port }
            };
            // server start
            try {
                server.Start();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        public void Dispose() {
            server?.KillAsync();
        }
    }
}
