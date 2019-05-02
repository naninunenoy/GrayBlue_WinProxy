using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrayBlueUWPCore;

namespace GrayBlue_WinProxy.API {
    public class BLEProxy : IClientRequest, IConnectionDelegate, INotifyDelegate {
        IServerNotify listener = default;

        public void Validate(IServerNotify listener) { this.listener = listener; }
        public void Unvalidate() { this.listener = default; }

        // IClientRequest

        public async Task<bool> CheckBLEAvailableAsync() {
            return await Plugin.Instance.CanUseBle();
        }

        public async Task<string[]> ScanAsync() {
            var result =  await Plugin.Instance.Scan();
            return result;
        }

        public async Task<bool> ConnectAsync(string deviceId) {
            return await Plugin.Instance.ConnectTo(deviceId, this, this);
        }

        public Task Disconnect(string deviceId) {
            Plugin.Instance.DisconnectTo(deviceId);
            return Task.CompletedTask;
        }

        public Task Disconnect() {
            Plugin.Instance.DisconnectAllDevices();
            return Task.CompletedTask;
        }

        // IConnectionDelegate

        public void OnConnectDone(string deviceId) {
            Debug.WriteLine($"Joined {deviceId}");

        }

        public void OnConnectFail(string deviceId) {
            // Do Nothing
        }

        public void OnConnectLost(string deviceId) {
            listener?.OnDeviceLost(deviceId);
            Debug.WriteLine($"Leaved {deviceId}");
        }

        // INotifyDelegate

        public void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            listener?.OnSensorUpdateNotify(deviceId, acc, gyro, mag, quat);
        }

        public void OnButtonPush(string deviceId, string buttonName) {
            listener?.OnButtonUpdateNotify(deviceId, true, buttonName, 0.0F);
        }

        public void OnButtonRelease(string deviceId, string buttonName, float pressTime) {
            listener?.OnButtonUpdateNotify(deviceId, false, buttonName, pressTime);
        }
    }
}
