using System;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Presenters;
using Dawnshard.Views;
using Unity.VisualScripting;
using UnityEngine;

namespace Dawnshard.Network
{
    public class CombactLogManager : MonoBehaviour
    {
        [SerializeField] private Transform entryLogParent;
        [SerializeField] private Transform extendedLogParent;
        [SerializeField] private UnityDictionary<Sprite> actionSprites;

        private bool isLoggable;
        private List<CardModel> cardModels = new();
        private EventBusManager eventBusManager;

        private bool firstActionTaken = false;
        public void Initialize(EventBusManager eventBusManager)
        {
            this.eventBusManager = eventBusManager;
        }

        public void AddPlayer(string playerId)
        {
            eventBusManager.PlayerEventBus.Subscribe(playerId,OnEachGameEvent);
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
        
        protected void Start()
        {
            LogFactory.Instance.SetDefaultExtendedLogParent(extendedLogParent);
        }

        private void OnEachGameEvent(IGameEvent gameEvent)
        {
            isLoggable = IsALoggableEntry(gameEvent);
            if(!isLoggable)
                return;
            LogModel logEntry = new(null, null, null, LogModel.LogEntryActionType.Error, false);

            logEntry = TransformGameEventToLogEntry(gameEvent);
            
            if (logEntry != null && logEntry.LogEntryType != LogModel.LogEntryActionType.Error)
            {
                firstActionTaken = true;
                CreateLogView(logEntry);
            }
        }

        private void CreateLogView(LogModel logEntry)
        {
            logEntry.ActionIcon = actionSprites.GetItem(logEntry.LogEntryType.ToString());
            logEntry.ActionText = logEntry.LogEntryType.ToString();
            CreateMiniLogEntry(logEntry);
        }

        private void CreateMiniLogEntry(LogModel logEntry)
        {
            if (logEntry.SourceCard != null)
            {
                LogFactory.Instance.CreateSourceCardLog(logEntry, entryLogParent);
            }
            else
            {
                LogFactory.Instance.CreateLog(logEntry, entryLogParent);
            }
        }

        /// <summary>
        /// Transform the game event in a log entry
        /// </summary>
        private LogModel TransformGameEventToLogEntry(IGameEvent gameEvent)
        {

            LogModel logEntry = null;
            switch (gameEvent)
            {
                case FightCreatureTriggered fightCreature:
                    {
                        var sourceCard = cardModels.Find(card=>card.InstanceId==fightCreature.AttackerId);
                        var target = cardModels.Find(card=>card.InstanceId==fightCreature.DefenderId);

                        if (sourceCard != null && target != null)
                        {
                            logEntry = new LogModel(
                                sourceCard,
                                new List<CardModel>() { target },
                                null,
                                LogModel.LogEntryActionType.Fight, sourceCard.IsOwnerLocalPlayer);
                        }

                        break;
                    }
                case ReapCreatureTriggered reapCreature:
                    {
                        var sourceCard = cardModels.Find(card=>card.InstanceId==reapCreature.ReapCardId);

                        if (sourceCard != null)
                        {
                            logEntry = new LogModel(
                                sourceCard,
                                null,
                                null,
                                LogModel.LogEntryActionType.Reap,
                                sourceCard.IsOwnerLocalPlayer);
                        }

                        break;
                    }
                case CardMovedEvent cardMoved:
                    {
                        //TODO: aggiungere il discard
                        var sourceCard = cardModels.Find(card=>card.InstanceId==cardMoved.CardInstanceId);
                        
                        if (sourceCard != null)
                        {
                            if (cardMoved.OrigZoneId != Constants.HandZone)
                                break;
                            if(cardMoved.DestZoneId == Constants.GraveyardZone)
                                logEntry = new LogModel(
                                    sourceCard,
                                    null,
                                    null,
                                    LogModel.LogEntryActionType.Discard,
                                    sourceCard.IsOwnerLocalPlayer);
                            else if (cardMoved.DestZoneId is Constants.BoardZone or Constants.ActionZone )
                                logEntry = new LogModel(
                                    sourceCard,
                                    null,
                                    null,
                                    LogModel.LogEntryActionType.Play,
                                    sourceCard.IsOwnerLocalPlayer);
                        }

                        break;
                    }
                //TODO: vedere se questi servono
                /*case ActionResponse.ActionOneofCase.SelectActiveWorldResponse:
                {
                    logEntry = new LogModel(
                        null,
                        null,
                        new List<PlayerModel>()
                        {
                            gamePresenter.GetBothPlayers()
                                .Find(playerPresenter => playerPresenter.Model.IsPlayerTurn == true).Model
                        },
                        LogModel.LogEntryActionType.SelectWorld,
                        gamePresenter.GetBothPlayers().Find(playerP => playerP.Model.IsPlayerTurn).Model.IsLocalPlayer);
                    break;
                }*/
                case TurnStoppedEvent turnsStopped:
                {
                    if (!firstActionTaken)
                        break;
                    logEntry = new LogModel(null, null, null, LogModel.LogEntryActionType.StopTurn,
                        turnsStopped.IsLocalPlayer);
                    break;
                }
                case CardAbilityTriggeredEvent cardAbilityTriggered:
                    {
                        CardModel sourceCard =
                            cardModels.Find(card=>card.InstanceId==cardAbilityTriggered.CardInstanceId);
                        List<CardModel> targerCards = new();
                        foreach (int id in cardAbilityTriggered.TargetIds)
                        {
                            targerCards.Add(cardModels.Find(card=>card.InstanceId==id));
                        }

                        logEntry = new LogModel(sourceCard, targerCards, null, LogModel.LogEntryActionType.AbilityOnCard,
                            sourceCard.IsOwnerLocalPlayer);
                        break;
                    }
                case PlayerAbilityTriggeredEvent playerAbilityTriggered:
                    {
                        CardModel sourceCard =
                            cardModels.Find(card=>card.InstanceId==playerAbilityTriggered.CardInstanceId);
                        List<PlayerModel> targetPlayers = new();
                        //foreach (string id in playerAbilityTriggered.PlayerTargetIds)
                        //{
                            //targetPlayers.Add(gamePresenter.GetPlayer(id).Model);
                        //}

                        logEntry = new LogModel(sourceCard, null, targetPlayers,
                            LogModel.LogEntryActionType.AbilityOnPlayer,
                            sourceCard.IsOwnerLocalPlayer);
                        break;
                    }
                default:
                    {
                        logEntry = new LogModel(null, null, null, LogModel.LogEntryActionType.Error, false);
                        break;
                    }
            }
            return logEntry;
        }

        /// <summary>
        /// Return whether or not the game event will create a log entry
        /// </summary>
        private bool IsALoggableEntry(IGameEvent gameEvent)
        {
            return gameEvent is CardAbilityTriggeredEvent or PlayerAbilityTriggeredEvent or CardMovedEvent or ReapCreatureTriggered or FightCreatureTriggered or TurnStoppedEvent ;
        }
    }
}