using System;
using System.Collections.Generic;
using System.Text;

namespace GrayBlue_WinProxy.GrayBlue {
    interface IBLENotify {
        void OnIMUDataUpdate(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat);
        void OnButtonOperation(string devceId, bool isPush, string button, float time);
        void OnDeviceLost(string deviceId);
    }
}
