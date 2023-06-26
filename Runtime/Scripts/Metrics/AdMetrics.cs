using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Newtonsoft.Json;


namespace Kitrum.GeeklabSDK
{
    [RequireComponent(typeof(AdMetrics))]
    public class AdMetrics : MonoBehaviour, IUnityAdsListener
    {
        private static float startWatchTime;
        private static string gameId;
        private static string adStatus;
        private static string platform;
        private const string INTERSTITIAL_ID = "Interstitial";
        private const string BANNER_ID = "Banner";
        private const string REWARDED_ID = "Rewarded";
        private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

        
        public static AdMetrics Instance;
        // public static AdMetrics Instance
        // {
        //     get
        //     {
        //         if (instance == null)
        //         {
        //             // Create new GameObject with WebRequestManager component
        //             var go = new GameObject(nameof(AdMetrics));
        //             instance = go.AddComponent<AdMetrics>();
        //             DontDestroyOnLoad(go);
        //         }
        //
        //         return instance;
        //     }
        // }
        
        // public static AdMetrics Instance { get; private set; }
        private static bool IsInitialized { get; set; }
        public bool ShowAdWasCalled { get; set; }
        public bool? IsUnityAdsReady { get; set; }
        

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (!SDKSettingsModel.Instance.EnableAdAnalytics)
                return;

            Advertisement.AddListener(this);

#if UNITY_ANDROID
            Advertisement.Initialize(SDKSettingsModel.Instance.GameIdAndroid, SDKSettingsModel.Instance.AdTestMode);
            gameId = SDKSettingsModel.Instance.GameIdAndroid;
            platform = "Android";
#elif UNITY_IOS
        Advertisement.Initialize(SDKSettingsModel.Instance.GameIdIOS, SDKSettingsModel.Instance.AdTestMode);
        gameId = SDKSettingsModel.Instance.GameIdIOS;
        platform = "iOS";
#else
        Advertisement.Initialize(SDKSettingsModel.Instance.GameIdAndroid, SDKSettingsModel.Instance.AdTestMode);
        gameId = SDKSettingsModel.Instance.GameIdAndroid;
        Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Unsupported platform to AD");
        platform = "Android";
#endif

            var eventSystems = FindObjectsOfType<EventSystem>();
            if (eventSystems is { Length: >= 2 })
            {
                Destroy(eventSystems[0].gameObject);
            }
        }


        private IEnumerator WaitForAdInitialization()
        {
            while (!Advertisement.IsReady(REWARDED_ID + "_" + platform))
            {
                yield return waitForSeconds;
            }

            Advertisement.Show(REWARDED_ID + "_" + platform);
        }


        public void OnUnityAdsReady(string placementId)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} OnUnityAdsReady placementId - {placementId}");
            IsInitialized = true;
            ShowAdWasCalled = false;
            IsUnityAdsReady = true;
            adStatus = "";
        }


        public void OnUnityAdsDidError(string message)
        {
            Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Log the error: {message}");
            adStatus = $"Error: {message}";
            startWatchTime = Time.time - startWatchTime;
            ShowAdWasCalled = true;
            IsUnityAdsReady = false;
            
#pragma warning disable CS4014
            var data = new List<object>
            {
                new { adStatus = "error", message = message },
            };
            var postData = JsonConverter.ConvertToJson(data);
            SendMetrics(postData, true);
#pragma warning restore CS4014
        }


        public void OnUnityAdsDidStart(string placementId)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Ad started. PlacementId: {placementId}");
            adStatus = "Started";
            startWatchTime = Time.time;
        }


        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log(
                    $"{SDKSettingsModel.GetColorPrefixLog()} Ad finished. PlacementId: {placementId}, Result: {showResult}");
            adStatus = showResult.ToString();
            startWatchTime = Time.time - startWatchTime;
            
#pragma warning disable CS4014
            var data = new List<object>
            {
                new { adStatus, placementId, watchTime = startWatchTime },
            };
            var postData = JsonConverter.ConvertToJson(data);
            SendMetrics(postData, true);
#pragma warning restore CS4014
        }
        
        
        public void ShowAd()
        {
            if (!SDKSettingsModel.Instance.IsSDKEnabled || !SDKSettingsModel.Instance.EnableAdAnalytics) 
                return;

            Instance.StartCoroutine(WaitForAdInitialization());
        }
        
        
        public static async Task<bool> SendMetrics(string postData = null, bool isCustom = false)
        {
            if (!SDKSettingsModel.Instance.SendStatistics) 
                return false;
            
            var taskCompletionSource = new TaskCompletionSource<bool>();

            WebRequestManager.Instance.SendAdEventRequest(postData, isCustom, s =>
            {
                if (SDKSettingsModel.Instance.ShowDebugLog)
                    Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} {s}");
                taskCompletionSource.SetResult(true);
            }, s =>
            {
                Debug.LogError($"{SDKSettingsModel.GetColorPrefixLog()} {s}");
                taskCompletionSource.SetResult(false);
            });
          
            return await taskCompletionSource.Task;
        }
    }
}
