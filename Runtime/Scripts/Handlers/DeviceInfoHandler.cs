using UnityEngine;
using System;

public class DeviceInfoHandler : MonoBehaviour
{
    private static DateTimeOffset sessionStartTime;
    private static TimeSpan sessionDuration;
    
    
    private void Start()
    {
        sessionStartTime = DateTimeOffset.UtcNow; // UTC time is commonly used for server-related operations
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            // Game is paused, calculate session duration and send data
            sessionDuration = DateTimeOffset.UtcNow - sessionStartTime;
            SendDeviceInfo();
        }
        else
        {
            // Game is resumed, reset session start time
            sessionStartTime = DateTimeOffset.UtcNow;
        }
    }

    private void OnApplicationQuit()
    {
        // Game is quitting, calculate session duration and send data
        sessionDuration = DateTime.UtcNow - sessionStartTime;
        SendDeviceInfo();
    }

    
    public static void SendDeviceInfo() {
        if (!SDKInfoModel.CollectServerData) return;
        
        sessionDuration = DateTimeOffset.UtcNow - sessionStartTime;
        
        var deviceInfo = GetDeviceInfo();
        var json = JsonUtility.ToJson(deviceInfo);

        Debug.Log(JsonUtility.ToJson(deviceInfo, true));
        
        WebRequestManager.Instance.SendDeviceInfoRequest(json,
            (response) => {
                Debug.Log("Device information successfully sent to Geeklab.: " + response);
            },
            (error) => {
                Debug.LogError("Error: " + error);
            }
        );
    }


    private static DeviceInfo GetDeviceInfo()
    {
        var deviceInfo = new DeviceInfo {
            Dpi = Screen.dpi,
            Width = Screen.width,
            Height = Screen.height,
            LowPower = SystemInfo.batteryLevel < 0.2f,
            Timezone = System.TimeZoneInfo.Local.StandardName,
            IosSystem = SystemInfo.operatingSystem,
            DeviceName = SystemInfo.deviceName,
            DeviceType = SystemInfo.deviceType.ToString(),
            DeviceModel = SystemInfo.deviceModel,
            GraphicsDeviceID = SystemInfo.graphicsDeviceID,
            GraphicsDeviceVendor = SystemInfo.graphicsDeviceVendor,
            GraphicsDeviceVersion = SystemInfo.graphicsDeviceVersion,
            SessionStartTime = sessionStartTime.ToString(),
            SessionDurationInSeconds = (int)sessionDuration.TotalSeconds
        };
        
        return deviceInfo;
    }
}