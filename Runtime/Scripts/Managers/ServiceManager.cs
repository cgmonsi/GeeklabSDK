using UnityEngine;
using UnityEngine.EventSystems;


namespace Kitrum.GeeklabSDK
{
    public class ServiceManager : MonoBehaviour
    {
        public static DeepLinkHandler DeepLinkHandler { get; private set; }
        public static DeviceInfoHandler DeviceInfoHandler { get; private set; }
        public static ClipboardHandler ClipboardHandler { get; private set; }
        public static TokenHandler TokenHandler { get; private set; }

        public static EngagementMetrics EngagementMetrics { get; private set; }
        public static PurchaseMetrics PurchaseMetrics { get; private set; }
        public static AdMetrics AdMetrics { get; private set; }

        public static MetricToggle MetricToggle { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            gameObject.AddComponent<WebRequestManager>();

            // Initialize services
            ClipboardHandler = gameObject.AddComponent<ClipboardHandler>();
            TokenHandler = gameObject.AddComponent<TokenHandler>();
            DeepLinkHandler = gameObject.AddComponent<DeepLinkHandler>();
            DeviceInfoHandler = gameObject.AddComponent<DeviceInfoHandler>();

            EngagementMetrics = gameObject.AddComponent<EngagementMetrics>();

            if (SDKSettingsModel.Instance.EnablePurchaseAnalytics)
                PurchaseMetrics = gameObject.AddComponent<PurchaseMetrics>();
            if (SDKSettingsModel.Instance.EnableAdAnalytics)
                AdMetrics = gameObject.AddComponent<AdMetrics>();

            MetricToggle = gameObject.AddComponent<MetricToggle>();
        }
    }
}