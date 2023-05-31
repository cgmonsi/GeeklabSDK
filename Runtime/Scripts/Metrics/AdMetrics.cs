using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private static AdMetrics instance;

    private static bool IsInitialized { get; set; }

    public static AdMetrics Instance { get => instance; set => instance = value; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        Advertisement.AddListener(this);

#if UNITY_ANDROID
        Advertisement.Initialize(SDKInfoModel.GameIdAndroid, SDKInfoModel.AdTestMode);
        gameId = SDKInfoModel.GameIdAndroid;
        platform = "Android";
#elif UNITY_IOS
        Advertisement.Initialize(SDKInfoModel.GameIdIOS, SDKInfoModel.AdTestMode);
        gameId = SDKInfoModel.GameIdIOS;
        platform = "iOS";
#else
        Advertisement.Initialize(SDKInfoModel.GameIdAndroid, SDKInfoModel.AdTestMode);
        gameId = SDKInfoModel.GameIdAndroid;
        Debug.LogWarning("Unsupported platform to AD");
        platform = "Android";
#endif
        
        if (FindObjectOfType<EventSystem>() != null) {
            Destroy(FindObjectOfType<EventSystem>().gameObject);
        }
    }

    public void ShowAd()
    {
        instance.StartCoroutine(WaitForAdInitialization());
    }
    
    private IEnumerator WaitForAdInitialization()
    {
        while (!Advertisement.IsReady(REWARDED_ID + "_" + platform))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Show(REWARDED_ID + "_" + platform);
    }
    

    public void OnUnityAdsReady(string placementId) {
        Debug.Log("OnUnityAdsReady placementId = " + placementId);
        IsInitialized = true;
        adStatus = "";
    }

    
    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Log the error: " + message);
        adStatus = $"Error: {message}";
        startWatchTime = Time.time - startWatchTime;
        SendMetrics();
    }

    
    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ad started. PlacementId: " + placementId);
        adStatus = "Started";
        watchedSeconds = 0.0f;
        startWatchTime = Time.time;
    }

    
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        Debug.Log("Ad finished. PlacementId: " + placementId + ", Result: " + showResult);
        adStatus = showResult.ToString();
        startWatchTime = Time.time - startWatchTime;
        SendMetrics();
    }


    public static string SendMetrics(Dictionary<string, string> postData = null)
    {
        if (!SDKInfoModel.CollectServerData) return null;

        postData ??= new Dictionary<string, string> {
            { "adId", gameId },
            { "adStatus", adStatus },
            { "watchedSeconds", watchedSeconds.ToString(CultureInfo.InvariantCulture) }
        };
       
        var json = JsonUtility.ToJson(postData);
        WebRequestManager.Instance.SendAdMetricsRequest(json, Debug.Log, Debug.LogError);

        return json;
    }
}
