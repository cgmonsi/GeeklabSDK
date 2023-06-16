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
        if (!IsConfigFullyEnabled(SDKSettingsModel.Instance.EnableAdAnalytics))
            return;
        
        AdMetrics.Instance.ShowAd();
    }
    
    /// <summary>
    /// Make a product purchase.
    /// </summary>
    /// <param name="value">Product ID</param>
    public static void BuyProduct(string value)
    { 
        if (!IsConfigFullyEnabled(SDKSettingsModel.Instance.EnablePurchaseAnalytics))
            return;
        
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
        if (!IsConfigFullyEnabled(SDKSettingsModel.Instance.SendStatistics))
            return;
        
        EngagementMetrics.SendMetrics();
    }
    
    /// <summary>
    /// Send purchase metrics to the server.
    /// </summary>
    public static void SendPurchaseMetrics()
    {
        if (!IsConfigFullyEnabled(SDKSettingsModel.Instance.SendStatistics))
            return;
        
        PurchaseMetrics.SendPurchaseMetrics();
    }
    
    /// <summary>
    /// Send advertisement metrics to the server.
    /// </summary>
    /// <param name="postData">Advertisement data to be sent</param>
    public static void SendAdMetrics(Dictionary<string, string> postData)
    {
        if (!IsConfigFullyEnabled(SDKSettingsModel.Instance.SendStatistics))
            return;
        
        AdMetrics.SendMetrics(postData);
    }
    
    /// <summary>
    /// Send device information to the server.
    /// </summary>
    public static void SendDeviceInformation()
    {
        if (!IsConfigFullyEnabled(SDKSettingsModel.Instance.SendStatistics))
            return;
        
        DeviceInfoHandler.SendDeviceInfo();
    }
    
    
    private static bool IsConfigFullyEnabled(bool value)
    { 
        if (SDKSettingsModel.Instance.IsSDKEnabled)
        {
            if (value)
            {
                return true;
            }
            else
            {
                Debug.LogWarning($"This option is disabled in the settings!\n" + 
                                 "Please enable it in the GeeklabSDK -> SDK Setting menu");
            }
        }
        else
        {
            Debug.LogWarning($"GeeklabSDK is disabled!\n" + 
                             "To work with the SDK, please enable it in the GeeklabSDK -> SDK Setting menu");
        }
        return false;
    }
}
