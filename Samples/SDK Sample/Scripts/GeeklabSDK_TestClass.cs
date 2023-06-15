using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;


public class GeeklabSDK_TestClass : MonoBehaviour {
    [FormerlySerializedAs("toggle")] [SerializeField]
    private Toggle toggleCollectServer;
    
    private void Start()
    {
        toggleCollectServer.isOn = GeeklabSDK.GetIsMetricsCollection();
    }
    
    public void SetPurchase()
    { 
        GeeklabSDK.BuyProduct("test_123");
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
        GeeklabSDK.ToggleMetricsCollection(!GeeklabSDK.GetIsMetricsCollection());
        Debug.Log($"CollectServerData: {GeeklabSDK.GetIsMetricsCollection()}");
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
