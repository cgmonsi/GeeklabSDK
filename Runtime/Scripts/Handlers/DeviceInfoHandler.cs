using UnityEngine;
using System;
using System.Threading.Tasks;


namespace Kitrum.GeeklabSDK
{
    public class DeviceInfoHandler : MonoBehaviour
    {
        private static DateTimeOffset sessionStartTime;
        private static TimeSpan sessionDuration;


        private void Start()
        {
            sessionStartTime = DateTimeOffset.UtcNow;
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                // Game is paused, calculate session duration and send data
                sessionDuration = DateTimeOffset.UtcNow - sessionStartTime;
#pragma warning disable CS4014
                SendDeviceInfo();
#pragma warning restore CS4014
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
            
#pragma warning disable CS4014
            SendDeviceInfo();
#pragma warning restore CS4014
        }


        private static DeviceInfoModel GetDeviceInfo()
        {
            var deviceInfo = new DeviceInfoModel
            {
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
        
        
        public static async Task<bool> SendDeviceInfo()
        {
            if (!SDKSettingsModel.Instance.SendStatistics) 
                return false;

            var taskCompletionSource = new TaskCompletionSource<bool>();

            sessionDuration = DateTimeOffset.UtcNow - sessionStartTime;

            var deviceInfo = GetDeviceInfo();
            var json = JsonUtility.ToJson(deviceInfo);

            // if (SDKSettingsModel.Instance.ShowDebugLog)
            //     Debug.Log(
            //         $"{SDKSettingsModel.GetColorPrefixLog()} Get Device Information = {JsonUtility.ToJson(deviceInfo, true)}");

            WebRequestManager.Instance.SendUserMetricsRequest(json,
                (response) =>
                {
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log(
                            $"{SDKSettingsModel.GetColorPrefixLog()} Device information successfully sent to Geeklab: {response}");
                    taskCompletionSource.SetResult(true);
                },
                (error) =>
                {
                    Debug.LogError(error);
                    taskCompletionSource.SetResult(false);
                }
            );
            
            return await taskCompletionSource.Task;
        }
    }
}