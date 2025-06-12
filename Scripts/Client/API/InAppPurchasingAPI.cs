using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Dawnshard.Menu;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class InAppPurchasingAPI : MonoBehaviour, IStoreListener
{
    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
    // We'll store the pending items until IAP initialization is complete.
    public List<ShopItemModel> IAPShopItems = new List<ShopItemModel>();
    private Action OnPurchaseSucceededAction;    
    private Action OnPurchaseFailedAction;
    


    public static InAppPurchasingAPI Instance { get; private set; }

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
        DontDestroyOnLoad(gameObject);
    }
    

    public void SetupIAP()
    {
        if (storeController != null)
        {
            Debug.Log("IAP already initialized.");
            // Optionally, update your UI here.
            return;
        }

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, ProductCatalog.LoadDefaultCatalog());
        UnityPurchasing.Initialize(this, builder);
    }

    
    public void PurchaseItem(string productId, Action onPurchaseSucceededAction, Action onPurchaseFailedAction)
    {
        if (storeController != null)
        {
            Product product = storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                this.OnPurchaseFailedAction = onPurchaseFailedAction;
                this.OnPurchaseSucceededAction = onPurchaseSucceededAction;
                Debug.Log("Initiating purchase for: " + productId);
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("Product " + productId + " not found or not available.");
            }
        }
        else
        {
            Debug.LogWarning("IAP is not initialized yet.");
        }
    }

    #region IStoreListener Implementation

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initialization Successful.");
        storeController = controller;
        extensionProvider = extensions;
        Debug.Log("After initialization IAP sees "+controller.products.all.Length+"product");
        foreach (var product in controller.products.all)
        {
            var shopItem = new ShopItemModel();
            shopItem.SKU = product.definition.id;
            shopItem.Description = product.metadata.localizedDescription;
            shopItem.ResourceObtained = product.definition.id.Split(".").Last().Replace("-","_");
            shopItem.Cost = (float)product.metadata.localizedPrice;
            shopItem.ResourceSpent = product.metadata.isoCurrencyCode;
#if UNITY_ANDROID
            var m = Regex.Match(product.metadata.localizedTitle, @"^([^(]*)");
            shopItem.Name = m.Groups[1].Value;
#else
            shopItem.Name = product.metadata.localizedTitle;
#endif
            Debug.Log($"Product info:{shopItem.Name} {shopItem.SKU} {shopItem.Description} {shopItem.Cost} {shopItem.ResourceSpent} {shopItem.ResourceObtained}");
            
            IAPShopItems.Add(shopItem);
        }
        // Now that IAP is initialized, spawn the UI for pending items if any.
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Initialization Failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("IAP Initialization Failed: " + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("Purchase successful for: " + args.purchasedProduct.definition.id);
        OnPurchaseSucceededAction?.Invoke();
        SendServerPurchaseReceipt(args);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("Purchase failed for: " + product.definition.storeSpecificId + " Reason: " + failureReason);
        OnPurchaseFailedAction?.Invoke();
    }
    #endregion
    
    #region ServerSideComunication
    
    private async void SendServerPurchaseReceipt(PurchaseEventArgs args)
    {
        bool isPurchaseGoneRight = false;
        Debug.Log(args.purchasedProduct.receipt);
#if UNITY_IOS
        isPurchaseGoneRight = await ShopAPI.ValidateRecipe(args.purchasedProduct.receipt);
#elif UNITY_ANDROID
        isPurchaseGoneRight = await ShopAPI.ValidateRecipe(args.purchasedProduct.receipt);
#endif
        if (!isPurchaseGoneRight)
        {
            ErrorPopup.Instance.Open();
            ErrorPopup.Instance.SetErrorText("Purchase failed.");
        }
    }
    
    #endregion
}
