using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GrayBlue_WinProxy.GrayBlue.Data {

    public enum JsonType {
        Undefine = 0,
        Method,
        Result,
        DeviceStateChange,
        NotifyIMU,
        NotifyButton
    }

    [JsonObject("root")]
    public class GrayBlueJson<T> {
        [JsonProperty("type")] public string Type { set; get; }
        [JsonProperty("content")] public T Content { set; get; }
    }

    public enum MethodType {
        Undefine = 0,
        CheckBle,
        Scan,
        Connect,
        Disconnect,
        DisconnectAll
    }

    [JsonObject("method")]
    public class Method {
        [JsonProperty("method_name")] public string Name { set; get; }
        [JsonProperty("method_param")] public string Param { set; get; }
    }

    [JsonObject("result")]
    public class MethodResult {
        [JsonProperty("method")] public Method Method { set; get; }
        [JsonProperty("result")] public string Result { set; get; }
    }

    [JsonObject("device")]
    public class Device {
        [JsonProperty("device_id")] public string DeviceId { set; get; }
        [JsonProperty("device_state")] public string State { set; get; }
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
