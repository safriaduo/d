using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Database;
using Dawnshard.Menu;
using Dawnshard.Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Menu
{ 
    public class TutorialNewSetPopup : Popup
    {
        [SerializeField] private GameObject setParents;
        [SerializeField] private Button goToSetsButton;
        [SerializeField] private RectTransform boxContainer;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private PopupOneButton worldPackReward;

        protected override void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                Close();
            });
            HideError();
        }
        
        private void SetupPopup()
        {
            if (TutorialStorageAPI.TutorialStoragePlayer.RewardForStep >= 4)
            {
                Close();
                return;
            }
            
            //se non c'è ancora stata la registrazione
            if (TutorialStorageAPI.TutorialStoragePlayer == null || !TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered)
            {
                Close();
                return;
            }
            //for the first reward
            if(TutorialStorageAPI.TutorialStoragePlayer.RewardForStep==0 && GameController.Instance.UserMetadata.CompletedTutorials>1)
            {
                Open();
                return;
            }
            //if the player has not already seen the reward for the current quest
            if (TutorialStorageAPI.TutorialStoragePlayer.RewardForStep==GameController.Instance.UserMetadata.CompletedTutorials-2)
            {
                Open();
                return;
            }
            Close();
        }

        public override void Open()
        {
            base.Open();
            ShowSets();
        }

        private void Update()
        {
            if (!parentObject.activeSelf)
            {
                SetupPopup();
            }
        }

        private void ShowSets()
        {
            var cardSets = CardSetAPI.CardSets;

            switch (GameController.Instance.UserMetadata.CompletedTutorials)
            {
                case 2:
                {
                    CardSetFactory.Instance.CreateMiniCardSetView(
                        cardSets.Find(cardset => cardset.WorldId.Contains("Golden")), setParents.transform);
                    CardSetFactory.Instance.CreateMiniCardSetView(
                        cardSets.Find(cardset => cardset.WorldId.Contains("Ancestral")), setParents.transform);
                    CardSetFactory.Instance.CreateMiniCardSetView(
                        cardSets.Find(cardset => cardset.WorldId.Contains("Court")), setParents.transform);
                    break;
                }         
                case 3:
                {
                    boxContainer.sizeDelta= new Vector2(boxContainer.rect.width, 600);
                    titleText.text = "Lost Eden";
                    CardSetFactory.Instance.CreateMiniCardSetView(
                        cardSets.Find(cardset => cardset.WorldId.Contains("Lost")), setParents.transform);
                    OpenWorldPackPopup();
                    break;
                }      
                case 4:
                {
                    boxContainer.sizeDelta= new Vector2(boxContainer.rect.width, 600);
                    titleText.text = "Army of Darkness";
                    CardSetFactory.Instance.CreateMiniCardSetView(
                        cardSets.Find(cardset => cardset.WorldId.Contains("Army")), setParents.transform);
                    OpenWorldPackPopup();
                    break;
                }      
                case 5:
                {
                    boxContainer.sizeDelta= new Vector2(boxContainer.rect.width, 600);
                    titleText.text = "Progeny Beyond Time";
                    CardSetFactory.Instance.CreateMiniCardSetView(
                        cardSets.Find(cardset => cardset.Name.Contains("Progeny")), setParents.transform);
                    OpenWorldPackPopup();
                    break;
                }      
            }

            TutorialStorageAPI.SaveTutorialStorage(TutorialStorageAPI.TutorialStoragePlayer.AlreadyRegistered, TutorialStorageAPI.TutorialStoragePlayer.LaunchArenaKey, GameController.Instance.UserMetadata.CompletedTutorials-1, TutorialStorageAPI.TutorialStoragePlayer.FirstDeckCreated, TutorialStorageAPI.TutorialStoragePlayer.AllQuestsCompleted);
            goToSetsButton.onClick.AddListener(GoToSets);
        }

        private void OpenWorldPackPopup()
        {
            //worldPackReward.Open();
            //worldPackReward.SetUpPopupButton("You gained a world pack!\nGo to Packs and choose your favourite world!", "Close", worldPackReward.Close, "you obtained a reward");
        }

        private void GoToSets()
        {
            MenuManager.Instance.GoToState(Constants.CollectionState, true);
            Close();
        }
    }
}
