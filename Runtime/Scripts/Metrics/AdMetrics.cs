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
    public class AdMetrics : MonoBehaviour
#if UNITY_2020_1_OR_NEWER
        , IUnityAdsLoadListener, IUnityAdsInitializationListener, IUnityAdsShowListener
#else
        , IUnityAdsListener
#endif
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

#if UNITY_2020_1_OR_NEWER
#else
            Advertisement.AddListener(this);
#endif
            

#if UNITY_ANDROID
            Advertisement.Initialize(SDKSettingsModel.Instance.GameIdAndroid, SDKSettingsModel.Instance.AdTestMode, this);
            gameId = SDKSettingsModel.Instance.GameIdAndroid;
            platform = "Android";
#elif UNITY_IOS
        Advertisement.Initialize(SDKSettingsModel.Instance.GameIdIOS, SDKSettingsModel.Instance.AdTestMode);
        gameId = SDKSettingsModel.Instance.GameIdIOS;
        platform = "iOS";
#else
        Advertisement.Initialize(SDKSettingsModel.Instance.GameIdAndroid, SDKSettingsModel.Instance.AdTestMode, this);
        gameId = SDKSettingsModel.Instance.GameIdAndroid;
        Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Unsupported platform to AD");
        platform = "Android";
#endif

            var eventSystems = FindObjectsOfType<EventSystem>();
            if (eventSystems.Length >= 2)
            {
                Destroy(eventSystems[0].gameObject);
            }
        }


        private IEnumerator WaitForAdInitialization()
        {
#if UNITY_2020_1_OR_NEWER
            while (!Advertisement.isInitialized)
            {
                yield return waitForSeconds;
            }   
            Advertisement.Show(REWARDED_ID + "_" + platform, this);
#else
            while (!Advertisement.IsReady(REWARDED_ID + "_" + platform))
            {
                yield return waitForSeconds;
            }   
            Advertisement.Show(REWARDED_ID + "_" + platform);
#endif
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
            OnUnityAdsFinish(placementId, showResult.ToString());
        }
        
        
        public void OnUnityAdsFinish(string placementId, string showResult)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log(
                    $"{SDKSettingsModel.GetColorPrefixLog()} Ad finished. PlacementId: {placementId}, Result: {showResult}");
            adStatus = showResult;
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
            }, error =>
            {
                Debug.LogError(error);
                taskCompletionSource.SetResult(false);
            });
          
            return await taskCompletionSource.Task;
        }

        
#if UNITY_2020_1_OR_NEWER
        public void OnUnityAdsAdLoaded(string placementId)
        {
            OnUnityAdsReady(placementId);
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            OnUnityAdsDidError(message);
        }
        
        public void OnInitializationComplete()
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Unity Ads Initialization Complete");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {            
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.LogError($"{SDKSettingsModel.GetColorPrefixLog()} Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }
        
        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.LogError($"{SDKSettingsModel.GetColorPrefixLog()} Unity Ads Show Failure: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            OnUnityAdsDidStart(placementId);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Unity Ads Show Clicked: {placementId}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            OnUnityAdsFinish(placementId, showCompletionState.ToString());
        }
#endif
    }
}
