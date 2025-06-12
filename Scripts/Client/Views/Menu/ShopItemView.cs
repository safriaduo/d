using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Dawnshard.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button button;
    [SerializeField] private Button infoButton;
    [SerializeField] private ConfirmPopup confirmPopup;
    [SerializeField] private TextPopup infoPopup;
    
    private string ConfirmBuyText(string itemName, float cost, int amount, string resourceSpent) =>
        resourceSpent == Constants.Tethras?
        $"You'll buy {amount} {itemName} for {cost} {Constants.Tethras}. Proceed?":
        "You'll be redirected to the Dawnshard in app shop. Proceed?";
    private const string CONFIRM_BUY ="Confirm purchase";

    public ShopItemModel Item { get; private set; }

    private void Start()
    {
        infoButton.onClick.AddListener(OpenInfoPanel);
    }

    private void OpenInfoPanel()
    {
        infoPopup.SetBodyText(Item.Description);
        infoPopup.Open();
    }
    
    public void Initialize(ShopItemModel item, System.Action<ShopItemModel> onItemSelected, bool addConfirmPopup = true)
    {
        Item = item;

        button.onClick.RemoveAllListeners();
        if (addConfirmPopup)
        {
            confirmPopup.SetConfirmPopup(() => onItemSelected.Invoke(item), item.CanBePurchased(),
                ConfirmBuyText(item.Name, item.Cost, item.Amount, item.ResourceSpent), CONFIRM_BUY);
            button.onClick.AddListener(confirmPopup.Open);
        }
        else
        {
            button.onClick.AddListener(() => onItemSelected.Invoke(item));
        }


        if (itemTitle != null)
            itemTitle.text = item.Name;

        costText.text = item.Cost.ToString();
        itemImage.sprite = item.LoadIcon();
    }
}
