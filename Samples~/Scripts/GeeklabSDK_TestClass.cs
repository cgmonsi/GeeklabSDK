using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class GeeklabSDK_TestClass : MonoBehaviour {
    [FormerlySerializedAs("toggle")] [SerializeField]
    private Toggle toggleCollectServer;
    
    [SerializeField]
    private TextMeshProUGUI Clipboard_TextMeshPro;
    [SerializeField]
    private TextMeshProUGUI DeepLink_TextMeshPro;
    [SerializeField]
    private TextMeshProUGUI CreativeToken_TextMeshPro;
    
    
    public void SetPurchase()
    { 
        GeeklabSDK.BuyProduct("test_123");
    }
    
    
    public void ShowAd()
    { 
        GeeklabSDK.ShowAd();
    }

    
    public void GetCreativeToken()
    {
        if (CreativeToken_TextMeshPro != null)
            CreativeToken_TextMeshPro.SetText(GeeklabSDK.GetCreativeToken());
        Debug.Log(GeeklabSDK.GetCreativeToken());
    }
    
    
    public void GetDeepLink()
    {
        if (DeepLink_TextMeshPro != null)
            DeepLink_TextMeshPro.SetText(GeeklabSDK.GetDeepLink());
        Debug.Log(GeeklabSDK.GetDeepLink());
    }
    
    
    public void ShowUserClipboard()
    { 
        if (Clipboard_TextMeshPro != null)
            Clipboard_TextMeshPro.SetText(GeeklabSDK.GetClipboard());
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
