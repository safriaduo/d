using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using AutoLetterbox;
using Dawnshard.Menu;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ShopState : AState
{
    [SerializeField] private ShopUIView shopUI;
    [SerializeField] private PopupOneButton popupOneButton;
    [SerializeField] private PlayerStatsPopup playerStatsPopup;
    [SerializeField] private ConfirmPopup goToStoreConfirmPopup;
    [SerializeField] private Popup receivedNotificationPopup;
    private const string PACKS_MENU_OPTION = "Tethras Shop";
    private const string WEB_STORE_MENU_OPTION = "Web Store";
    private const string APP_STORE_MENU_OPTION = "Buy Packs";
    private const string PURCHASE_FAILED_TITLE= "Purchase Failed";
    private const string PURCHASE_SUCCEEDED_TITLE= "Purchase Succeeded";


    private const string OPEN_PACK_TEXT = "Open it!";
    private const string GO_BACK_TEXT = "Go back!";
    private const string POPUP_ERROR_MESSAGE = "Something went wrong!";
    private string TextBody(int amount, string resourceObtained) => $"You bought {amount} {resourceObtained}!";
    
    /// <summary>
    /// Item has been selected on the UI
    /// </summary>
    private async void OnItemSelected(ShopItemModel item)
    {
        LoadingPopup.Instance.Open();
        bool isPurchaseConfirmed = await ShopAPI.BuyShopItem(item);
        LoadingPopup.Instance.Close();
        if (isPurchaseConfirmed)
        {
            LoadUserData();

            var buttonText = OPEN_PACK_TEXT;
            popupOneButton.SetUpPopupButton(
                //TODO: check if the purchase has gone well
                TextBody(item.Amount, item.ResourceObtained.Replace("_"," ")), buttonText, () =>
                {
                    popupOneButton.Close();
                    if (item.ResourceObtained == Constants.StandardPack)
                    {
                        MenuManager.Instance.GoToState(Constants.RewardsState, true);
                    }
                    else
                        popupOneButton.Close();
                }, titleText:PURCHASE_SUCCEEDED_TITLE);
        }
        else
        {
            popupOneButton.SetUpPopupButton(
                POPUP_ERROR_MESSAGE, onClick:() => { popupOneButton.Close(); }, titleText:PURCHASE_FAILED_TITLE, buttonText:GO_BACK_TEXT);
        }
        popupOneButton.Open();
        // if (item.CanBePurchased())
        //     popupOneButton.SetUpPopupButton(, 
        //         "Buy", 
        //         () => OnPurchaseConfirmed(item));
        // else
        //     popupOneButton.SetUpPopupButton(
        //         $"You don't have enough {Constants.Tethras}.\n" +
        //         $"Your balance: {GameController.Instance.Wallet[Constants.Tethras]} {Constants.Tethras}\n" +
        //         $"Cost: {item.Cost} {Constants.Tethras}",
        //         "Close",popupOneButton.Close);
    }

    /// <summary>
    /// Buys the selected item
    /// </summary>

    public override void Enter(AState from)
    {
        gameObject.SetActive(true);
        MenuManager.Instance.SetOptionsBackground(optionBackground);
        MenuManager.Instance.SetOptions(GetOptions(), PACKS_MENU_OPTION);
        LoadUserData();
    }

    private async void LoadUserData()
    {
        try
        {
            await GameController.Instance.RefreshAssets();
            var currencies = GameController.Instance.Wallet;
            playerStatsPopup.Initialize(currencies.ContainsKey(Constants.Debristar)?
                    currencies[Constants.Debristar]:0, 
                currencies.ContainsKey(Constants.Tethras)?
                    currencies[Constants.Tethras]:0);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }        
    }

    private Dictionary<string, Action> GetOptions()
    {
        return new Dictionary<string, Action>()
        {
            { PACKS_MENU_OPTION, ShowPacks },
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            { APP_STORE_MENU_OPTION, GoToAppStore }
#elif UNITY_WEBGL
            { WEB_STORE_MENU_OPTION, GoToStore }
#endif
        };
    }

    private void GoToAppStore()
    {
        shopUI.DestroySpawnedItems();
        
        InAppPurchasingAPI.Instance.IAPShopItems.ForEach(iapItem => Debug.Log("IapShopItems: "+iapItem.Name+" costs "+iapItem.Cost));
        
        shopUI.SpawnShopItems(InAppPurchasingAPI.Instance.IAPShopItems,
            (shopItemModel) =>
            {
                LoadingPopup.Instance.Open();
                InAppPurchasingAPI.Instance.PurchaseItem(shopItemModel.SKU, () => PurchaseConfirmed(shopItemModel), PurchaseFailed);
            });
    }
    

    private void PurchaseConfirmed (ShopItemModel item)
    {
        LoadUserData();

        var buttonText = OPEN_PACK_TEXT;
        popupOneButton.SetUpPopupButton(
            //TODO: check if the purchase has gone well
            TextBody(1, item.ResourceObtained.Replace("_"," ")), buttonText, () =>
            {
                popupOneButton.Close();
                if (item.ResourceObtained == Constants.StandardPack 
                    || Constants.BlazingPack == item.ResourceObtained 
                    || Constants.ShinyPack == item.ResourceObtained 
                    || Constants.WorldPack == item.ResourceObtained)
                {
                    MenuManager.Instance.GoToState(Constants.RewardsState, true);
                }
                else
                    popupOneButton.Close();
            }, titleText: PURCHASE_SUCCEEDED_TITLE);
        LoadingPopup.Instance.Close();
        popupOneButton.Open();
    }
    private void PurchaseFailed ()
    {
        popupOneButton.SetUpPopupButton(
            POPUP_ERROR_MESSAGE, onClick:() => { popupOneButton.Close(); }, buttonText: GO_BACK_TEXT, titleText: PURCHASE_FAILED_TITLE);
        LoadingPopup.Instance.Close();
        popupOneButton.Open();
    }

    private async void OnAppPurchaseItemSelected(ShopItemModel item)
    {
        
        
        // if (item.CanBePurchased())
        //     popupOneButton.SetUpPopupButton(, 
        //         "Buy", 
        //         () => OnPurchaseConfirmed(item));
        // else
        //     popupOneButton.SetUpPopupButton(
        //         $"You don't have enough {Constants.Tethras}.\n" +
        //         $"Your balance: {GameController.Instance.Wallet[Constants.Tethras]} {Constants.Tethras}\n" +
        //         $"Cost: {item.Cost} {Constants.Tethras}",
        //         "Close",popupOneButton.Close);
    }
    
    private void GoToStore()
    {
        shopUI.DestroySpawnedItems();
        goToStoreConfirmPopup.SetConfirmPopup(()=>Application.OpenURL(Constants.WEB_STORE_URL),true, Constants.WEB_STORE_BODY, Constants.WEB_STOORE_TITLE);
        goToStoreConfirmPopup.Open();
    }

    private void ShowPacks()
    {
        shopUI.DestroySpawnedItems();

        shopUI.SpawnShopItems(ShopAPI.ItemList.Items, OnItemSelected);
    }

    public override void Exit(AState to)
    {
        gameObject.SetActive(false);
        popupOneButton.Close();
    }

    public override string GetName() => Constants.ShopState;
}
