using System.Collections.Generic;
using UnityEngine;


namespace Kitrum.GeeklabSDK
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SDKSettings", menuName = "Geeklab/SDK Settings", order = 1)]
    public class SDKSettingsModel : ScriptableObject
    {
        private static SDKSettingsModel _instance;

        public static SDKSettingsModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SDKSettingsModel>("SDKSettings");

                    // Create new instance if not found
                    if (_instance == null)
                    {
                        _instance = CreateInstance<SDKSettingsModel>();

                        // Save the newly created instance as an asset in the project
#if UNITY_EDITOR
                        UnityEditor.AssetDatabase.CreateAsset(_instance, "Assets/Resources/SDKSettings.asset");
                        UnityEditor.AssetDatabase.SaveAssets();
#endif
                        if (Instance.ShowDebugLog)
                            Debug.Log("New SDKSettings instance created.");
                    }
                }

                return _instance;
            }
        }
        
        public static bool IsTokenVerified = false;


        [HideInInspector]
        [HideInFieldGroup("Main Settings")]
        [FieldGroup("Main Settings")]
        public string Token;


        [FieldGroup("Main Settings")] [Header("Main Settings")]
        public bool IsSDKEnabled = false;
        
        [DisableIfSDKDisabled] [FieldGroup("Main Settings")]
        public bool CheckClipboardForToken = true;

        [DisableIfSDKDisabled] [FieldGroup("Main Settings")]
        public bool SendStatistics = true;

        [DisableIfSDKDisabled] [FieldGroup("Main Settings")]
        public bool ShowDebugLog = true;
        
        
        [DisableIfSDKDisabled] [FieldGroup("Auto Analytics")] [Header("Auto Analytics")]
        public bool EnableAdAnalytics = false;

        [DisableIfSDKDisabled] [FieldGroup("Auto Analytics")]
        public bool EnablePurchaseAnalytics = false;


        [DisableIfSDKDisabled] [DisableIfAdAnalyticsDisabled] [FieldGroup("Ad Settings")] [Header("Ad Settings")]
        public string GameIdIOS = "5288124"; //For Test: 5288124

        [DisableIfSDKDisabled] [DisableIfAdAnalyticsDisabled] [FieldGroup("Ad Settings")]
        public string GameIdAndroid = "5288125"; //For Test: 5288125

        [DisableIfSDKDisabled] [DisableIfAdAnalyticsDisabled] [FieldGroup("Ad Settings")]
        public bool AdTestMode = true;


        [DisableIfSDKDisabled]
        [DisableIfPurchaseAnalyticsDisabled]
        [Header("Purchase Settings:")]
        public List<PurchasableItemModel> purchasableItems = new List<PurchasableItemModel>();


        private static string PrefixDebugLog = $"<color=#668cff>GeeklabSDK</color> <color=#666666>=></color>";

        public static string GetColorPrefixLog()
        {
            return PrefixDebugLog;
        }
    }
}