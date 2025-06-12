using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AlturaNFT;
using Dawnshard.Database;
using Dawnshard.Network;
using Dawnshard.Presenters;
using Dawnshard.Views;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Dawnshard.Menu
{
    public class ForgeState : AState
    {
        [SerializeField] private GameObject forgeForegroundGameObjects;
        
        [SerializeField] private GameObject viewsParent;
        [SerializeField] private GameObject tutorialForge;
        [SerializeField] private Transform optionMenuPosition;

        [SerializeField] private PlayerStatsPopup playerStatsPopup;
        
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button burnButton;
        
        [SerializeField] private TMP_Text costText;
        
        [SerializeField] private ConfirmPopup confirmPopup;
        
        [SerializeField] private MMFeedbacks upgradeAnimation;
        [SerializeField] private MMFeedbacks forgeSetupAnimation;
        [SerializeField] private MMFeedbacks burnAnimation;
        [SerializeField] private MMFeedbacks cardSetSelectedAnimation;

        [SerializeField] private InteractableCardSetView dummyCardSetSelected;
        
        [Header("Search bar")]
        [SerializeField] private GameObject filterOptions;
        [SerializeField] private TMP_InputField searchByNameInputField;
        [SerializeField] private TogglePopup searchBySetWorldToggle;
        [SerializeField] private Toggle[] worldFilterToggles;
        [SerializeField] private Button clearFiltersButton;
        [SerializeField] private Image worldFilterImage;
        [SerializeField] private GameObject worldFilterOptions;
        
        private bool isCollectionFiltered = false;
        private Sprite worldFilterSpriteDefault;
        private Color worldFilterColorDefault;
        
        private string worldFilter = "";
        private string nameFilter = "";

        
        [Header("Show info UI")]
        [SerializeField] private CardListSelector cardListSelector;
        [SerializeField] private SliderCardsAnimation sliderCardsAnimation;
        [SerializeField] private Transform cardParentTransform;
        
        private List<InteractableCardSetPresenter> interactableSetPresenters = new();
        private bool isCardSetSelected = false;
        private bool isBurnOption = false;
        private InteractableCardSetPresenter selectedCardSet;
        private Dictionary<string, int> currencies = new();
        
        private const string CONFIRM_BURN_TITLE = "Confirm Burn";
        private const string CONFIRM_UPGRADE_TITLE = "Confirm Upgrade";
        public static string ConfirmButtonBurn(int reward) => $"You will burn this set and gain {reward} {Constants.Debristar}.\n All decks that contain this set will be deleted, too.\n Are you sure?";
        public static string ConfirmButtonUpgrade(int reward) => $"You will upgrade this set for {reward} {Constants.Debristar}.\n  Are you sure?";

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
                    foreach (var set in interactableSetPresenters)
                    {
                        set.ShowSet(true);
                        set.ToggleVisibility(true, null);
                    }
            }
            else
            {
                var cardRegEx = CreateContainsRegex(cardName);
                isCollectionFiltered = true;
                foreach (var set in interactableSetPresenters)
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
        
        private void Update()
        {
            clearFiltersButton.gameObject.SetActive(isCollectionFiltered);
        }
        
        private void FilterSetByWorld(string worldId="")
        {
            worldFilter = worldId;
            if (string.IsNullOrEmpty(worldId))
            {
                foreach (var set in interactableSetPresenters)
                {
                    set.ShowSet(true);
                    set.ToggleVisibility(true, null);
                }
            }
            else
            {
                isCollectionFiltered = true;
                foreach (var set in interactableSetPresenters)
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

        public override void Enter(AState from)
        {
            base.Enter(from);
            ClearFilters();
            MenuManager.Instance.SetOptionsPosition(optionMenuPosition);
            forgeSetupAnimation.Events.OnComplete.RemoveAllListeners();
            cardSetSelectedAnimation.Events.OnComplete.RemoveAllListeners();
            
            forgeForegroundGameObjects.SetActive(true);
            
            forgeSetupAnimation.PlayFeedbacks();

            // MenuManager.Instance.SetOptions(new Dictionary<string, Action>()
            // {
            //     { Constants.ForgeStateForge, OnForgePressed },
            //     { Constants.ForgeStateBurn, OnBurnPressed }
            // },string.Empty);
            LoadUserData();
        }
        
        private async Task LoadUserData()
        {
            try
            {
                await CardSetAPI.LoadSets(()=>
                {
                    ShowInteractableSets();
                });
                await GameController.Instance.RefreshWallet();
                currencies = GameController.Instance.Wallet;
                playerStatsPopup.Initialize(currencies.ContainsKey(Constants.Debristar)?
                        currencies[Constants.Debristar]:0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }        
        }

        public override void Exit(AState from)
        {
            forgeSetupAnimation.Events.OnComplete.RemoveAllListeners();
            forgeSetupAnimation.Events.OnComplete.AddListener(() =>
            {
                DeleteAllViews();
                forgeForegroundGameObjects.SetActive(false);
                base.Exit(from);
            });

            if (isCardSetSelected)
            {
                cardSetSelectedAnimation.Events.OnComplete.RemoveAllListeners();
                cardSetSelectedAnimation.Events.OnComplete.AddListener(()=>
                {
                    forgeSetupAnimation.PlayFeedbacks();
                    isCardSetSelected = false;
                    selectedCardSet = null;
                    dummyCardSetSelected.gameObject.SetActive(false);
                });
                cardSetSelectedAnimation.PlayFeedbacks();
                return;
            }
            forgeSetupAnimation.PlayFeedbacks();
        }
        
        private void ShowInteractableSets()
        {
            ShowInteractableSets(CardSetAPI.PlayerSets);
            //ShowInteractableSets(CardSetAPI.BaseSets);
        }

        private void DeleteAllViews()
        {
            interactableSetPresenters.Clear();
            foreach (Transform child in viewsParent.transform)
            {
                if(child!=viewsParent.transform)
                    Destroy(child.gameObject);
            }
        }
        
        private void ShowInteractableSets(List<CardSetModel> cardSetModels)
        {
            int setNumber = cardSetModels.Count;
            for (int i = 0; i < setNumber; i++)
            {
                var cardSetModel = cardSetModels[i];
                var setPresenter = CardSetFactory.Instance.CreateInteractableCardSetViewAlternate(cardSetModel, viewsParent.transform);

                setPresenter.ToggleButtonListener(true, () =>
                {
                    if (isCardSetSelected) return;

                    SelectCardSet(setPresenter, cardSetModel);
                    SetupOptionButtons(cardSetModel);
                });
                setPresenter.ToggleInfoButtonListener(true, () => { ShowSetUI(setPresenter.Model); });
                interactableSetPresenters.Add(setPresenter);
            }
        }

        private void SetupOptionButtons(CardSetModel cardSetModel)
        {
            burnButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.RemoveAllListeners();
            burnButton.onClick.AddListener(() =>
            {
                confirmPopup.SetConfirmPopup(()=>{
                    if (isCardSetSelected)
                    {
                        BurnSetCoroutine();
                    }
                },true, ConfirmButtonBurn(ForgeAPI.GetBurnReward(cardSetModel)),"Confirm burn", "Confirm!");
                confirmPopup.Open();
            });
            if (cardSetModel.IncandescenseLevel == Constants.OneiricIncandescense)
            {
                upgradeButton.interactable = false;
            }
            else
            {
                upgradeButton.interactable = true;
                upgradeButton.onClick.AddListener(() =>
                {
                    confirmPopup.SetConfirmPopup(()=>{
                            if (isCardSetSelected)
                            {
                                UpgradeSetCoroutine();
                            }
                        }, currencies.TryGetValue(Constants.Debristar, out var currency) && ForgeAPI.GetBurnReward(cardSetModel)<currency,
                        ConfirmButtonUpgrade(ForgeAPI.GetUpgradeCost(cardSetModel)),"Confirm upgrade", "Confirm!");
                    confirmPopup.Open();
                });
            }

            
        }

        private void SelectCardSet(InteractableCardSetPresenter setPresenter, CardSetModel cardSetModel)
        {
            cardSetSelectedAnimation.PlayFeedbacks();
            isCardSetSelected = true;
            selectedCardSet = setPresenter;
            var dummyCardSetPresenter = new InteractableCardSetPresenter(dummyCardSetSelected, cardSetModel, true);
            dummyCardSetPresenter.ToggleButtonListener(true, () =>
            {
                DeselectCardSet();
            });

            if(cardSetModel.IncandescenseLevel!=Constants.OneiricIncandescense)
                costText.text = ForgeAPI.GetUpgradeCost(cardSetModel).ToString();
            dummyCardSetSelected.gameObject.SetActive(true);
        }
        
        private void DeselectCardSet()
        {
            cardSetSelectedAnimation.PlayFeedbacks();
            isCardSetSelected = false;
            selectedCardSet = null;
            dummyCardSetSelected.gameObject.SetActive(false);
        }

        private async void  UpgradeSetCoroutine()
        {
            LoadingPopup.Instance.Open();
            await ForgeAPI.UpgradeSetRequest(selectedCardSet.Model);
            LoadingPopup.Instance.Close();
            upgradeAnimation.Events.OnComplete.RemoveAllListeners();
            upgradeAnimation.Events.OnComplete.AddListener(() =>
            {
                RefreshForge();
                DeselectCardSet();
            });
            upgradeAnimation.PlayFeedbacks();
            //yield return new WaitForSeconds(upgradeAnimation.TotalDuration+cardSetSelectedAnimation.TotalDuration+2f);
        }

        private async void BurnSetCoroutine()
        {
            LoadingPopup.Instance.Open();
            await ForgeAPI.BurnSetRequest(selectedCardSet.Model);
            LoadingPopup.Instance.Close();
            burnAnimation.Events.OnComplete.RemoveAllListeners();
            burnAnimation.Events.OnComplete.AddListener(() =>
            {
                RefreshForge();
                DeselectCardSet();
            });
            burnAnimation.PlayFeedbacks();
            //yield return new WaitForSeconds(burnAnimation.TotalDuration+cardSetSelectedAnimation.TotalDuration+2f);
        }
        
        private async void RefreshForge()
        {
            DeleteAllViews();
            await LoadUserData();
        }
        
        private void ShowSetUI(CardSetModel cardSetModel)
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
            cardListSelector.gameObject.SetActive(false);
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

        public override string GetName() => Constants.ForgeState;
    }

}
