using System.Collections.Generic;
using Kitrum.GeeklabSDK;
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
    
    /// <summary>
    /// Show an advertisement.
    /// </summary>
    public static void ShowAd()
    {
        AdMetrics.Instance.ShowAd();
    }
    
    /// <summary>
    /// Make a product purchase.
    /// </summary>
    /// <param name="value">Product ID</param>
    public static void BuyProduct(string value)
    { 
        PurchaseMetrics.BuyProduct(value);
    }
    
    /// <summary>
    /// Get deep link URL if any.
    /// </summary>
    /// <returns>Deep link URL</returns>
    public static string GetDeepLink() 
    {
        return DeepLinkHandler.GetDeepLink();
    }
    
    /// <summary>
    /// Get the current clipboard content.
    /// </summary>
    /// <returns>Clipboard content</returns>
    public static string GetClipboard()
    { 
        return ClipboardHandler.ReadClipboard();
    }
    
    /// <summary>
    /// Enable or disable metrics collection.
    /// </summary>
    /// <param name="isEnabled">A flag indicating whether to enable metrics collection</param>
    public static void ToggleMetricsCollection(bool isEnabled)
    {
        SDKSettingsModel.Instance.SendStatistics = isEnabled;
    }
    
    /// <summary>
    /// Check if metrics collection is enabled.
    /// </summary>
    /// <returns>True if metrics collection is enabled, false otherwise</returns>
    public static bool GetIsMetricsCollection()
    {
        return SDKSettingsModel.Instance.SendStatistics;
    }

    /// <summary>
    /// Send engagement metrics to the server.
    /// </summary>
    public static void SendEngagementMetrics()
    { 
        EngagementMetrics.SendMetrics();
    }
    
    /// <summary>
    /// Send purchase metrics to the server.
    /// </summary>
    public static void SendPurchaseMetrics() 
    {
        PurchaseMetrics.SendPurchaseMetrics();
    }
    
    /// <summary>
    /// Send advertisement metrics to the server.
    /// </summary>
    /// <param name="postData">Advertisement data to be sent</param>
    public static void SendAdMetrics(Dictionary<string, string> postData)
    { 
        AdMetrics.SendMetrics(postData);
    }
    
    /// <summary>
    /// Send device information to the server.
    /// </summary>
    public static void SendDeviceInformation()
    { 
        DeviceInfoHandler.SendDeviceInfo();
    }
}
