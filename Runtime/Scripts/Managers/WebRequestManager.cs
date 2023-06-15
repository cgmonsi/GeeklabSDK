using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Net;
using System.IO;
using System;


namespace Kitrum.GeeklabSDK
{
    public class WebRequestManager : MonoBehaviour
    {
        public bool isDebugOn = true;

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


        public void SendDeviceInfoRequest(string json, Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.SEND_DEVICE_INFO, json, onSuccess, onError, UnityWebRequest.kHttpVerbPOST);
        }

        public void GetTokenRequest(Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.GET_TOKEN, "", onSuccess, onError, UnityWebRequest.kHttpVerbGET);
        }

        public void SetTokenRequest(string json, Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.SEND_TOKEN + json, "", onSuccess, onError, UnityWebRequest.kHttpVerbPOST);
        }


        public void CheckDataCollectionStatusRequest(Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.CHECK_DATA_COLLECTION_STATUS, "", onSuccess, onError,
                UnityWebRequest.kHttpVerbGET);
        }

        public void SendTokenRequest(string json, Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.SEND_TOKEN, json, onSuccess, onError, UnityWebRequest.kHttpVerbGET);
        }


        public void SendAdMetricsRequest(string json, Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.SEND_AD_METRICS, json, onSuccess, onError);
        }

        public void SendEngagementMetricsRequest(string json, Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.SEND_ENGAGEMENT_METRICS, json, onSuccess, onError);
        }

        public void SendPurchaseMetricsRequest(string json, Action<string> onSuccess, Action<string> onError = null)
        {
            SendRequest(ApiEndpointsModel.SEND_PURCHASE_METRICS, json, onSuccess, onError);
        }


        private void SendRequest(string endpoint, string json, Action<string> onSuccess, Action<string> onError = null,
            string method = UnityWebRequest.kHttpVerbPOST)
        {
            if (IsInternetAvailable())
            {
                StartCoroutine(SendRequestCoroutine(endpoint, json, onSuccess, onError, method));
            }
            else
            {
                Debug.LogWarning(
                    $"{SDKSettingsModel.GetColorPrefixLog()} There is no Internet connection. Please check your connection and try again.");
            }
        }


        private IEnumerator SendRequestCoroutine(string endpoint, string json, Action<string> onSuccess,
            Action<string> onError, string method)
        {
            using var www = new UnityWebRequest(endpoint, method);

            if (method == UnityWebRequest.kHttpVerbPOST)
            {
                var bodyRaw = Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                DebugLogError($"Error: {www.error}", onError);
            }
            else
            {
                try
                {
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

        private static bool IsInternetAvailable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }



        private void DebugLogError(string message, Action<string> onError)
        {
            if (onError == null && isDebugOn)
            {
                Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} {message}");
            }
            else
            {
                onError?.Invoke($"{SDKSettingsModel.GetColorPrefixLog()} Unexpected exception encountered: {message}");
            }
        }
    }
}