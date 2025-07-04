using System;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using UnityEngine;

namespace Dawnshard.Menu
{
    public class PlayState : AState
    {
        private enum PlayStateType
        {
            None,
            Quest,
            Friendly,
            Ranked,
            Reverse,
        }

        [SerializeField] private RankedMatchPopup rankedMatchPopup;
        [SerializeField] private TrainingMatchPopup trainingMatchPopup;
        [SerializeField] private QuestMatchPopup questMatchPopup;
        [SerializeField] private GameObject deckParent;
        [SerializeField] private PopupOneButton popupOneButton;
        
        [Header("Show info UI")]
        [SerializeField] private CardListSelector cardListSelector;
        [SerializeField] private Transform cardSetParentTransform;


        private List<DeckModel> decks;
        private List<InteractableDeckPresenter> deckPresenters = new();
        private PlayStateType currentState = PlayStateType.None;


        public override void Enter(AState from)
        {
            base.Enter(from);

            MenuManager.Instance.SetOptions(new Dictionary<string, Action>()
            {
                { Constants.PlayStateQuest, OnQuestMatchPressed },
                { Constants.PlayStateReverse, OnReverseMatchPressed },
                { Constants.PlayStateFriendly, OnTrainingMatchPressed },
                { Constants.PlayStateRanked, OnRankedMatchPressed }
            }, Constants.PlayStateQuest, notifications:notifications, lockedOptions:lockedOptions);

            OnQuestMatchPressed();
            questMatchPopup.Close();
            rankedMatchPopup.Close();
            trainingMatchPopup.Close();
        }

        private void OnReverseMatchPressed()
        {
            DeleteAllViews();
            currentState = PlayStateType.Ranked;
            rankedMatchPopup.IsReverseMode = true;
            ShowDecks();
        }

        private void OnQuestMatchPressed()
        {
            if (GameController.Instance.Decks.Count <= 0)
            {
                popupOneButton.Open();
                popupOneButton.SetUpPopupButton(
                    "To play you need to have a deck and you don't have any deck yet, do you want to create one?","New deck!", () =>
                {
                    MenuManager.Instance.GoToState(Constants.CollectionState, true);
                    popupOneButton.Close();
                },"You have no decks");
            }
            else
            {
                popupOneButton.Close();
            }
            DeleteAllViews();
            currentState = PlayStateType.Quest;
            ShowDecks();
        }

        public override void Exit(AState from)
        {
            if (rankedMatchPopup.isSearchingForMatch)
            {
                rankedMatchPopup.SetOnCloseAction(() => base.Exit(from));
                DeleteAllViews();
            }
            else
            {
                DeleteAllViews();
                base.Exit(from);
            }
        }

        private void OnTrainingMatchPressed()
        {
            DeleteAllViews();
            currentState = PlayStateType.Friendly;
            ShowDecks();
        }

        private void OnRankedMatchPressed()
        {
            DeleteAllViews();
            currentState = PlayStateType.Ranked;
            ShowDecks();
        }

        private void DeleteAllViews()
        {
            rankedMatchPopup.IsReverseMode = false;
            deckPresenters.Clear();
            foreach (Transform child in deckParent.transform)
            {
                Destroy(child.gameObject);
            }
            questMatchPopup.Close();
            rankedMatchPopup.Close();
            trainingMatchPopup.Close();
        }

        private void SelectDeck(DeckModel deck)
        {
            EnableDecksInteraction(false);
            if (currentState is PlayStateType.Ranked or PlayStateType.Reverse)
            {
                rankedMatchPopup.SetDeckView(deck, () => EnableDecksInteraction(true));
                rankedMatchPopup.Open();
            }
            else if (currentState == PlayStateType.Friendly)
            {
                trainingMatchPopup.SetDeckView(deck, () => EnableDecksInteraction(true));
                trainingMatchPopup.Open();
            }
            else if (currentState == PlayStateType.Quest)
            {
                questMatchPopup.SetDeckView(deck, () => EnableDecksInteraction(true));
                questMatchPopup.Open();
            }
        }

        private void EnableDecksInteraction(bool enable)
        {
            foreach (var deckPresenter in deckPresenters)
            {
                deckPresenter.EnableInteraction(enable);
            }
        }

        private void ShowDecks()
        {
            decks = GameController.Instance.Decks;

            //if (deckParent.transform.childCount == 0)
            {
                foreach (var deck in decks)
                {
                    var deckPresenter = DeckFactory.Instance.CreateInteractableDeckView(deck, deckParent.transform);
                    deckPresenters.Add(deckPresenter);
                    deckPresenter.ToggleButtonListener(true, () => SelectDeck(deck));
                    deckPresenter.ToggleInfoButtonListener(true, () =>
                    {
                        ShowDeckUI(deckPresenter.Model);
                    });
                }
            }
            /*else
            {
                foreach (Transform child in deckParent.transform)
                {
                    child.gameObject.SetActive(true);
                }

                foreach (var deck in deckPresenters)
                {
                    deck.EnableInteraction(true);
                }
            }*/
        }
        
        public void ShowDeckUI(DeckModel deckModel)
        {
            cardListSelector.gameObject.SetActive(true);
            CardFactory cardFactory = CardFactory.Instance;
            List<CardSetPresenter> cardSetsUI = new();
            foreach (var setIds in deckModel.CardSetIds)
            {
                if(!CardSetAPI.CardSets.Exists(cardSetModel => cardSetModel.ItemId == setIds))
                    return;

                var setModel = CardSetAPI.CardSets.Find(cardSetModel => cardSetModel.ItemId == setIds);
                cardSetsUI.Add(CardSetFactory.Instance.CreateCardSetView(setModel, cardSetParentTransform));
            }
            List<GameObject> setGO = new();
            foreach (Transform child in cardSetParentTransform)
            {
                setGO.Add(child.gameObject);
            }
            cardListSelector.Initialize(0, HideInfoUI, titleText: deckModel.Name);
        }
        
        private void HideInfoUI()
        {
            foreach (Transform child in cardSetParentTransform)
            {
                Destroy(child.gameObject);
            }
            cardListSelector.gameObject.SetActive(false);
        }


        public override string GetName() => Constants.PlayState;
    }
}
