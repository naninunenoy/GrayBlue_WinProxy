using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using GrayBlueUWPCore;

namespace GrayBlue_WinProxy.API {
    public class BLEProxy : StreamingHubBase<IClientRequest, IServerNotify>, IClientRequest,
                            IConnectionDelegate, INotifyDelegate {
        const string groupName = "BLEProxy";
        IGroup bleGroup = null;

        // IClientRequest

        public Task<bool> CheckBLEAvailableAsync() {
            return Plugin.Instance.CanUseBle();
        }

        public Task<string[]> ScanAsync() {
            return Plugin.Instance.Scan();
        }

        public Task<bool> ConnectAsync(string deviceId) {
            return Plugin.Instance.ConnectTo(deviceId, this, this);
        }

        public void Disconnect(string deviceId) {
            Plugin.Instance.DisconnectTo(deviceId);
            Leave().ContinueWith(_ => {
                Debug.WriteLine($"Leaved {deviceId}");
            });
        }

        private async Task Join() {
            bleGroup = await Group.AddAsync(groupName);
        }

        private async Task Leave() {
            if (bleGroup != null) {
                await bleGroup.RemoveAsync(Context);
            }
        }

        // IConnectionDelegate

        public void OnConnectDone(string deviceId) {
            Join().ContinueWith(_ => {
                Debug.WriteLine($"Joined {deviceId}");
            });
        }

        public void OnConnectFail(string deviceId) {
            // Do Nothing
        }

        public void OnConnectLost(string deviceId) {
            Broadcast(bleGroup)?.OnDeviceLost(deviceId);
            Leave().ContinueWith(_ => {
                Debug.WriteLine($"Leaved {deviceId}");
            });
        }

        // INotifyDelegate

        public void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            Broadcast(bleGroup)?.OnSensorUpdateNotify(deviceId, acc, gyro, mag, quat);
        }

        public void OnButtonPush(string deviceId, string buttonName) {
            Broadcast(bleGroup)?.OnButtonUpdateNotify(deviceId, true, buttonName, 0.0F);
        }

        public void OnButtonRelease(string deviceId, string buttonName, float pressTime) {
            Broadcast(bleGroup)?.OnButtonUpdateNotify(deviceId, false, buttonName, pressTime);
        }

        // You can hook OnConnecting/OnDisconnected by override.
        protected override async ValueTask OnDisconnected() {
            // on disconnecting, if automatically removed this connection from group.
            await CompletedTask;
        }
    }
}
