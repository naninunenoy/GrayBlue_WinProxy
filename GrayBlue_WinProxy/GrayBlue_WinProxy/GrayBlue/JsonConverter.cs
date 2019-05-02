using System;
using System.Collections.Generic;
using System.Text;
using GrayBlue_WinProxy.GrayBlue.Data;

namespace GrayBlue_WinProxy.GrayBlue {
    static class JsonConverter {
        public static string ToMethodResultJson(string method, string param, string response) {
            return "{}";
        }
        public static string ToConnectResultJson(string deviceId, bool success) {
            return "{}";
        }

        public static string ToDeviceLostJson(string deviveId) {
            return "{}";
        }

        public static string ToIMUNotifyJson(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat) {
            return "{}";
        }

        public static string ToButtonNotifyJson(string devceId, bool isPush, string button, float time) {
            return "{}";
        }

    }
}
