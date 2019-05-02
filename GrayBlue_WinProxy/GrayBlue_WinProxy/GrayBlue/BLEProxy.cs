using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using GrayBlueUWPCore;

namespace GrayBlue_WinProxy.GrayBlue {
    class BLEProxy : IBLERequest, IConnectionDelegate, INotifyDelegate, IDisposable {
        private readonly IPlugin plugin;
        public IBLENotify BLENotifyDelegate { set; get; }

        public BLEProxy() {
            plugin = Plugin.Instance;
        }

        public void Dispose() {
            plugin.Dispose();
        }

        public async Task<bool> CheckBLEAsync() {
            return await plugin.CanUseBle();
        }

        public async Task<string[]> ScanAsync() {
            return await plugin.Scan();
        }

        public async Task<bool> ConnetAsync(string deviceId) {
            return await plugin.ConnectTo(deviceId, this, this);
        }

        public Task Disconnect(string deviceId) {
            plugin.DisconnectTo(deviceId);
            return Task.CompletedTask;
        }

        public Task Disconnect() {
            plugin.DisconnectAllDevices();
            return Task.CompletedTask;
        }

        // IConnectionDelegate

        void IConnectionDelegate.OnConnectDone(string deviceId) {
            // Do Nothing
        }

        void IConnectionDelegate.OnConnectFail(string deviceId) {
            // Do Nothing
        }

        void IConnectionDelegate.OnConnectLost(string deviceId) {
            BLENotifyDelegate?.OnDeviceLost(deviceId);
        }

        // INotifyDelegate

        void INotifyDelegate.OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            BLENotifyDelegate?.OnIMUDataUpdate(deviceId, acc, gyro, mag, quat);
        }

        void INotifyDelegate.OnButtonPush(string deviceId, string buttonName) {
            BLENotifyDelegate?.OnButtonOperation(deviceId, true, buttonName, 0.0F);
        }

        void INotifyDelegate.OnButtonRelease(string deviceId, string buttonName, float pressTime) {
            BLENotifyDelegate?.OnButtonOperation(deviceId, false, buttonName, pressTime);
        }
    }
}
