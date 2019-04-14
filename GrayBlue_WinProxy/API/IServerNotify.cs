using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrayBlue_WinProxy.API {
    interface IServerNotify {
        void OnSensorUpdateNotify(string deviceId, float[] acc, float[] gyro, float[] mag, float[] quat);
        void OnButtonUpdateNotify(string deviceId, bool isPressed, string button, float time);
        void OnDeviceLost(string deviceId);
    }
}
