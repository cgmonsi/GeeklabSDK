using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


namespace Kitrum.GeeklabSDK
{
    public delegate void OnPurchaseMade(string productId);

    public class PurchaseMetrics : MonoBehaviour, IStoreListener
    {
        public static event OnPurchaseMade PurchaseMadeEvent;

        private static PurchaseMetrics Instance { get; set; }

        private static bool isInitialized;
        private static string token;
        private static int valueOfPurchase;
        private static string idOfPurchasedItem;
        private static IStoreController controller;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            InitializePurchasing();
        }


        private static void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            var purchasableItems = new Dictionary<string, ProductType> { };
            foreach (var purchasableItem in SDKSettingsModel.Instance.purchasableItems)
            {
                purchasableItems.Add(purchasableItem.name, purchasableItem.type);
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var item in purchasableItems)
            {
                builder.AddProduct(item.Key, item.Value);
            }

            UnityPurchasing.Initialize(Instance, builder);
        }


        private static bool IsInitialized()
        {
            if (SDKSettingsModel.Instance.EnablePurchaseAnalytics)
                return controller != null && isInitialized;
            else
                return false;

        }
        
        private static bool IsConfigFullyEnabled()
        { 
            if (SDKSettingsModel.Instance.IsSDKEnabled)
            {
                if (SDKSettingsModel.Instance.SendStatistics)
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Collection of information is disabled!\n" + 
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


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            isInitialized = true;
            PurchaseMetrics.controller = controller;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Initialization purchase failed. Reason: " +
                             error);
            SendPurchaseMetrics();
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log(
                    $"{SDKSettingsModel.GetColorPrefixLog()} You've successfully bought the product: {e.purchasedProduct.definition.id}");
            PurchaseMadeEvent?.Invoke(e.purchasedProduct.definition.id);
            SendPurchaseMetrics();
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Purchase failed: {failureReason}");
        }

        public static void BuyProduct(string productId)
        {
            if (!IsConfigFullyEnabled())
                return;
                
            if (IsInitialized())
            {
                var product = controller.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Purchasing product asychronously: " +
                                  product.definition.id);
                    controller.InitiatePurchase(product);
                }
                else
                {
                    Debug.LogWarning(
                        $"{SDKSettingsModel.GetColorPrefixLog()} BuyProduct: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.LogWarning(
                    $"{SDKSettingsModel.GetColorPrefixLog()} BuyProduct: FAIL. {productId} - Not initialized");

            }
        }


        public static void Initialize(string token)
        {
            PurchaseMetrics.token = token;
        }


        public void UpdateMetrics(int valueOfPurchase, string idOfPurchasedItem)
        {
            PurchaseMetrics.valueOfPurchase = valueOfPurchase;
            PurchaseMetrics.idOfPurchasedItem = idOfPurchasedItem;

            SendPurchaseMetrics();
        }


        public static void SendPurchaseMetrics()
        {
            if (!IsConfigFullyEnabled())
                return;
            
            var json = "{\"token\":\"" + token + "\"," +
                       "\"value_of_purchase\":" + valueOfPurchase + "," +
                       "\"id_of_purchased_item\":\"" + idOfPurchasedItem + "\"}";

            WebRequestManager.Instance.SendPurchaseMetricsRequest(json, s =>
            {
                if (SDKSettingsModel.Instance.ShowDebugLog)
                    Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} {s}");
            }, s => { Debug.LogError($"{SDKSettingsModel.GetColorPrefixLog()} {s}"); });
        }
    }
}