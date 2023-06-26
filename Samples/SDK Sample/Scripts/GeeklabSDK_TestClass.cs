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
    
    
    
    public void SendPurchaseMetrics()
    {
        var postData = new List<object>
        {
            new { id = "item1", price = 10 },
            new { id = "item2", price = 20 },
        };
        GeeklabSDK.SendCustomPurchaseMetrics(postData);
    }
    
    public void SendAdMetrics()
    { 
        var postData = new List<object>
        {
            new { id = "Test", status = "Showed" },
        };
        GeeklabSDK.SendCustomAdMetrics(postData);
    }
    
    public void SendDeviceInformation()
    { 
        GeeklabSDK.SendUserMetrics();
    }
}
