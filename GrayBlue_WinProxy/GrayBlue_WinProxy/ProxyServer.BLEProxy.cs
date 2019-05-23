using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using GrayBlue_WinProxy.GrayBlue;

namespace GrayBlue_WinProxy {
    partial class ProxyServer : IBLENotify {
        private readonly RequestAgent requestAgent;
        object lockObject = new object();

        void IBLENotify.OnRequestDone(string requestName, string requestParam, string response) {
            var json = JsonConverter.ToMethodResultJson(requestName, requestParam, response);
            lock (lockObject) {
                RunTaskOn(context, Broadcast(json));
            }
        }

        void IBLENotify.OnConnectSuccess(string deviceId) {
            var json = JsonConverter.ToConnectResultJson(deviceId, true);
            lock (lockObject) {
                RunTaskOn(context, Broadcast(json));
            }
        }

        void IBLENotify.OnConnectFail(string deviceId) {
            var json = JsonConverter.ToConnectResultJson(deviceId, false);
            lock (lockObject) {
                RunTaskOn(context, Broadcast(json));
            }
        }

        void IBLENotify.OnDeviceLost(string deviceId) {
            var json = JsonConverter.ToDeviceLostJson(deviceId);
            lock (lockObject) {
                RunTaskOn(context, Broadcast(json));
            }
        }

        void IBLENotify.OnButtonOperation(string devceId, bool isPush, string button, float time) {
            var json = JsonConverter.ToButtonNotifyJson(devceId, isPush, button, time);
            lock (lockObject) {
                RunTaskOn(context, Broadcast(json));
            }
        }

        void IBLENotify.OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            var json = JsonConverter.ToIMUNotifyJson(deviceId, acc, gyro, mag, quat);
            lock (lockObject) {
                RunTaskOn(context, Broadcast(json));
            }
        }

        void RunTaskOn(SynchronizationContext on, Task task) {
            on?.Post(async _ => { await task; }, null);
        }
    }
}
