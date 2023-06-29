using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;
using Kitrum.GeeklabSDK;


public class GeeklabSDK_TestClass : MonoBehaviour {
    [FormerlySerializedAs("toggle")] [SerializeField]
    private Toggle toggleCollectServer;
    
    
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
    
    
    
    public async void SendPurchaseMetrics()
    {
        var postData = new List<object>
        {
            new { id = "item1", price = 10 },
            new { id = "item2", price = 20 },
        };
        await GeeklabSDK.SendCustomPurchaseMetrics(postData);
    }
    
    public async void SendAdMetrics()
    { 
        var postData = new List<object>
        {
            new { id = "Test", status = "Showed" },
        };
        await GeeklabSDK.SendCustomAdMetrics(postData);
    }
    
    public async void SendDeviceInformation()
    { 
        await GeeklabSDK.SendUserMetrics();
    }
}
