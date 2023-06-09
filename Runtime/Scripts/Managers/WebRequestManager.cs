using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Net;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Kitrum.GeeklabSDK
{
    public class WebRequestManager : MonoBehaviour
    {
        private static bool isDebugOn = true;

        private static WebRequestManager instance;

        public static WebRequestManager Instance
        {
            get
            {
                if (instance == null)
                {
                    // Create new GameObject with WebRequestManager component
                    var go = new GameObject(nameof(WebRequestManager));
                    instance = go.AddComponent<WebRequestManager>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        
        public void CheckDataCollectionStatusRequest(Action<string> onSuccess = null, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.CHECK_DATA_COLLECTION_STATUS, "", onSuccess, onError,
                UnityWebRequest.kHttpVerbGET);
        }
        

        public void SendUserMetricsRequest(string data, Action<string> onSuccess = null, Action<string> onError = null)
        {
            SendWebhookRequest("user.metrics", data, onSuccess, onError);
        }

        public void SendAdEventRequest(string data, bool isCustom, Action<string> onSuccess = null, Action<string> onError = null)
        {
            var type = "ad.event";
            if (isCustom) type = "custom.ad.event";

            SendWebhookRequest(type, data, onSuccess, onError);
        }


        public void SendPurchaseMetricsRequest(string data, bool isCustom, Action<string> onSuccess = null, Action<string> onError = null)
        {
            var type = "purchase";
            if (isCustom) type = "custom.purchase";
                
            SendWebhookRequest(type, data, onSuccess, onError);
        }
        
        
        public void VerifyCreativeTokenRequest(string token, Action<string> onSuccess = null, Action<string> onError = null)
        {
            var postData = new
            {
                token = token,
            };
            var json = JsonConvert.SerializeObject(postData);
            SendRequest(ApiEndpointsModel.VERIFY_TOKEN, json, onSuccess, onError);
        }
        

        public void FetchTokenRequest(Action<string> onSuccess, Action<string> onError = null)
        {
            var deviceInfo = DeviceInfoHandler.GetDeviceInfo();
            var postData = new
            {
                dpi = (int)deviceInfo.Dpi,
                width = deviceInfo.Width,
                height = deviceInfo.Height,
                lowPower = deviceInfo.LowPower,
                timezone = deviceInfo.Timezone,
                iosSystem = deviceInfo.IosSystem,
                deviceName = deviceInfo.DeviceName,
                deviceType = deviceInfo.DeviceType,
                generation = deviceInfo.Generation,
                deviceModel = deviceInfo.DeviceModel,
                resolutions = deviceInfo.Resolutions, //array
                installedFonts = deviceInfo.InstalledFonts, //array
                graphicsDeviceID = deviceInfo.GraphicsDeviceID,
                graphicsDeviceVendor = deviceInfo.GraphicsDeviceVendor,
                graphicsDeviceVersion = deviceInfo.GraphicsDeviceVersion,
            };
            var json = JsonConvert.SerializeObject(postData);

            SendRequest(ApiEndpointsModel.FETCH_TOKEN, json, onSuccess, onError, UnityWebRequest.kHttpVerbPOST);
        }

        
        private void SendWebhookRequest(string type, string data, Action<string> onSuccess = null, Action<string> onError = null)
        {
            var currentDate = DateTime.Now;
            var currentDateText = currentDate.ToString("yyyy-MM-dd HH:mm:ss");

            var postData = new
            {
                type = type,
                created = currentDateText,
                creativeToken = TokenHandler.GetCreativeToken(),
                data = data
            };

            var json = JsonConvert.SerializeObject(postData);
            SendRequest(ApiEndpointsModel.WEBHOOK, json, onSuccess, onError);
        }
        
        
        private void SendRequest(string endpoint, string json, Action<string> onSuccess, Action<string> onError = null,
            string method = UnityWebRequest.kHttpVerbPOST, Dictionary<string, string> headerData = null)
        {
            if (IsInternetAvailable())
            {
                StartCoroutine(SendRequestCoroutine(endpoint, json, onSuccess, onError, method, headerData));
            }
            else
            {
                Debug.LogWarning(
                    $"{SDKSettingsModel.GetColorPrefixLog()} There is no Internet connection. Please check your connection and try again.");
            }
        }


        private static IEnumerator SendRequestCoroutine(string endpoint, string json, Action<string> onSuccess,
            Action<string> onError, string method, Dictionary<string, string> headerData = null)
        {
            using (UnityWebRequest www = new UnityWebRequest(endpoint, method))
            {
                if (method == UnityWebRequest.kHttpVerbPOST)
                {
                    var bodyRaw = Encoding.UTF8.GetBytes(json);
                    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                if (headerData != null)
                {
                    foreach (var headerItem in headerData)
                    {
                        www.SetRequestHeader(headerItem.Key, headerItem.Value);
                    }
                }

                if (!string.IsNullOrEmpty(SDKSettingsModel.Instance.Token))
                {
                    www.SetRequestHeader("Authorization", "Bearer " + SDKSettingsModel.Instance.Token);
                }

                yield return www.SendWebRequest();
                
#pragma warning disable CS0618
                if (www.isNetworkError || www.isHttpError)
#pragma warning restore CS0618
                {
                    switch (www.responseCode)
                    {
                        case 400:
                            DebugLogError("Bad request, data not formatted properly.", onError);
                            break;
                        case 401:
                            DebugLogError("API key is not valid.", onError);
                            break;
                        case 404:
                            DebugLogError($"{www.error}\n{www.downloadHandler.text}", onError);
                            break;
                        case 500:
                            DebugLogError("Server error.\n" + www.downloadHandler.text + "\n", onError);
                            break;
                        default:
                            DebugLogError($"Error: {www.error}\n" + www.downloadHandler.text + "\n", onError);
                            break;
                    }
                }
                else
                {
                    try
                    {
                        // onSuccess?.Invoke(www.downloadHandler.text + "\nData Request:" + json + "\n");
                        onSuccess?.Invoke(www.downloadHandler.text);
                    }
                    catch (WebException webEx)
                    {
                        DebugLogError($"Exception encountered: {webEx.Message}", onError);
                    }
                    catch (IOException ioEx)
                    {
                        DebugLogError($"IOException encountered: {ioEx.Message}", onError);
                    }
                    catch (Exception ex)
                    {
                        DebugLogError($"Unexpected exception encountered: {ex.Message}", onError);
                    }
                }
            }
        }



        private static bool IsInternetAvailable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }



        public static void DebugLogError(string message, Action<string> onError)
        {
            if (onError == null && isDebugOn)
            {
                Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} {message}");
            }
            else
            {
                onError?.Invoke($"{SDKSettingsModel.GetColorPrefixLog()} {message}");
            }
        }
    }
}