using System.Collections.Generic;
using UnityEngine.Purchasing;
using UnityEngine;

public class GeeklabSDK : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Show Service in Hierarchy")]
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
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod() 
    {
        Initialize();
    }
    

    private void Awake()
    {
        SDKInfoChecker.CheckSDKInfoModel();

        var serviceLocator = new GameObject("ServiceLocator");
        serviceLocator.AddComponent<ServiceLocator>();
        serviceLocator.hideFlags = showServiceInHierarchy ? serviceLocator.hideFlags : HideFlags.HideInHierarchy;
    }
    
    
    public void InitializePurchasing(Dictionary<string, ProductType> listItems)
    {
        Initialize();
        PurchaseMetrics.Instance.InitializePurchasing(listItems);
    }
    

    public static void ShowAd()
    {
        Initialize();
        AdMetrics.Instance.ShowAd();
    }
    
    public static void BuyProduct(string value)
    { 
        Initialize();
        PurchaseMetrics.BuyProduct(value);
    }
    
    
    public static string GetDeepLink() 
    {
        Initialize();
        return DeepLinkHandler.GetDeepLink();
    }
    
    public static string GetClipboard()
    { 
        Initialize();
        return ClipboardHandler.ReadClipboard();
    }
    
    
    public static bool ToggleMetricsCollection(bool isEnabled)
    {
        Initialize();
        SDKInfoModel.CollectServerData = isEnabled;
        return SDKInfoModel.CollectServerData;
    }
    
    
    public static void SendEngagementMetrics()
    { 
        Initialize();
        EngagementMetrics.SendMetrics();
    }
    
    public static void SendPurchaseMetrics() 
    {
        Initialize();
        PurchaseMetrics.SendPurchaseMetrics();
    }
    
    public static void SendAdMetrics(Dictionary<string, string> postData)
    { 
        Initialize();
        AdMetrics.SendMetrics(postData);
    }
    
    public static void SendDeviceInformation()
    { 
        Initialize();
        DeviceInfoHandler.SendDeviceInfo();
    }
}
