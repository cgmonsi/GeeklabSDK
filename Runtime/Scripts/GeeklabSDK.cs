using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine;

public class GeeklabSDK : MonoBehaviour
{
    private bool showServiceInHierarchy;

    private static GeeklabSDK instance;
    public static GeeklabSDK Instance => Initialize();

    private static GeeklabSDK Initialize()
    {
        if (instance != null) {
            return instance;
        }

        var gameObject = new GameObject("GeeklabSDK");
        instance = gameObject.AddComponent<GeeklabSDK>();
        DontDestroyOnLoad(gameObject);
        return instance;
    }
    
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoadRuntimeMethod() 
    {
        if (SDKSettingsModel.Instance.IsSDKEnabled)
            Initialize();
    }
    
    
    private void Awake()
    {
        if (SDKSettingsModel.Instance.ShowDebugLog)
            Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} SDK Initialized!");
        
        //------Init All Managers and hide them------//
        var serviceLocator = new GameObject("ServiceLocator");
        serviceLocator.AddComponent<ServiceManager>();
        serviceLocator.hideFlags = showServiceInHierarchy ? serviceLocator.hideFlags : HideFlags.HideInHierarchy;
        gameObject.hideFlags = showServiceInHierarchy ? gameObject.hideFlags : HideFlags.HideInHierarchy;
    }
    

    public static void ShowAd()
    {
        AdMetrics.Instance.ShowAd();
    }
    
    public static void BuyProduct(string value)
    { 
        PurchaseMetrics.BuyProduct(value);
    }
    
    
    public static string GetDeepLink() 
    {
        return DeepLinkHandler.GetDeepLink();
    }
    
    public static string GetClipboard()
    { 
        return ClipboardHandler.ReadClipboard();
    }
    
    
    public static bool ToggleMetricsCollection(bool isEnabled)
    {
        SDKSettingsModel.Instance.SendStatistics = isEnabled;
        return SDKSettingsModel.Instance.SendStatistics;
    }
    
    
    public static void SendEngagementMetrics()
    { 
        EngagementMetrics.SendMetrics();
    }
    
    public static void SendPurchaseMetrics() 
    {
        PurchaseMetrics.SendPurchaseMetrics();
    }
    
    public static void SendAdMetrics(Dictionary<string, string> postData)
    { 
        AdMetrics.SendMetrics(postData);
    }
    
    public static void SendDeviceInformation()
    { 
        DeviceInfoHandler.SendDeviceInfo();
    }
}
