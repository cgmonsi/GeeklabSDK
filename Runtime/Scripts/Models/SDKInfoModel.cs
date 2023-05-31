using UnityEngine;

public static class SDKInfoModel {
    [FieldGroup("Ad Settings")]
    public static string GameIdIOS { get; set; } = "5288124"; //For Test: 5288124
    
    [FieldGroup("Ad Settings")]
    public static string GameIdAndroid { get; set; } = "5288125"; //For Test: 5288125
    
    [FieldGroup("Ad Settings")]
    public static bool AdTestMode { get; set; } = true;
    
    [FieldGroup("Main Settings")]
    public static bool CollectServerData { get; set; } = true;
}