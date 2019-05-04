using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using GrayBlue_WinProxy.GrayBlue.Data;

namespace GrayBlue_WinProxy.GrayBlue {
    class RequestAgent {
        private readonly IBLERequest bleProxy;
        private readonly IBLENotify notify;

        public RequestAgent(IBLERequest request, IBLENotify notify) {
            this.bleProxy = request;
            this.notify = notify;
        }

        public void OnReceiveJson(string json) {
            // 受信するのはBLEに対する要求(Scan/Connect)のみ
            // センサデータの通知は来たとしても無視
            var data = JsonConvert.DeserializeObject<GrayBlueJson<object>>(json);
            if (data.Type != "Method") {
                return;
            }
            var method = JsonConvert.DeserializeObject<GrayBlueJson<Method>>(json)?.Content;
            if (method == null) {
                return;
            }
            if (!Enum.TryParse(method.Name, out MethodType methodType)) {
                Debug.WriteLine($"unknown method {method.Name}");
                return;
            }
            switch (methodType) {
            case MethodType.CheckBle:
                bleProxy.CheckBLEAsync().ContinueWith(t => {
                    notify.OnRequestDone(methodType.ToString(), "", t.Result.ToString());
                });
                break;
            case MethodType.Scan:
                bleProxy.ScanAsync().ContinueWith(t => {
                    notify.OnRequestDone(methodType.ToString(), "", string.Join(",", t.Result));
                });
                break;
            case MethodType.Connect:
                var deviceId = method.Param;
                bleProxy.ConnetAsync(deviceId).ContinueWith(t => {
                    if (t.Result) {
                        notify.OnConnectSuccess(deviceId);
                    } else {
                        notify.OnConnectFail(deviceId);
                    }
                });
                break;
            case MethodType.Disconnect:
                bleProxy.Disconnect(method.Param);
                break;
            case MethodType.DisconnectAll:
                bleProxy.Disconnect();
                break;
            default:
                // Do Nothing
                break;
            }

        }
    }
}
