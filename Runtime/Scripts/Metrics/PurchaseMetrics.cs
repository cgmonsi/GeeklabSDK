using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public delegate void OnPurchaseMade(string productId);

public class PurchaseMetrics : MonoBehaviour, IStoreListener
{
    public static event OnPurchaseMade PurchaseMadeEvent;

    public static PurchaseMetrics Instance { get; private set; }

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
    }
    
    
    public void InitializePurchasing(Dictionary<string, ProductType> listItems) 
    {
        if (IsInitialized()) 
        {
            return;
        }
        
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var item in listItems) {
            builder.AddProduct(item.Key, item.Value);
        }
        UnityPurchasing.Initialize(this, builder);
    }
    
    
    private static bool IsInitialized()
    {
        return controller != null;
    }
    
    
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        PurchaseMetrics.controller = controller;
    }

    
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Initialization failed. Reason: " + error);
        SendPurchaseMetrics();
    }

    
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) 
    {
        Debug.Log($"You've successfully bought the product: {e.purchasedProduct.definition.id}");
        PurchaseMadeEvent?.Invoke(e.purchasedProduct.definition.id);
        SendPurchaseMetrics();
        return PurchaseProcessingResult.Complete;
    }
    

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed: {failureReason}");
    }
    
    public static void BuyProduct(string productId)
    {
        if (IsInitialized())
        {
            var product = controller.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log("Purchasing product asychronously: " + product.definition.id);
                controller.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProduct: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log($"BuyProduct: FAIL. {productId} - Not initialized");

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
        if (!SDKInfoModel.CollectServerData) return;

        var json = "{\"token\":\"" + token + "\"," +
                   "\"value_of_purchase\":" + valueOfPurchase + "," +
                   "\"id_of_purchased_item\":\"" + idOfPurchasedItem + "\"}";
        
        WebRequestManager.Instance.SendPurchaseMetricsRequest(json, Debug.Log, Debug.LogError);
    }
}