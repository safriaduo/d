using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialMenuManager : MonoBehaviour
{
    [SerializeField] private NotificationView createDeckNotification;
    [SerializeField] private NotificationView questNotification;
    [SerializeField] private CollectionState collectionState;
    [SerializeField] private GameObject tutorialDeckBuilding;
    [SerializeField] private PlayState playState;

    private void Start()
    {
        if(TutorialStorageAPI.TutorialStoragePlayer.AllQuestsCompleted)
            gameObject.SetActive(false);
        if(GameController.Instance.UserMetadata.CompletedTutorials>4)
            HideQuestNotification();
    }

    void Update()
    {
        if (!TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered)
            return;
        if (GameController.Instance.UserMetadata.CompletedTutorials == 2 && TutorialStorageAPI.TutorialStoragePlayer.RewardForStep==1 && !TutorialStorageAPI.TutorialStoragePlayer.FirstDeckCreated)
        {
            ShowCreateDeckNotification();
            return;
        }

        if (tutorialDeckBuilding.activeSelf && TutorialStorageAPI.TutorialStoragePlayer.FirstDeckCreated)
            HideDeckBuildingTutorial();

        if(GameController.Instance.Decks==null || GameController.Instance.UserMetadata==null)
            return;
        
        if (GameController.Instance.Decks.Count>0 && !TutorialStorageAPI.TutorialStoragePlayer.AllQuestsCompleted)
        {
            ShowQuestNotification();
        }
    }

    private async void HideQuestNotification()
    {
        questNotification.HideNotification();
        playState.SetNotifications(null);
        await TutorialStorageAPI.SaveTutorialStorage(TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered, TutorialStorageAPI.TutorialStoragePlayer.LaunchArenaKey, TutorialStorageAPI.TutorialStoragePlayer.RewardForStep, TutorialStorageAPI.TutorialStoragePlayer.FirstDeckCreated, true);
        gameObject.SetActive(false);
    }

    private void ShowQuestNotification()
    {
        createDeckNotification.HideNotification();
        questNotification.ShowNotification();
        collectionState.SetNotifications(null);
        playState.SetNotifications(new Dictionary<string, bool>(){{Constants.PlayStateQuest, true}});
    }

    private void ShowCreateDeckNotification()
    {
        questNotification.HideNotification();
        createDeckNotification.ShowNotification();
        collectionState.SetNotifications(new Dictionary<string, bool>(){{Constants.CollectionStateCreateDeck, true}});
    }

    public async void HideDeckBuildingTutorial()
    {
        if (tutorialDeckBuilding.activeSelf)
        {
            tutorialDeckBuilding.SetActive(false);
            await TutorialStorageAPI.SaveTutorialStorage(TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered, TutorialStorageAPI.TutorialStoragePlayer.LaunchArenaKey, TutorialStorageAPI.TutorialStoragePlayer.RewardForStep, true);
        }
    }
}
