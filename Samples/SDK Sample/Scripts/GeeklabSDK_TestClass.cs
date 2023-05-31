using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GeeklabSDK_TestClass : MonoBehaviour {
    [FormerlySerializedAs("toggle")] [SerializeField]
    private Toggle toggleCollectServer;
    
    private void Start()
    {
        var testListItems = new Dictionary<string, ProductType> {
            { "test", ProductType.Subscription }
        };
        GeeklabSDK.Instance.InitializePurchasing(testListItems);

        toggleCollectServer.isOn = SDKInfoModel.CollectServerData;
    }
    
    public void SetPurchase()
    { 
        GeeklabSDK.BuyProduct("test");
    }
    
    
    public void ShowAd()
    { 
        GeeklabSDK.ShowAd();
    }

    
    public void GetDeepLink()
    {
        Debug.Log(GeeklabSDK.GetDeepLink());
    }
    
    
    public void ShowUserClipboard()
    { 
        Debug.Log(GeeklabSDK.GetClipboard());
    }
    
    
    public void SwitchCollectServerData() {
        GeeklabSDK.ToggleMetricsCollection(!SDKInfoModel.CollectServerData);
        Debug.Log($"CollectServerData: {SDKInfoModel.CollectServerData}");
    }
    
    
    public void SendEngagementMetrics()
    { 
        GeeklabSDK.SendEngagementMetrics();
    }
    
    public void SendPurchaseMetrics() 
    {
        GeeklabSDK.SendPurchaseMetrics();
    }
    
    public void SendAdMetrics()
    { 
        var postData = new Dictionary<string, string> {
            { "adId", "123" },
            { "adStatus", "Finished" },
            { "watchedSeconds", "5" }
        };
        GeeklabSDK.SendAdMetrics(postData);
    }
    
    public void SendDeviceInformation()
    { 
        GeeklabSDK.SendDeviceInformation();
    }
}
