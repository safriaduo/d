using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AlturaNFT;
using Dawnshard.Database;
using Dawnshard.Network;
using Dawnshard.Presenters;
using Dawnshard.Views;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Dawnshard.Menu
{
    public class CollectionState : AState
    {
        [SerializeField] private GameObject viewsParent;
        [SerializeField] private CreateDeckPopup createDeckPopup;
        [SerializeField] private GameObject tutorialDeckBuilding;
        [SerializeField] private ConfirmPopup confirmPopup;
        [SerializeField] private StudioEventEmitter studioEventEmitter;
        
        [Header("Search bar")]
        [SerializeField] private GameObject filterOptions;
        [SerializeField] private TMP_InputField searchByNameInputField;
        [SerializeField] private TogglePopup searchBySetWorldToggle;
        [SerializeField] private Toggle[] worldFilterToggles;
        [SerializeField] private Button clearFiltersButton;
        [SerializeField] private Image worldFilterImage;
        [SerializeField] private GameObject worldFilterOptions;
        
        [Header("Show info UI")]
        [SerializeField] private CardListSelector cardListSelector;
        [SerializeField] private SliderCardsAnimation sliderCardsAnimation;
        [SerializeField] private Transform cardParentTransform;
        [SerializeField] private Transform cardSetParentTransform;



        private bool isCollectionFiltered = false;

        private List<DeckModel> decks;
        private List<DeckPresenter> deckPresenters = new();

        private List<CardSetPresenter> setPresenters = new();

        private List<InteractableCardSetPresenter> interactableSetPresenters = new();
        private List<InteractableCardSetPresenter> filteredInteractableSetPresenters = new();

        private Sprite worldFilterSpriteDefault;
        private Color worldFilterColorDefault;

        bool isCreatingDeck = false;
        
        private const int BASE_SET_COUNT = 3;
        private int createDeckSetCount = 0;
        private string worldFilter = "";
        private string nameFilter = "";


        public override void Enter(AState from)
        {
            base.Enter(from);

            MenuManager.Instance.SetOptions(new Dictionary<string, Action>()
            {
                { Constants.CollectionStateSets, OnSetPressed },
                { Constants.CollectionStateDecks, OnDeckPressed },
                { Constants.CollectionStateCreateDeck, OnCreateDeckPressed }
            },  Constants.CollectionStateSets, notifications: notifications);
        }

        private void Awake()
        {
            worldFilterSpriteDefault = worldFilterImage.sprite;
            worldFilterColorDefault = worldFilterImage.color;
        }

        private void Start()
        {
            foreach (var toggle in worldFilterToggles)
            {
                toggle.onValueChanged.AddListener((isOn)=>
                {
                    if (isOn)
                    {
                        worldFilterImage.sprite = AssetDatabase.Instance.GetWorldRecord(searchBySetWorldToggle.GetActiveToggle()).worldIcon;
                        worldFilterImage.color = AssetDatabase.Instance.GetWorldRecord(searchBySetWorldToggle.GetActiveToggle()).mainColor;
                        FilterSetByWorld();
                        FilterSetByWorld(searchBySetWorldToggle.GetActiveToggle());
                        FilterSetByName(nameFilter);
                        worldFilterOptions.SetActive(false);
                    }
                });
            }
            clearFiltersButton.onClick.AddListener(() =>
            {
                foreach (var toggle in worldFilterToggles)
                {
                    toggle.isOn = false;
                }
                searchByNameInputField.text = string.Empty;
                ClearFilters();
            });
            searchByNameInputField.onValueChanged.AddListener((newValue)=>
            {
                FilterSetByName(newValue);
                FilterSetByWorld(worldFilter);
            });
        }

        private void Update()
        {
            clearFiltersButton.gameObject.SetActive(isCollectionFiltered);
        }

        public override void Exit(AState from)
        {
            // Debug.Log("close");
            createDeckPopup.Close();
            DeleteAllViews();
            //
            base.Exit(from);
        }

        public void ClearFilters()
        {
            if (!isCollectionFiltered)
                return;
            FilterSetByWorld();
            FilterSetByName();
            isCollectionFiltered = false;
            worldFilterImage.sprite = worldFilterSpriteDefault;
            worldFilterImage.color = worldFilterColorDefault;
        }
        
        public static Regex CreateContainsRegex(string searchText, bool ignoreCase = true)
        {
            // Escape the searchText to treat special characters literally.
            string escaped = Regex.Escape(searchText);

            // Replace escaped asterisks (\*) with the regex wildcard .*
            escaped = escaped.Replace(@"\*", ".*");

            // Build a pattern that matches any string containing the modified searchText.
            string pattern = ".*" + escaped + ".*";

            RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            return new Regex(pattern, options);
        }
        
        private void FilterSetByName(string cardName="")
        {
            nameFilter = cardName;
            if (string.IsNullOrEmpty(cardName))
            {
                if (isCreatingDeck)
                {
                    foreach (var set in filteredInteractableSetPresenters)
                    {
                        set.ShowSet(true);
                        set.ToggleVisibility(true, null);
                    }
                }
                else
                {
                    foreach (var set in setPresenters)
                    {
                        set.ShowSet(true);
                        set.ToggleVisibility(true, null);
                    }
                }
            }
            else
            {
                var cardRegEx = CreateContainsRegex(cardName);
                isCollectionFiltered = true;

                if (isCreatingDeck)
                {
                    foreach (var set in filteredInteractableSetPresenters)
                    {
                        bool match = false;
                        foreach (var card in set.Model.Cards)
                        {
                            if (cardRegEx.IsMatch(card))
                            {
                                match = true;
                                break;
                            }
                        }
                        if(!match && !cardRegEx.IsMatch(set.Model.Name))
                            set.ToggleVisibility(false, () => set.ShowSet(false));
                        else
                        {
                            set.ShowSet(true);
                            set.ToggleVisibility(true);
                        }
                    }
                }
                else
                {
                    foreach (var set in setPresenters)
                    {
                        bool match = false;
                        foreach (var card in set.Model.Cards)
                        {
                            if (cardRegEx.IsMatch(card))
                            {
                                match = true;
                                break;
                            }
                        }
                        if(!match && !cardRegEx.IsMatch(set.Model.Name))
                            set.ToggleVisibility(false, () => set.ShowSet(false));
                        else
                        {
                            set.ShowSet(true);
                            set.ToggleVisibility(true);
                        }
                    }
                }
            }
        }
        
        private void FilterSetByWorld(string worldId="")
        {
            worldFilter = worldId;
            if (string.IsNullOrEmpty(worldId))
            {
                if (isCreatingDeck)
                {
                    foreach (var set in filteredInteractableSetPresenters)
                    {
                            set.ShowSet(true);
                            set.ToggleVisibility(true, null);
                    }
                }
                else
                {
                    foreach (var set in setPresenters)
                    {
                            set.ShowSet(true);
                            set.ToggleVisibility(true, null);
                    }
                }
            }
            else
            {
                isCollectionFiltered = true;
                if (isCreatingDeck)
                {
                    foreach (var set in filteredInteractableSetPresenters)
                    {
                        if(!set.IsVisible())
                            continue;
                        if (worldId != set.Model.WorldId)
                        {
                            set.ToggleVisibility(false, () => set.ShowSet(false));
                        }
                    }
                }
                else
                {
                    foreach (var set in setPresenters)
                    {
                        if(!set.IsVisible())
                            continue;
                        if (worldId != set.Model.WorldId)
                        {
                            set.ToggleVisibility(false, () => set.ShowSet(false));
                        }
                    }
                }
            }
        }

        private void OnDeckPressed()
        {
            filterOptions.SetActive(false);
            DeleteAllViews();
            ShowDecks();
            createDeckPopup.Close();
            isLikeHome = false;
        }

        private void OnCreateDeckPressed()
        {
            filterOptions.SetActive(true);
            DeleteAllViews();
            tutorialDeckBuilding.SetActive(!TutorialStorageAPI.TutorialStoragePlayer.AllQuestsCompleted);
            ShowInteractableSets();
            createDeckPopup.SetOnSaved(model =>
            {
                createDeckPopup.RemoveAllSet();
                createDeckSetCount = 0;
                SaveDeck(model);
            });
            //createDeckPopup.SetOnPopupClosed(OnSetPressed);
            createDeckPopup.Open();
            isLikeHome = true;;
            isCreatingDeck = true;
            filteredInteractableSetPresenters = interactableSetPresenters.ToList();
            ClearFilters();
        }

        private void ShowInteractableSets()
        {
            var sets = CardSetAPI.BaseSets;
            var unlockedBaseSet = GameController.Instance.UserMetadata.CompletedTutorials;

            var orderedSets = OrderSets(sets);
            ShowInteractableSets(orderedSets, true, unlockedBaseSet - 1);
            ShowInteractableSets(CardSetAPI.PlayerSets, false);
        }

        private void OnSetPressed()
        {
            filterOptions.SetActive(true);
            DeleteAllViews();
            ShowSets();
            createDeckPopup.Close();
            isLikeHome = false;
            isCreatingDeck = false;
            ClearFilters();
        }

        private void DeleteAllViews()
        {
            deckPresenters.Clear();
            foreach (Transform child in viewsParent.transform)
            {
                Destroy(child.gameObject);
            }
            createDeckSetCount = 0;
            createDeckPopup.RemoveAllSet();
            setPresenters.Clear();
            interactableSetPresenters.Clear();
        }

        private void ShowDecks()
        {
            DeleteAllViews();
            decks = GameController.Instance.Decks;
            foreach (var deck in decks)
            {
                var deckPresenter = DeckFactory.Instance.CreateEditInteractableDeckViewCross(deck, viewsParent.transform);
                deckPresenters.Add(deckPresenter);
                deckPresenter.ToggleButtonListener(true, () =>
                {
                    confirmPopup.SetConfirmPopup(async () =>
                    {
                        try
                        {
                            await GameController.Instance.DeleteDeck(deck);
                            deckPresenters.Remove(deckPresenter);
                            DeleteAllViews();
                            ShowDecks();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }, true,
                        $"Do you want to delete the deck {deck.Name}?\n Your set won't be deleted.",
                        "Delete deck",
                        "Delete"
                        );
                    confirmPopup.Open();
                });
                deckPresenter.ToggleInfoButtonListener(true, () =>
                {
                    ShowDeckUI(deckPresenter.Model);
                });
                deckPresenter.ToggleEditListener(true, async (text)=>
                {
                    if (!decks.ConvertAll(deckModel => deckModel.Name).Contains(text) && !string.IsNullOrEmpty(text))
                    {
                        var newDeckModel = new DeckModel();
                        newDeckModel.Name = text;
                        newDeckModel.DeckSpriteId = deck.DeckSpriteId;
                        newDeckModel.CardSetIds = deck.CardSetIds.ToList();
                        try
                        {
                            await GameController.Instance.DeleteDeck(deck);
                            await GameController.Instance.SaveDeck(newDeckModel);
                            deckPresenters.Remove(deckPresenter);
                            DeleteAllViews();
                            ShowDecks();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                });
            }
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

        private void ShowSets()
        {
            //TODO: Function that gets base sets
            var sets = CardSetAPI.BaseSets;
            var unlockedBaseSet = GameController.Instance.UserMetadata.CompletedTutorials;

            var orderedSets = OrderSets(sets);
            ShowSets(orderedSets, true, unlockedBaseSet - 1);
            ShowSets(CardSetAPI.PlayerSets, false);
        }

        private void ShowSets(List<CardSetModel> cardSetModels, bool baseSets, int unlockedBaseSet = 0)
        {
            int setNumber = baseSets ? Mathf.Min(BASE_SET_COUNT + unlockedBaseSet - 1,6) : cardSetModels.Count;
            for (int i = 0; i < setNumber; i++)
            {
                var setPresenter = CardSetFactory.Instance.CreateCardSetView(cardSetModels[i], viewsParent.transform);
                setPresenter.ToggleInfoButtonListener(true, () => { ShowSetUI(setPresenter.Model); });
                setPresenters.Add(setPresenter);
            }
        }
        
        public void ShowSetUI(CardSetModel cardSetModel)
        {
            cardListSelector.gameObject.SetActive(true);
            CardFactory cardFactory = CardFactory.Instance;
            List<IBaseCardPresenter> cardPresentersUI = new();
            foreach (var card in cardSetModel.Cards)
            {
                CardModel cardModel;
                if ( (cardModel=CardDatabase.GetCardByName(card)) != null)
                {
                    cardModel.IncandescenseLevel = cardSetModel.IncandescenseLevel;
                    cardPresentersUI.Add(cardFactory.CreateBaseCard(cardModel,
                        cardParentTransform));
                }
            }
            List<GameObject> cardsGO = new();
            foreach (Transform child in cardParentTransform)
            {
                cardsGO.Add(child.gameObject);
            }
            sliderCardsAnimation.AddCardList(cardsGO);
            cardListSelector.Initialize(0, HideInfoUI, titleText: cardSetModel.Name);
        }

        private void HideInfoUI()
        {
            foreach (Transform child in cardParentTransform)
            {
                Destroy(child.gameObject);
            }
            sliderCardsAnimation.Clear();
            foreach (Transform child in cardSetParentTransform)
            {
                Destroy(child.gameObject);
            }
            cardListSelector.gameObject.SetActive(false);
        }

        private void ShowInteractableSets(List<CardSetModel> cardSetModels, bool baseSets, int unlockedBaseSet = 0)
        {
            int setNumber = baseSets ? Mathf.Min(BASE_SET_COUNT + unlockedBaseSet - 1,6) : cardSetModels.Count;
            for (int i = 0; i < setNumber; i++)
            {
                var cardSetModel = cardSetModels[i];
                var setPresenter = CardSetFactory.Instance.CreateInteractableCardSetView(cardSetModel, viewsParent.transform);

                void OnRemoved()
                {
                    foreach (var set in interactableSetPresenters)
                    {
                        if (set.Model.WorldId == setPresenter.Model.WorldId)
                        {
                            set.ShowSet(true);
                            set.ToggleVisibility(true, null);
                            filteredInteractableSetPresenters.Add(set);
                        }
                    }
                    createDeckSetCount--;
                }
                setPresenter.ToggleButtonListener(true, () =>
                {
                    if (createDeckSetCount >= 3)
                    {
                        return;
                    }
                    studioEventEmitter.Play();
                    createDeckSetCount++;
                    createDeckPopup.AddSet(cardSetModel, OnRemoved);
                    foreach (var set in interactableSetPresenters)
                    {
                        if (set.Model.WorldId == setPresenter.Model.WorldId)
                        {
                            filteredInteractableSetPresenters.Remove(set);
                            set.ToggleVisibility(false, () => set.ShowSet(false));
                        }
                    }
                });
                setPresenter.ToggleInfoButtonListener(true, () => { ShowSetUI(setPresenter.Model); });
                interactableSetPresenters.Add(setPresenter);
            }
        }

        private List<CardSetModel> OrderSets(List<CardSetModel> cardSetModels)
        {
            var orderedSet = new List<CardSetModel>();
            orderedSet.Add(cardSetModels.Find(model => model.WorldId.Contains("Golden")));
            orderedSet.Add(cardSetModels.Find(model => model.WorldId.Contains("Ancestral")));
            orderedSet.Add(cardSetModels.Find(model => model.WorldId.Contains("Court")));
            orderedSet.Add(cardSetModels.Find(model => model.WorldId.Contains("Lost")));
            orderedSet.Add(cardSetModels.Find(model => model.WorldId.Contains("Army")));
            orderedSet.Add(cardSetModels.Find(model => model.WorldId.Contains("Progeny")));
            return orderedSet;
        }

        private async void SaveDeck(DeckModel deck)
        {
            await GameController.Instance.SaveDeck(deck);
            MenuManager.Instance.SetOptions(new Dictionary<string, Action>()
            {
                { Constants.CollectionStateSets, OnSetPressed },
                { Constants.CollectionStateDecks, OnDeckPressed },
                { Constants.CollectionStateCreateDeck, OnCreateDeckPressed }
            }, notifications: notifications, invokeOption: Constants.CollectionStateDecks);
        }

        public override string GetName() => Constants.CollectionState;
    }

}
