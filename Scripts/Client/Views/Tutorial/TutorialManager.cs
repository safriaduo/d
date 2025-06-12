using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoLetterbox;
using Dawnshard.Database;
using Dawnshard.Network;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    // Define the tutorial steps as an enumeration for clarity
    public enum TutorialSteps
    {
        PlayCard = 0,
        Creature = 1,
        EndTurn = 2,

        Fight = 3,
        Hint = 4,

        HowToWin = 5,
        Action = 6,
        Reap = 7,

        World = 8,
        WorldCrystal = 9,
        Bench = 10,

        WaitingForArtefact = 11,
        Artifact = 12,

        WaitingForDiscard = 13,
        Discard = 14,
        Win = 15,

        NoTutorial = 16,
    }

    public static TutorialSteps currentTutorialStep = TutorialSteps.NoTutorial;

    [SerializeField] private TMP_Text concedeButtonText;
    // Serialized fields to assign GameObjects for each tutorial step in the Inspector
    [Header("TutorialFirstTurn")]
    [SerializeField]
    private List<GameObject> tutorialPlayCardGameObjects = new();
    [SerializeField]
    private List<GameObject> tutorialCreatureGameObjects = new();
    [SerializeField]
    private List<GameObject> tutorialEndTurnGameObjects = new();

    [Header("TutorialSecondTurn")]
    [SerializeField]
    private List<GameObject> tutorialFightGameObjects = new();
    [SerializeField]
    private List<GameObject> tutorialHintGameObjects = new();

    [Header("TutorialThirdTurn")]
    [SerializeField]
    private List<GameObject> tutorialHowToWinGameObjects = new();
    [SerializeField]
    private List<GameObject> tutorialActions = new();
    [SerializeField]
    private List<GameObject> tutorialReapGameObjects = new();

    [Header("TutorialFourthTurn")]
    [SerializeField]
    private List<GameObject> tutorialChangeWorld = new();
    [SerializeField]
    private List<GameObject> tutorialChangeWorldCrystal = new();
    [SerializeField]
    private List<GameObject> tutorialBench = new();

    [Header("OtherTutorials")]
    [SerializeField]
    private List<GameObject> tutorialArtifact = new();
    [SerializeField]
    private List<GameObject> tutorialDiscard = new();

    [Header("TutorialLastTurn")]
    [SerializeField]
    private List<GameObject> tutorialWin = new();

    private List<CardModel> cardModels = new();
    private EventBusManager eventBusManager;

    /// <summary>
    /// Displays the GameObjects associated with the current tutorial step.
    /// Hides all others.
    /// </summary>
    private void ShowCurrentStep()
    {
        HideAllSteps(); // Ensure all are hidden before showing the current

        switch (currentTutorialStep)
        {
            case TutorialSteps.PlayCard:
                tutorialPlayCardGameObjects.ForEach(o => o.SetActive(true));
                break;
            case TutorialSteps.Creature:
                tutorialCreatureGameObjects.ForEach(o => o.SetActive(true));
                UIManager.CurrentStateUI = UIManager.StateUI.Tutorial;
                break;
            case TutorialSteps.EndTurn:
                tutorialEndTurnGameObjects.ForEach(o => o.SetActive(true));
                UIManager.CurrentStateUI = UIManager.StateUI.Tutorial;
                break;
            case TutorialSteps.Fight:
                tutorialFightGameObjects.ForEach(o => o.SetActive(true));
                break;
            case TutorialSteps.Hint:
                tutorialHintGameObjects.ForEach(o => o.SetActive(true));
                UIManager.CurrentStateUI = UIManager.StateUI.Tutorial;
                break;
            case TutorialSteps.HowToWin:
                tutorialHowToWinGameObjects.ForEach(o => o.SetActive(true));
                UIManager.CurrentStateUI = UIManager.StateUI.Tutorial;
                break;
            case TutorialSteps.Reap:
                tutorialReapGameObjects.ForEach(o => o.SetActive(true));
                break;
            case TutorialSteps.Action:
                tutorialActions.ForEach(o => o.SetActive(true));
                break;
            case TutorialSteps.World:
                tutorialChangeWorld.ForEach(o => o.SetActive(true));
                break;
            case TutorialSteps.WorldCrystal:
                tutorialChangeWorldCrystal.ForEach(o => o.SetActive(true));
                UIManager.CurrentStateUI = UIManager.StateUI.Tutorial;
                break;
            case TutorialSteps.Bench:
                tutorialBench.ForEach(o => o.SetActive(true));
                UIManager.CurrentStateUI = UIManager.StateUI.Tutorial;
                break;
            case TutorialSteps.Artifact:
                tutorialArtifact.ForEach(o => o.SetActive(true));
                break;
            case TutorialSteps.Discard:
                tutorialDiscard.ForEach(o => o.SetActive(true));
                break;
            case TutorialSteps.Win:
                tutorialWin.ForEach(o => o.SetActive(true));
                UIManager.CurrentStateUI = UIManager.StateUI.Tutorial;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Hides all GameObjects associated with all tutorial steps.
    /// </summary>
    private void HideAllSteps()
    {
        if (UIManager.CurrentStateUI == UIManager.StateUI.Tutorial)
        {
            UIManager.CurrentStateUI = UIManager.StateUI.None;
        }
        tutorialPlayCardGameObjects.ForEach(o => o.SetActive(false));
        tutorialEndTurnGameObjects.ForEach(o => o.SetActive(false));
        tutorialCreatureGameObjects.ForEach(o => o.SetActive(false));

        tutorialFightGameObjects.ForEach(o => o.SetActive(false));
        tutorialHintGameObjects.ForEach(o => o.SetActive(false));

        tutorialActions.ForEach(o => o.SetActive(false));
        tutorialReapGameObjects.ForEach(o => o.SetActive(false));
        tutorialHowToWinGameObjects.ForEach(o => o.SetActive(false));

        tutorialChangeWorld.ForEach(o => o.SetActive(false));
        tutorialChangeWorldCrystal.ForEach(o => o.SetActive(false));
        tutorialBench.ForEach(o => o.SetActive(false));

        tutorialArtifact.ForEach(o => o.SetActive(false));
        tutorialDiscard.ForEach(o => o.SetActive(false));

        tutorialWin.ForEach(o => o.SetActive(false));
    }

    /// <summary>
    /// Resets the tutorial to the first step.
    /// Useful for testing or restarting the tutorial.
    /// </summary>
    public void ResetTutorial()
    {
        HideAllSteps();
        ShowCurrentStep();
    }

    /// <summary>
    /// Optional: Automatically advance the tutorial when the scene loads.
    /// You can remove this if you want to control advancement manually.
    /// </summary>
    private void Start()
    {
        if (GameController.Instance.UserMetadata.CompletedTutorials <= 1)
        {
            currentTutorialStep = TutorialSteps.PlayCard;
            concedeButtonText.text = "Skip Tutorial";
        }
        else
            currentTutorialStep = TutorialSteps.NoTutorial;
        HideAllSteps();
    }

    public void Initialize(EventBusManager eventBusManager)
    {
        this.eventBusManager = eventBusManager;
    }

    public void AddPlayer(string playerId)
    {
        eventBusManager.PlayerEventBus.Subscribe(playerId, OnEachGameEvent);
    }

    private void OnEachGameEvent(IGameEvent gameEvent)
    {

        switch (gameEvent)
        {
            case CardMovedEvent cardMovedEvent:
                {
                    if (currentTutorialStep == TutorialSteps.PlayCard)
                    {
                        HideAllSteps();
                        currentTutorialStep = TutorialSteps.Creature;
                        ShowCurrentStep();
                    }

                    else if (currentTutorialStep == TutorialSteps.Hint || (currentTutorialStep == TutorialSteps.Fight && cardMovedEvent.DestZoneId == Constants.BoardZone))
                    {
                        if (cardModels.Find(card => card.InstanceId == cardMovedEvent.CardInstanceId).IsOwnerLocalPlayer)
                        {
                            currentTutorialStep = TutorialSteps.Hint;
                            ShowCurrentStep();
                        }
                    }
                    else if (currentTutorialStep == TutorialSteps.Action)
                    {
                        HideAllSteps();

                        IEnumerator Reap()
                        {
                            yield return new WaitForSeconds(2f);
                            currentTutorialStep = TutorialSteps.Reap;
                            ShowCurrentStep();
                        }

                        StartCoroutine(Reap());
                    }
                    else if (currentTutorialStep == TutorialSteps.World)
                    {
                        if(cardModels.Find(model=>model.InstanceId==cardMovedEvent.CardInstanceId).WorldId.Contains("Golden") 
                           && cardMovedEvent.DestZoneId==Constants.BoardZone)
                        {
                            currentTutorialStep++;
                            HideAllSteps();
                            ShowCurrentStep();
                        }
                    }
                    else if (currentTutorialStep == TutorialSteps.WaitingForArtefact &&
                             cardModels.Find(cardModel => cardModel.InstanceId == cardMovedEvent.CardInstanceId).Type ==
                             Constants.ArtifactType && cardMovedEvent.DestZoneId == Constants.HandZone)
                    {
                        currentTutorialStep = TutorialSteps.Artifact;
                    }
                    else if (currentTutorialStep == TutorialSteps.Artifact &&
                             cardModels.Find(cardModel => cardModel.InstanceId == cardMovedEvent.CardInstanceId).Type ==
                             Constants.ArtifactType && cardMovedEvent.DestZoneId == Constants.BoardZone)
                    {
                        HideAllSteps();
                        currentTutorialStep = TutorialSteps.WaitingForDiscard;
                    }

                    else if (currentTutorialStep == TutorialSteps.Discard && cardMovedEvent.DestZoneId == Constants.GraveyardZone)
                    {
                        HideAllSteps();
                        currentTutorialStep = TutorialSteps.Win;
                    }
                    break;
                }

            case TurnStartedEvent turnStartedEvent:
                {
                    if (currentTutorialStep == TutorialSteps.PlayCard)
                    {
                        ShowCurrentStep();
                    }
                    else if (currentTutorialStep == TutorialSteps.Fight)
                    {
                        IEnumerator Fight()
                        {
                            yield return new WaitForSeconds(2f);
                            ShowCurrentStep();
                        }
                        HideAllSteps();
                        StartCoroutine(Fight());

                    }
                    else if (currentTutorialStep == TutorialSteps.HowToWin)
                    {
                        IEnumerator HowToWin()
                        {
                            yield return new WaitForSeconds(.2f);
                            ShowCurrentStep();
                        }
                        StartCoroutine(HowToWin());
                    }
                    else if (currentTutorialStep == TutorialSteps.World)
                    {
                        ShowCurrentStep();
                    }
                    else if (currentTutorialStep == TutorialSteps.Artifact)
                    {
                        ShowCurrentStep();
                    }
                    else if (currentTutorialStep == TutorialSteps.WaitingForDiscard &&
                           cardModels.Find(cardModel => cardModel.Name == "Void" && cardModel.ZoneId == Constants.HandZone) != null)
                    {
                        currentTutorialStep = TutorialSteps.Discard;
                        ShowCurrentStep();
                    }
                    break;
                }
            case FightCreatureTriggered:
                {
                    if (currentTutorialStep == TutorialSteps.Fight)
                    {
                        HideAllSteps();
                        currentTutorialStep = TutorialSteps.Hint;
                    }
                    break;
                }

            case ReapCreatureTriggered:
                {
                    if (currentTutorialStep == TutorialSteps.Reap)
                    {
                        HideAllSteps();
                        currentTutorialStep = TutorialSteps.World;
                    }
                    break;
                }
            case PlayerStatChangedEvent playerStatChangedEvent:
                if (currentTutorialStep == TutorialSteps.Win && playerStatChangedEvent.Stat.ID == Constants.ShardStat)
                {
                    if (playerStatChangedEvent.Stat.Value == 10)
                    {
                        ShowCurrentStep();
                    }
                }
                break;
        }
    }

    public void ContinueTutorial()
    {
        HideAllSteps();
        currentTutorialStep++;
        if (currentTutorialStep is TutorialSteps.Action or TutorialSteps.EndTurn or TutorialSteps.Bench)
            ShowCurrentStep();
    }

    public void AddCardModels(List<CardModel> _cardModels)
    {
        foreach (var cardModel in _cardModels)
        {
            if (!this.cardModels.ConvertAll(input => input.InstanceId).Contains(cardModel.InstanceId))
            {
                this.cardModels.Add(cardModel);
                eventBusManager.CardEventBus.Subscribe(cardModel.InstanceId, OnEachGameEvent);
            }
        }

        foreach (var thisCardModel in this.cardModels.ToList())
        {
            if (!_cardModels.ConvertAll(input => input.InstanceId).Contains(thisCardModel.InstanceId))
            {
                //this.cardModels.Remove(thisCardModel);
                //eventBusManager.CardEventBus.Unsubscribe(thisCardModel.InstanceId, OnEachGameEvent);
            }
        }
    }
}