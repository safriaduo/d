using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Dawnshard.Database;
using Dawnshard.Menu;
using Dawnshard.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button button;
    [SerializeField] private Button infoButton;
    [SerializeField] private ConfirmPopup confirmPopup;
    [SerializeField] private TextPopup infoPopup;
    [SerializeField] private TogglePopup togglePopup;
    
    private string ConfirmOpenText(string itemName) =>
        $"You'll open a {itemName}. Proceed?";
    private string ConfirmOpenWorldPackText(string world) =>
        $"You'll open a {world} pack. Proceed?";
    
    private const string CONFIRM_OPEN ="Confirm open";

    public RewardModel Item { get; private set; }

    private void Start()
    {
        infoButton.onClick.AddListener(OpenInfoPanel);
    }

    private void OpenInfoPanel()
    {
        infoPopup.SetBodyText($"This is a {Item.RewardId.Replace("_"," ")}!\n You have {Item.Amount} of this pack left!");
        infoPopup.Open();
    }
    
    public void Initialize(RewardModel item, Action<string, string> onItemSelected)
    {
        Item = item;

        button.onClick.RemoveAllListeners();
        if(item.RewardId!=Constants.WorldPack)
        {
            confirmPopup.SetConfirmPopup(() => onItemSelected.Invoke(Item.RewardId, ""), Item.Amount > 0,
                ConfirmOpenText(Item.RewardId.Replace("_", " ")), CONFIRM_OPEN);
            button.onClick.AddListener(confirmPopup.Open);
        }        
        else
        {
            void SetUpConfirmPopup(){
                confirmPopup.SetConfirmPopup(() => onItemSelected.Invoke(Item.RewardId, togglePopup.GetActiveToggle()), Item.Amount>0, ConfirmOpenWorldPackText(togglePopup.GetActiveToggle()),CONFIRM_OPEN);
                confirmPopup.Open();
            }
            button.onClick.AddListener(togglePopup.Open);
            togglePopup.SetUpPopupButton("Choose world", "Confirm", SetUpConfirmPopup);
        }
        if (itemTitle != null)
            itemTitle.text = Item.RewardId.Replace("_"," ");

        quantityText.text = item.Amount.ToString();
        itemImage.sprite = AssetDatabase.Instance.GetRewardRecord(item.RewardId).rewardIcon;
    }
}