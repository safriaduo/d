using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dawnshard.Menu;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using AssetDatabase = Dawnshard.Database.AssetDatabase;

public class LaunchArenaKeyPopup : PopupOneButton
{
    /*[SerializeField] private Image launchArenaKeyImage;
    [SerializeField] private ConfirmPopup confirmClaimPopup;
    private bool loadingNft = false;
    public override void Open()
    {
        base.Open();
    }

    protected override void Start()
    {
        base.Start();
        SetUpPopupButton("Enter the tournament", "Join", () =>
        {
            confirmClaimPopup.SetConfirmPopup(
                ConfirmLaunchArenaKeyBurn, true);
            confirmClaimPopup.Open();
        });
    }*/

    protected void Update()
    {
        // if (!TutorialStorageAPI.TutorialStoragePlayer.LaunchArenaKey && !string.IsNullOrEmpty(GameController.Instance.Address) &&
        //     TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered && !loadingNft)
        // {
        //     loadingNft = true;
        //     NFTLoadAPI.CheckForArenaKeyNft((keyId) =>
        //     {
        //         Open();
        //         launchArenaKeyImage.sprite = AssetDatabase.Instance.GetLaunchArenaKeyRecord(keyId).launchArenaKeySprite;
        //     });
        // }
    }

    /*private async void ConfirmLaunchArenaKeyBurn()
    {
        try
        {
            await NFTLoadAPI.LaunchArenaKeyMessageRPC();
            await TutorialStorageAPI.SaveTutorialStorage(TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered, true, TutorialStorageAPI.TutorialStoragePlayer.RewardForStep, TutorialStorageAPI.TutorialStoragePlayer.FirstDeckCreated, TutorialStorageAPI.TutorialStoragePlayer.AllQuestsCompleted);
            Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Close();
            throw;
        }
    }*/
}
