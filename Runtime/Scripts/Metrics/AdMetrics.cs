using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Kitrum.GeeklabSDK
{
    [RequireComponent(typeof(AdMetrics))]
    public class AdMetrics : MonoBehaviour, IUnityAdsListener
    {
        private static float startWatchTime;
        private static float watchedSeconds;
        private static string gameId;
        private static string adStatus;
        private static string platform;
        private const string INTERSTITIAL_ID = "Interstitial";
        private const string BANNER_ID = "Banner";
        private const string REWARDED_ID = "Rewarded";
        private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

        public static AdMetrics Instance { get; private set; }
        private static bool IsInitialized { get; set; }


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

        public void ShowAd()
        {
            if (SDKSettingsModel.Instance.EnableAdAnalytics)
                Instance.StartCoroutine(WaitForAdInitialization());
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
            adStatus = "";
        }


        public void OnUnityAdsDidError(string message)
        {
            Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Log the error: {message}");
            adStatus = $"Error: {message}";
            startWatchTime = Time.time - startWatchTime;
            SendMetrics();
        }


        public void OnUnityAdsDidStart(string placementId)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Ad started. PlacementId: {placementId}");
            adStatus = "Started";
            watchedSeconds = 0.0f;
            startWatchTime = Time.time;
        }


        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log(
                    $"{SDKSettingsModel.GetColorPrefixLog()} Ad finished. PlacementId: {placementId}, Result: {showResult}");
            adStatus = showResult.ToString();
            startWatchTime = Time.time - startWatchTime;
            SendMetrics();
        }


        public static string SendMetrics(Dictionary<string, string> postData = null)
        {
            if (!SDKSettingsModel.Instance.SendStatistics) return null;

            postData ??= new Dictionary<string, string>
            {
                { "adId", gameId },
                { "adStatus", adStatus },
                { "watchedSeconds", watchedSeconds.ToString(CultureInfo.InvariantCulture) }
            };

            var json = JsonUtility.ToJson(postData);
            WebRequestManager.Instance.SendAdMetricsRequest(json, s =>
            {
                if (SDKSettingsModel.Instance.ShowDebugLog)
                    Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} {s}");
            }, s => { Debug.LogError($"{SDKSettingsModel.GetColorPrefixLog()} {s}"); });

            return json;
        }
    }
}
