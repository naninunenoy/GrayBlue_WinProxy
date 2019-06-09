GrayBlue_WinProxy
====

<img src="https://github.com/naninunenoy/GrayBlue/blob/doc/doc/icon.png?raw=true" width="200" />

This is proxy software for converting protocol of [GrayBlue](https://github.com/naninunenoy/GrayBlue) data BLE(Bluetooth smart Gatt) to websocket.

<img src="https://img.shields.io/badge/platform-windows10-lightGray.svg" /> 

## Demo
You can receive notified quaternion on UnityEditor with this app.

<img src="https://github.com/naninunenoy/GrayBlue/blob/doc/doc/demo_editor.gif?raw=true" width="200" />

## Description

<img src="https://img.shields.io/badge/Gray-Blue-blue.svg?labelColor=lightGray" /> notifies 9-DOF motion data.

This makes you enable to do test play with GrayBlue devices on Unity Editor. You do not need to wait building project.

### Websocket
This set up http server on `http://127.0.0.1:12345` and wait some client connections. 

When a client order BLE operation (like scan or connection), a server(this) call `Windows.Devices.Bluetooth` API and return result of API.

A server(this) and clients communicate by Json message on websocket prottocol.

### Json
#### 0. root
All json in this app has `type` and `content` like this.

```js
{
    "type" : "XXX", /*names of type*/
    "content" : {
        /* some json content */ 
    }
}
```

If you parse or create Json message, you need to know `type` and their struct of `content`.

#### 1. Method
Clients order some BLE operation (like a scan, connect and disconnect). It express as **Method**.

Example.

* Scan

  ```json
  {
      "type" : "Method",
      "content" : {
          "method_name" : "Scan",
          "method_param" : ""
      }
  }
  ```

* Connect

  ```js
  {
      "type" : "Method",
      "content" : {
          "method_name" : "Connect",
          "method_param" : "BluetoothLE#BluetoothLE98:5f:d3:3a:e8:3c-84:0d:8e:3d:32:02"
      }
  }
  ```

#### 2. Result
Response of BLE operation (like a scan, connect). It express as **Result**.

Example.

* Scan

  ```js
  {
      "type" : "Result",
      "content" : {
          "method" : {
              "method_name" : "Scan",
              "method_param" : ""
          },
          "result" : "BluetoothLE#BluetoothLE98:5f:d3:3a:e8:3c-84:0d:8e:3d:32:02"
      }
  }

* Connect

  ```js
  {
      "type" : "Result",
      "content" : {
          "method" : {
              "method_name" : "Connect",
              "method_param" : "BluetoothLE#BluetoothLE98:5f:d3:3a:e8:3c-84:0d:8e:3d:32:02"
          },
          "result" : "True" // if connect failed "False"
      }
  }

#### 3. DeviceStateChange
A server notify BLE device state when they are changed. It express as **DeviceStateChange**.

* Lost

  ```js
  {
      "type" : "DeviceStateChange",
      "content" : {
          "device_id" : "BluetoothLE#BluetoothLE98:5f:d3:3a:e8:3c-84:0d:8e:3d:32:02",
          "device_state" : "Lost"
      }
  }
  ```

#### 4. NotifyIMU
A server notify 9-DOF sensor vlaue update from GrayBlue. It express as **NotifyIMU**.

* IMU

  ```js
  {
      "type" : "NotifyIMU",
      "content" : {
          "device_id" : "BluetoothLE#BluetoothLE98:5f:d3:3a:e8:3c-84:0d:8e:3d:32:02",
          "acc" : { "x" : 0.0 , "y" : 0.0,  "z" : 1.0 },
          "gyro" : { "x" : 0.0 , "y" : 0.0,  "z" : 0.0 },
          "mag" : { "x" : 0.0 , "y" : 0.0,  "z" : 0.0 },
          "quat" : { "x" : 0.0 , "y" : 0.0,  "z" : 0.0,  "w" : 1.0 },
      }
  }
  ```

#### 5. NotifyButton
A server notify M5Stack-Gray button events. It express as **NotifyButton**.

* Button Press

  ```js
  {
      "type" : "NotifyButton",
      "content" : {
          "device_id" : "BluetoothLE#BluetoothLE98:5f:d3:3a:e8:3c-84:0d:8e:3d:32:02",
          "button" : "A",
          "press" : True,
          "time" : 0.0,
      }
  }
  ```

* Button Release

  ```js
  {
      "type" : "NotifyButton",
      "content" : {
          "device_id" : "BluetoothLE#BluetoothLE98:5f:d3:3a:e8:3c-84:0d:8e:3d:32:02",
          "button" : "A",
          "press" : False,
          "time" : 1.0,
      }
  }
  ```

## Environment
.Net Core 3.0.0-preview4-27615-11

## Library
* System.Reactive
    - https://github.com/dotnet/reactive
    - LICENSE: [Apache-2.0](https://licenses.nuget.org/Apache-2.0)
    - version: 4.1.5

 * Newtonsoft.Json
    - https://www.newtonsoft.com/json
    - LICENSE: [MIT](https://licenses.nuget.org/MIT)
    - version: 12.0.2

* System.Net.WebSockets
    - https://www.nuget.org/packages/System.Net.WebSockets/4.3.0
    - LICENSE: [MS-.NET-Library License](https://go.microsoft.com/fwlink/?LinkId=329770)
    - version: 4.3.0

## Licence
[MIT](https://github.com/naninunenoy/GrayBlue_WinProxy/blob/master/LICENSE)

## Author
[@naninunenoy](https://github.com/naninunenoy)
