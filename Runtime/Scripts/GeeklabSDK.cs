using System.Collections.Generic;
using System.Threading.Tasks;
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

            if (!SDKSettingsModel.Instance.IsSDKEnabled || string.IsNullOrEmpty(SDKSettingsModel.Instance.Token))
                return null;
            
            var gameObject = new GameObject("GeeklabSDK");
            instance = gameObject.AddComponent<GeeklabSDK>();
            DontDestroyOnLoad(gameObject);
            return instance;
        }
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnAfterSceneLoadRuntimeMethod() 
        {
            if (SDKSettingsModel.Instance.IsSDKEnabled && !string.IsNullOrEmpty(SDKSettingsModel.Instance.Token))
                Initialize();
        }
        
        
        private void Awake()
        {
            if (SDKSettingsModel.Instance != null && SDKSettingsModel.Instance.ShowDebugLog)
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
            
            if (AdMetrics.Instance != null)
                AdMetrics.Instance.ShowAd();
        }
        
        /// <summary>
        /// Make a product purchase.
        /// </summary>
        /// <param name="value">Product ID</param>
        public static void BuyProduct(string value)
        { 
            if (SDKSettingsModel.Instance == null || !IsConfigFullyEnabled(SDKSettingsModel.Instance.EnablePurchaseAnalytics))
                return;
            
            PurchaseMetrics.BuyProduct(value);
        }
        
        /// <summary>
        /// Get Creative Token if any.
        /// </summary>
        /// <returns>Creative Token</returns>
        public static string GetCreativeToken() 
        {
            return TokenHandler.GetCreativeToken();
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
            return ClipboardHandler.GetText();
        }
        
        /// <summary>
        /// Enable or disable metrics collection.
        /// </summary>
        /// <param name="isEnabled">A flag indicating whether to enable metrics collection</param>
        public static void ToggleMetricsCollection(bool isEnabled)
        {
            if (SDKSettingsModel.Instance != null)
                SDKSettingsModel.Instance.SendStatistics = isEnabled;
        }
        
        /// <summary>
        /// Check if metrics collection is enabled.
        /// </summary>
        /// <returns>True if metrics collection is enabled, false otherwise</returns>
        public static bool GetIsMetricsCollection()
        {
            if (SDKSettingsModel.Instance != null)
                return SDKSettingsModel.Instance.SendStatistics;
            else
                return false;
        }

        /// <summary>
        /// Send User Metrics to the server.
        /// </summary>
        public static async Task<bool?> SendUserMetrics(object data)
        {
            if (SDKSettingsModel.Instance == null || !IsConfigFullyEnabled(SDKSettingsModel.Instance.SendStatistics))
                return false;
            
            var postData = JsonConverter.ConvertToJson(data);
            return await UserMetrics.SendMetrics(postData);
        }
        
        /// <summary>
        /// Send purchase metrics to the server
        /// </summary>
        /// <param name="data">Purchase data. Can be a string, list, or dictionary.</param>
        public static async Task<bool?> SendCustomPurchaseMetrics(object data)
        {
            if (SDKSettingsModel.Instance == null || !IsConfigFullyEnabled(SDKSettingsModel.Instance.SendStatistics))
                return false;

            var postData = JsonConverter.ConvertToJson(data);
            return await PurchaseMetrics.SendPurchaseMetrics(postData);
        }
        
        /// <summary>
        /// Send advertisement metrics to the server.
        /// </summary>
        /// <param name="postData">Advertisement data to be sent. Can be a string, list, or dictionary.</param>
        public static async Task<bool?> SendCustomAdMetrics(object data)
        {
            if (SDKSettingsModel.Instance == null || !IsConfigFullyEnabled(SDKSettingsModel.Instance.SendStatistics))
                return false;

            var postData = JsonConverter.ConvertToJson(data);
            return await AdMetrics.SendMetrics(postData, true);
        }


        
        private static bool IsConfigFullyEnabled(bool value)
        { 
            if (SDKSettingsModel.Instance != null && SDKSettingsModel.Instance.IsSDKEnabled 
                                                  && !string.IsNullOrEmpty(SDKSettingsModel.Instance.Token))
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