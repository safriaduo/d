using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using Dawnshard.Network;
using Dawnshard.Presenters;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuestMatchPopup : RankedMatchPopup
{
    [SerializeField] private Button firstQuestButton;
    [SerializeField] private Button secondQuestButton;
    [SerializeField] private Button thirdQuestButton;
    [SerializeField] private GameObject firstQuestFinished;
    [SerializeField] private GameObject secondQuestFinished;
    [SerializeField] private GameObject thirdQuestFinished;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text flavourText;
    [SerializeField] private TMP_Text citText;
    [SerializeField] private Transform cardSetParent;

    private CardSetPresenter miniCardSetPresenter = null;

    private static readonly string[] titles = new string[] {
        "Select Quest", "Lost Eden","Army of Darkness","Progeny Beyond Time"
    };

    private static readonly string[] flavours = new string[] {
        "",
        "Lost Eden excels in swarming the battlefield with countless small creatures, each contributing to a relentless tide of shard gain. They overwhelm their enemies, flooding the field with unstoppable synergy.",
        "The Army of Darkness thrives in brutal combat, commanding fearsome creatures that dominate the battlefield and shatter enemy shards with relentless power.",
        "The Progeny Beyond Time thrives on annihilating enemy creatures and asserting board control. With devastating precision, they dismantle strategies and dominate the battlefield."
    };

    private static readonly string[] cits = new string[] {
        "",
        "\"We're born, we live and we die with the Dawnshards. We're part of them.\"",
        "\"They have driven us from the Void to be slaves. Now the universe will become our home!\"",
        "\"Hear the wispers of the abyss, for I am the harbinger of chaos\""
    };

    private int tutorialNumber = 0;

    private enum QuestTutorial
    {
        None = 0,
        First,
        Second,
        Third
    }

    private QuestTutorial questTutorial = QuestTutorial.None;

    private DeckModel deck;
    private void OnEnable()
    {
        findMatchButton.interactable = false;
        cardSetParent.gameObject.SetActive(false);
        titleText.text = titles[0];
        flavourText.text = flavours[0];
        citText.text = cits[0];
    }

    protected override void Start()
    {
        tutorialNumber = GameController.Instance.UserMetadata.CompletedTutorials;
        //tutorialNumber = GameController.Instance.GetTutorialStep();
        base.Start();
        IsSinglePlayer = true;
        firstQuestButton.onClick.AddListener(() => { SelectQuest(1); });
        if (tutorialNumber == 2)
        {
            thirdQuestButton.interactable = false;
            secondQuestButton.interactable = false;
            return;
        }

        secondQuestButton.onClick.AddListener(() =>
        {
            SelectQuest(2);
        });
        if (tutorialNumber == 3)
        {
            thirdQuestButton.interactable = false;
            firstQuestFinished.SetActive(true);
            return;
        }

        thirdQuestButton.onClick.AddListener(() =>
        {
            SelectQuest(3);
        });
        if (tutorialNumber == 4)
        {
            firstQuestFinished.SetActive(true);
            secondQuestFinished.SetActive(true);
            return;
        }

        if (tutorialNumber > 4)
        {
            firstQuestFinished.SetActive(true);
            secondQuestFinished.SetActive(true);
            thirdQuestFinished.SetActive(true);
        }
    }

    private void SelectQuest(int questNumber)
    {
        UpdateSetPresenter(CardSetAPI.CardSets.Find(set => set.WorldId == titles[questNumber]));
        cardSetParent.gameObject.SetActive(true);
        switch (questNumber)
        {
            case 1:        questTutorial = QuestTutorial.First;
                break;
            case 2:        questTutorial = QuestTutorial.Second;
                break;
            case 3:        questTutorial = QuestTutorial.Third;
                break;
            default:         questTutorial = QuestTutorial.First;
                break;
        }
        findMatchButton.interactable = true;
        titleText.text = titles[questNumber];
        flavourText.text = flavours[questNumber];
        citText.text = cits[questNumber];
    }

    private void UpdateSetPresenter(CardSetModel cardSetModel)
    {
        if (miniCardSetPresenter != null)
        {
            miniCardSetPresenter.Model = cardSetModel;
            miniCardSetPresenter.SetStaticFieldNoCards();
        }
        else
        {
            miniCardSetPresenter = CardSetFactory.Instance.CreateMiniCardSetView(cardSetModel, cardSetParent);
        }
    }

    protected override async void StartAIMatchAsync()
    {
        try
        {
            //await GameController.Instance.StartTutorialMatch();
            await GameController.Instance.StartTutorialMatch((int)questTutorial + 1, deck.Name);
            isSearchingForMatch = true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowError(e.Message);
            return;
        }
    }

    public override void SetDeckView(DeckModel deck, Action OnEnd)
    {
        this.deck = deck;
        OnPopupClosed = OnEnd;
    }
}
