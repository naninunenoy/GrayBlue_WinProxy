using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GrayBlue_WinProxy.GrayBlue.Data {
    [JsonObject("method")]
    public class Method {
        [JsonProperty("method")] public string Name { set; get; }
        [JsonProperty("param")] public string Param { set; get; }
    }
    public enum MethodType {
        Undefine = 0,
        CheckBle,
        Scan,
        Connect,
        Disconnect,
        Quit
    }

    [JsonObject("imu")]
    public class IMU {
        [JsonProperty("device_id")] public string DeviceId { set; get; }
        [JsonProperty("acc")] public Vector3 Acc { set; get; }
        [JsonProperty("gyro")] public Vector3 Gyro { set; get; }
        [JsonProperty("mag")] public Vector3 Mag { set; get; }
        [JsonProperty("quat")] public Quaternion Quat { set; get; }
    }

    [JsonObject("vec3")]
    public struct Vector3 {
        public float x, y, z;
    }

    [JsonObject("quat")]
    public struct Quaternion {
        public float w, x, y, z;
    }

    [JsonObject("button")]
    public class Button {
        [JsonProperty("device_id")] public string DeviceId { set; get; }
        [JsonProperty("button")] public string ButtonName { set; get; }
        [JsonProperty("press")] public bool IsPressed { set; get; }
        [JsonProperty("time")] public float PressTime { set; get; }
    }
}
