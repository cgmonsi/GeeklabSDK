using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
#if UNITY_PURCHASING_NEW
using Unity.Services.Core;
#endif


namespace Kitrum.GeeklabSDK
{
    public delegate void OnPurchaseMade(string productId);

    public class PurchaseMetrics : MonoBehaviour, IStoreListener
    {
        public static event OnPurchaseMade PurchaseMadeEvent;
        
        public static PurchaseMetrics Instance;
        // public static PurchaseMetrics Instance
        // {
        //     get
        //     {
        //         if (instance == null)
        //         {
        //             // Create new GameObject with WebRequestManager component
        //             var go = new GameObject(nameof(PurchaseMetrics));
        //             instance = go.AddComponent<PurchaseMetrics>();
        //             DontDestroyOnLoad(go);
        //         }
        //
        //         return instance;
        //     }
        // }

        private static bool isInitialized;
        private static string token;
        private static int valueOfPurchase;
        private static string idOfPurchasedItem;
        private static IStoreController controller;
        public bool? IsUnityPurchaseReady { get; set; }


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

#if UNITY_PURCHASING_NEW
        private async Task InitializeUnityServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to initialize Unity Services: {e.Message}");
            }
        }
#endif

        
#if UNITY_PURCHASING_NEW
        private static async void InitializePurchasing()
        {
            if (IsInitialized())
            {
                return;
            }

            await Instance.InitializeUnityServices();

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

#pragma warning disable CS0618
            UnityPurchasing.Initialize(Instance, builder);
#pragma warning restore CS0618
        }

#else
        
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

#pragma warning disable CS0618
            UnityPurchasing.Initialize(Instance, builder);
#pragma warning restore CS0618
        }
#endif

            
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
            IsUnityPurchaseReady = true;
            PurchaseMetrics.controller = controller;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            isInitialized = false;
            IsUnityPurchaseReady = false;
            Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Initialization purchase failed. Reason: " +
                             error);
            
#pragma warning disable CS4014
            var data = new List<object>
            {
                new { status = "failed", message = error.ToString() },
            };
            var postData = JsonConverter.ConvertToJson(data);
            SendPurchaseMetrics(postData);
#pragma warning restore CS4014
        }

        
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            OnInitializeFailed(error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            if (SDKSettingsModel.Instance.ShowDebugLog)
                Debug.Log(
                    $"{SDKSettingsModel.GetColorPrefixLog()} You've successfully bought the product: {e.purchasedProduct.definition.id}");
            PurchaseMadeEvent?.Invoke(e.purchasedProduct.definition.id);
            
#pragma warning disable CS4014
            var data = new List<object>
            {
                new { status = "success", id = e.purchasedProduct.definition.id },
            };
            var postData = JsonConverter.ConvertToJson(data);
            SendPurchaseMetrics(postData);
#pragma warning restore CS4014

            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, UnityEngine.Purchasing.PurchaseFailureReason failureReason)
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


        public static async Task<bool> SendPurchaseMetrics(string postData = null, bool isCustom = false)
        {
            if (!IsConfigFullyEnabled())
                return false;
            
            var taskCompletionSource = new TaskCompletionSource<bool>();

            WebRequestManager.Instance.SendPurchaseMetricsRequest(postData, isCustom, s =>
            {
                if (SDKSettingsModel.Instance.ShowDebugLog)
                    Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} {s}");
                
                taskCompletionSource.SetResult(true);
            }, error =>
            {
                Debug.LogError(error);
                taskCompletionSource.SetResult(false);
            });
            
            return await taskCompletionSource.Task;
        }
    }
}