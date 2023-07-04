using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace Kitrum.GeeklabSDK
{
    public class DeviceInfoHandler : MonoBehaviour
    {
        private static DateTimeOffset sessionStartTime;
        private static TimeSpan sessionDuration;


        static IDeviceModel _deviceModel;
        static IDeviceModel deviceModel {
            get {
                if (_deviceModel == null) {
                    #if UNITY_ANDROID && !UNITY_EDITOR
                    _deviceModel = new AndroidDeviceModel();
                    #elif UNITY_IOS && !UNITY_TVOS && !UNITY_EDITOR
                    _deviceModel = new IOSDeviceModel();
                    #else
                    _deviceModel = new StandardDeviceModel();
                    #endif
                }
                return _deviceModel;
            }
        }


#if UNITY_IOS
        // [DllImport("__Internal")]
        // static extern string GetDeviceModel();

        // [DllImport("__Internal")]
        // static extern string GetInstalledFonts();
        
#endif


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
// #pragma warning disable CS4014
//                 SendDeviceInfo();
// #pragma warning restore CS4014
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
            
// #pragma warning disable CS4014
//             SendDeviceInfo();
// #pragma warning restore CS4014
        }


        public static DeviceInfoModel GetDeviceInfo()
        {
            var deviceGeneration = deviceModel.GetDeviceModel();
            var installedFonts = deviceModel.GetInstalledFonts();
            
            // var deviceGeneration = "";
            // var installedFonts = "";
#if UNITY_IOS && !UNITY_TVOS
            // deviceGeneration = GetDeviceModel();
            // installedFonts = GetInstalledFonts();
#elif UNITY_ANDROID
            using (var javaClass = new AndroidJavaClass("com.kitrum.plugin.DeviceGeneration"))
            {
                deviceGeneration = javaClass.CallStatic<string>("GetDeviceGeneration");
            }
            using (var javaClass = new AndroidJavaClass("com.kitrum.plugin.InstalledFonts"))
            {
                installedFonts = javaClass.CallStatic<string>("GetInstalledFonts");
            }
#endif
            
            var installedFontsArray = installedFonts.Split(',');
            var installedFontsJson = JsonConvert.SerializeObject(installedFontsArray);
            
            
            var resolutionsStr = Screen.resolutions.Select(r => r.width + "x" + r.height).ToArray();
            // var resolutions = JsonConvert.SerializeObject(resolutionsStr);
            
            var timeZone = DateTime.UtcNow.ToString("o");

            // var resolutions = Screen.resolutions;
            // var resolutionList = new StringBuilder();

            // foreach (Resolution res in resolutions)
            // {
            //     resolutionList.Append(res.width + "x" + res.height + ", ");
            // }

            // // Remove the last comma and space
            // if (resolutionList.Length > 2)
            //     resolutionList.Remove(resolutionList.Length - 2, 2);

            var deviceInfo = new DeviceInfoModel
            {
                Dpi = Screen.dpi,
                Width = Screen.width,
                Height = Screen.height,
                LowPower = SystemInfo.batteryLevel < 0.2f,
                Timezone = timeZone,
                IosSystem = SystemInfo.operatingSystem,
                DeviceName = SystemInfo.deviceName,
                DeviceType = SystemInfo.deviceType.ToString(),
                DeviceModel = SystemInfo.deviceModel,
                Resolutions = resolutionsStr,
                InstalledFonts = installedFontsArray,
                Generation = deviceGeneration,
                GraphicsDeviceID = SystemInfo.graphicsDeviceID.ToString(),
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
                            $"{SDKSettingsModel.GetColorPrefixLog()} {response}");
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


    interface IDeviceModel {
        string GetDeviceModel();
        string GetInstalledFonts();
    }
    
    class StandardDeviceModel : IDeviceModel {
        public string GetDeviceModel() {
            // handle standard case here
            return "";
        }

        public string GetInstalledFonts() {
            // handle standard case here
            return "";
        }
    }

#if UNITY_IOS && !UNITY_TVOS
    class IOSDeviceModel : IDeviceModel {
        [DllImport("__Internal")]
        static extern string _GetDeviceModel();

        [DllImport("__Internal")]
        static extern string _GetInstalledFonts();

        public string GetDeviceModel() {
            return _GetDeviceModel();
        }

        public string GetInstalledFonts() {
            return _GetInstalledFonts();
        }
    }
#endif

#if UNITY_ANDROID
    class AndroidDeviceModel : IDeviceModel {
        public string GetDeviceModel() {
            using (var javaClass = new AndroidJavaClass("com.kitrum.plugin.DeviceGeneration")) {
                return javaClass.CallStatic<string>("GetDeviceGeneration");
            }
        }

        public string GetInstalledFonts() {
            using (var javaClass = new AndroidJavaClass("com.kitrum.plugin.InstalledFonts")) {
                return javaClass.CallStatic<string>("GetInstalledFonts");
            }
        }
    }
#endif
}