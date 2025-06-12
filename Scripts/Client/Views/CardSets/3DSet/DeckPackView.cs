using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Database;
using Dawnshard.Menu;
using Dawnshard.Network;
using Dawnshard.Presenters;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class DeckPackView : MonoBehaviour
    {
        [SerializeField] private MMFeedbacks hoverAnimation;
        [SerializeField] private MMFeedbacks firstClicksAnimation;
        [SerializeField] private MMFeedbacks secondClicksAnimation;
        [SerializeField] private MMFeedbacks lastClicksAnimation;
        [SerializeField] private MMFeedbacks growLightAnimation;
        [SerializeField] private MMFeedbacks lockInUnpackAnimation;
        [SerializeField] private MMFeedbacks createSetAnimation;
        [SerializeField] private MMFeedbacks endSetAnimation;
        [SerializeField] private GameObject deckPack;
        [SerializeField] private Transform cardsParent;
        [SerializeField] private Transform setParent;


        bool hasLightGrowPlayed = false;
        bool hasLockedIn = false;
        private int cardShown = 0;

        private CardSetPresenter setPresenter = null;

        public void DeckPackClicked()
        {
            if(!hasLightGrowPlayed && deckPack.transform.localScale.magnitude > 210 * Math.Cbrt(3))
            {
                hasLightGrowPlayed = true;
                growLightAnimation.PlayFeedbacks();
            }
            if (deckPack.transform.localScale.magnitude > 190 * Math.Cbrt(3))
            {
                lastClicksAnimation.PlayFeedbacks();
            }
            if (deckPack.transform.localScale.magnitude > 170*Math.Cbrt(3))
            {
                secondClicksAnimation.PlayFeedbacks();
                if (!hasLockedIn)
                {
                    hasLockedIn = true;
                    lockInUnpackAnimation.PlayFeedbacks();
                }
            }
            firstClicksAnimation.PlayFeedbacks();
        }

        public void DeckPackHovered()
        {
            if(!hasLockedIn)
                hoverAnimation.PlayFeedbacks();
        }

        public async void CreateNewSet(string packName, string world)
        {
            LoadingPopup.Instance.Open();
            CardSetModel cardSetModel;
            try
            { 
                cardSetModel = await CardSetAPI.OpenPack(packName, world);
            }
            catch (Exception e)
            {
                LoadingPopup.Instance.Close();
                ErrorPopup.Instance.Open();
                ErrorPopup.Instance.SetErrorText(e.Message);
                return;
            }
            lockInUnpackAnimation.Events.OnComplete.RemoveAllListeners();
            growLightAnimation.Events.OnComplete.RemoveAllListeners();
            growLightAnimation.Events.OnComplete.AddListener(() =>
            {
                createSetAnimation.PlayFeedbacks();

            });
            lockInUnpackAnimation.Events.OnComplete.AddListener(() =>
            {
                growLightAnimation.PlayFeedbacks();
            });
            lockInUnpackAnimation.PlayFeedbacks();
            setPresenter = CardSetFactory.Instance.CreateCardSetView(cardSetModel, setParent);
            setPresenter.HideAllCards();
            LoadingPopup.Instance.Close();
        }

        public void CreateCards()
        {
            int i = 0;
            var cardList = setPresenter.Model.Cards.ToList();
            cardList.Reverse();
            foreach (var card in cardList)
            {
                CardModel cardModel = CardDatabase.GetCardByName(card);
                var cardPresenter = CardFactory.Instance.CreateBaseCard(cardModel, cardsParent);
                if(cardsParent.GetChild(i).TryGetComponent(out BaseCardView cardView)){
                    cardsParent.GetChild(i).localScale = 600f*Vector3.one;
                    cardView.SetSetPosition(new Vector3(setParent.position.x, setParent.position.y, cardsParent.GetChild(i).localPosition.z-10f));
                    cardPresenter.RegisterInputCallback(
                        UserInput.MouseDown,
                        () =>
                        {
                            cardView.GoToSet();
                            AddCardToSet();
                            cardPresenter.UnregisterInputCallback(UserInput.MouseDown);
                        }
                    );
                    i++;
                }
            }
        }

        public void AddCardToSet()
        {
            cardShown++;
            if (setPresenter != null)
            {
                setPresenter.ShowNextCard(cardShown-1);
            }

            if (cardShown == 10)
            {
                endSetAnimation.PlayFeedbacks();
            }
        }
    }
}

