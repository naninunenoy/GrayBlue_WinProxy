using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using GrayBlue_WinProxy.GrayBlue.Data;

namespace GrayBlue_WinProxy.GrayBlue {
    static class JsonConverter {
        public static string ToMethodResultJson(string method, string param, string response) {
            var result = new MethodResult {
                Method = new Method { Name = method, Param = param },
                Result = response
            };
            var data = new GrayBlueJson<MethodResult> {
                Type = JsonType.Result.ToString(),
                Content = result
            };
            return JsonConvert.SerializeObject(data);
        }
        public static string ToConnectResultJson(string deviceId, bool success) {
            var result = new MethodResult {
                Method = new Method {
                    Name = MethodType.Connect.ToString(),
                    Param = deviceId
                },
                Result = success.ToString()
            };
            var data = new GrayBlueJson<MethodResult> {
                Type = JsonType.Result.ToString(),
                Content = result
            };
            return JsonConvert.SerializeObject(data);
        }

        public static string ToDeviceLostJson(string deviceId) {
            var device = new Device {
                DeviceId = deviceId,
                State = "Lost"
            };
            var data = new GrayBlueJson<Device> {
                Type = JsonType.DeviceStateChange.ToString(),
                Content = device
            };
            return JsonConvert.SerializeObject(data);
        }

        public static string ToIMUNotifyJson(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            var imuData = new IMU {
                DeviceId = deviceId,
                Acc = new Vector3 { x = acc[0], y = acc[1], z = acc[2] },
                Gyro = new Vector3 { x = gyro[0], y = gyro[1], z = gyro[2] },
                Mag = new Vector3 { x = mag[0], y = mag[1], z = mag[2] },
                Quat = new Quaternion { x = quat[0], y = quat[1], z = quat[2], w = quat[3] }
            };
            var data = new GrayBlueJson<IMU> {
                Type = JsonType.NotifyIMU.ToString(),
                Content = imuData
            };
            return JsonConvert.SerializeObject(data);
        }

        public static string ToButtonNotifyJson(string deviceId, bool isPush, string button, float time) {
            var btnData = new Button {
                DeviceId = deviceId,
                ButtonName = button,
                IsPressed = isPush,
                PressTime = time
            };
            var data = new GrayBlueJson<Button> {
                Type = JsonType.NotifyButton.ToString(),
                Content = btnData
            };
            return JsonConvert.SerializeObject(data);
        }
    }
}
