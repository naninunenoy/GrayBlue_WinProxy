using System;
using System.Collections.Generic;
using System.Text;

namespace GrayBlue_WinProxy.GrayBlue {
    interface IBLENotify {
        void OnRequestDone(string requestName, string requestParam, string response);
        void OnConnectSuccess(string deviceId);
        void OnConnectFail(string deviceId);
        void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat);
        void OnButtonOperation(string devceId, bool isPush, string button, float time);
        void OnDeviceLost(string deviceId);
    }
}
