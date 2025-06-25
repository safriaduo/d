using Dawnshard.Database;
using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace Dawnshard.Network
{
    /// <summary>
    /// The event dispatcher receives the data sent by the server and translate
    /// it to models which can be communicated to the presenters.
    /// It's the only component that can update the models of the game
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        private IGamePresenter gamePresenter;
        private readonly EventBusManager eventBusManager;
        private Queue<Action> gameStateActionQueue = new Queue<Action>();
        public static GameStateModel GameState { get; private set; }

        public enum GameEventType
        {
            GameStart,
            GameEnd,
            GameUpdateStart,
            GameUpdateEnd,
            GameStateUpdate
        }

        public Action<GameEventType, GameUpdate> onGameUpdate;

        public EventDispatcher(EventBusManager eventBusManager)
        {
            this.eventBusManager = eventBusManager;
        }

        public void RegisterGamePresenter(IGamePresenter presenter)
        {
            gamePresenter = presenter;
            presenter.Initialize(eventBusManager);
        }

        public void RegisterEventCatcher(EventCatcher eventCatcher)
        {
            eventCatcher.SetGamePresenter(gamePresenter);
            onGameUpdate += eventCatcher.OnGameUpdate;
        }

        public void OnGameStart(GameStart response)
        {
            IEnumerator wait()
            {
                yield return new WaitUntil(() => gamePresenter != null);
                yield return null;
                GameStart(response);
            }

            if (gamePresenter != null)
            {
                GameStart(response);
            }
            else
            {
                CoroutineHelper.Instance.StartCoroutineHelper(wait());
            }
        }

        private void GameStart(GameStart response)
        {
            UpdateGameView(response.GameState, true);

            onGameUpdate?.Invoke(GameEventType.GameStart, null);

            gamePresenter.GetBothPlayers().ForEach((player) =>
            {
                player.SetTimer(response.TurnDuration);
                player.SetRank();
            });
        }

        public void OnGameStateUpdate(GameStateUpdate response)
        {
            IEnumerator wait()
            {
                yield return new WaitUntil(() => gamePresenter != null);
                yield return null;
                GameStateUpdate(response);
            }

            if (gamePresenter != null)
            {
                GameStateUpdate(response);
            }
            else
            {
                CoroutineHelper.Instance.StartCoroutineHelper(wait());
            }
        }

        private void GameStateUpdate(GameStateUpdate response)
        {
            gameStateActionQueue.Enqueue(() =>
            {
                UpdateGameView(response.GameState);
                onGameUpdate?.Invoke(GameEventType.GameStateUpdate, null);
            });
            gameStateActionQueue.Dequeue()?.Invoke();
        }

        public void OnGameEnd(GameEnd response)
        {
            gamePresenter.GetPlayer(response.WinnerId).OnPlayerWon(GameController.Instance.EndMatch,
                response.BattlePassId, response.BattlePassExp);
        }

        public void OnOpponentLeft(GameEnd response)
        {
            //TODO: mettere l'exp nell'opponent left
            gamePresenter.GetPlayer(GameController.Instance.LocalPlayerId).OnPlayerWon(GameController.Instance.EndMatch, response.BattlePassId, response.BattlePassExp);
            onGameUpdate?.Invoke(GameEventType.GameEnd, null);
        }

        public void OnGameUpdate(GameUpdate response)
        {
            if (response.Error != null)
            {
                //TODO: ci va una view che dice che cosa non va bene
                return;
            }
            gameStateActionQueue.Enqueue(() => UpdateGameModel(response.GameState));
            CoroutineHelper.Instance.StartCoroutine(PerformResponse(response));
        }

        /// <summary>
        /// This coroutine is used to create a chain of animations and 
        /// wait for their completion and updates the modells
        /// </summary>
        private IEnumerator PerformResponse(GameUpdate response)
        {
            onGameUpdate?.Invoke(GameEventType.GameUpdateStart, null);

            UpdateLogModels(response.GameState);

            GameInteractor.Lock(true);

            foreach (var gameEvent in response.Events)
            {
                yield return PerformGameEvents(gameEvent);
            }

            gameStateActionQueue.Dequeue()?.Invoke();

            UpdateCardsTargets(response);

            UpdateBoard(response);

            gamePresenter.ResponseElaborationEnded();

            onGameUpdate?.Invoke(GameEventType.GameUpdateEnd, response);

            GameInteractor.Lock(false);
        }

        private void UpdateBoard(GameUpdate response)
        {
            var currentPlayerBoardInstanceId = response.GameState.CurrentPlayer.Zones.ToList()
                .Find(zoneDTO => zoneDTO.ZoneID == Constants.BoardZone).InstanceID;
            var currentOpponentBoardInstanceId = response.GameState.CurrentOpponent.Zones.ToList()
                .Find(ZoneDTO => ZoneDTO.ZoneID == Constants.BoardZone).InstanceID;
            eventBusManager.ZoneEventBus.Publish(currentPlayerBoardInstanceId, new UpdateBoardEvent());
            eventBusManager.ZoneEventBus.Publish(currentOpponentBoardInstanceId, new UpdateBoardEvent());
        }

        private void UpdateCardsTargets(GameUpdate response)
        {
            foreach (var cardDTO in response.GameState.CurrentOpponent.Zones.ToList().SelectMany(input => input.Cards).ToList())
            {
                eventBusManager.CardEventBus.Publish(
                    cardDTO.InstanceID,
                    new CardTargetsUpdateEvent(UnmarshalAvailableTargets(cardDTO.AvailableTargets.ToArray()))
                    );
            }
            foreach (var cardDTO in response.GameState.CurrentPlayer.Zones.ToList().SelectMany(input => input.Cards).ToList())
            {
                eventBusManager.CardEventBus.Publish(
                    cardDTO.InstanceID,
                    new CardTargetsUpdateEvent(UnmarshalAvailableTargets(cardDTO.AvailableTargets.ToArray()))
                );
            }
        }


        /// <summary>
        /// Perform the game events that happened after an update and 
        /// change the models accordingly
        /// </summary>
        private IEnumerator PerformGameEvents(GameEvent gameEvent)
        {
            switch (gameEvent.EventCase)
            {
                case GameEvent.EventOneofCase.None:
                    break;

                case GameEvent.EventOneofCase.ZoneCardAdded:
                    {
                        CardModel cardModel = gameEvent.ZoneCardAdded.CardData != null ? UnmarshalCard(gameEvent.ZoneCardAdded.CardData, gameEvent.ZoneCardAdded.ZoneId) : null;

                        eventBusManager.ZoneEventBus.Publish(gameEvent.ZoneCardAdded.ZoneInstanceId,
                            new ZoneCardAddedEvent(gameEvent.ZoneCardAdded.CardInstanceId, cardModel));

                        break;
                    }
                case GameEvent.EventOneofCase.ZoneCardRemoved:
                    {
                        eventBusManager.ZoneEventBus.Publish(gameEvent.ZoneCardRemoved.ZoneInstanceId,
                            new ZoneCardRemovedEvent(gameEvent.ZoneCardRemoved.CardId));
                        break;
                    }
                case GameEvent.EventOneofCase.CardMovedTriggered:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.CardMovedTriggered.CardId,
                            new CardMovedEvent(gameEvent.CardMovedTriggered.OrigZoneId, gameEvent.CardMovedTriggered.DestZoneId, gameEvent.CardMovedTriggered.CardId));

                        if (gameEvent.CardMovedTriggered.DestZoneId == Constants.GraveyardZone && gameEvent.CardMovedTriggered.OrigZoneId != Constants.ActionZone)
                            break;
                        if (gameEvent.CardMovedTriggered.DestZoneId != Constants.GraveyardZone ||
                            gameEvent.CardMovedTriggered.OrigZoneId == Constants.ActionZone)
                        {
                            var cardPresenter = ZonePresenter.GetCardPresenter(gameEvent.CardMovedTriggered.CardId);
                            var duration = cardPresenter?.GetMoveDuration(gameEvent.CardMovedTriggered.OrigZoneId,
                                gameEvent.CardMovedTriggered.DestZoneId) ?? 0f;
                            if (duration > 0f)
                                yield return new WaitForSeconds(duration);
                        }

                        break;
                    }

                case GameEvent.EventOneofCase.CardStatChanged:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.CardStatChanged.CardId,
                            new CardStatChangedEvent(new StatModel(gameEvent.CardStatChanged.Stat), gameEvent.CardStatChanged.PrevValue));

                        break;
                    }

                case GameEvent.EventOneofCase.PlayerStatChanged:
                    {
                        eventBusManager.PlayerEventBus.Publish(gameEvent.PlayerStatChanged.PlayerId,
                             new PlayerStatChangedEvent(new StatModel(gameEvent.PlayerStatChanged.Stat), gameEvent.PlayerStatChanged.PrevValue));

                        break;
                    }

                case GameEvent.EventOneofCase.CardKeywordChanged:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.CardKeywordChanged.CardId,
                            new CardKeywordChangedEvent(gameEvent.CardKeywordChanged.KeywordId, gameEvent.CardKeywordChanged.Added));

                        break;
                    }

                case GameEvent.EventOneofCase.CardReadyChanged:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.CardReadyChanged.CardId,
                            new CardReadyChangedEvent(gameEvent.CardReadyChanged.CanFight, gameEvent.CardReadyChanged.CanReap));

                        var readyPresenter = ZonePresenter.GetCardPresenter(gameEvent.CardReadyChanged.CardId);
                        var readyDuration = readyPresenter?.ReadyChangeDuration ?? 0f;
                        if (readyDuration > 0f)
                            yield return new WaitForSeconds(readyDuration);

                        break;
                    }

                case GameEvent.EventOneofCase.ActiveWorldChanged:
                    {
                        eventBusManager.PlayerEventBus.Publish(gameEvent.ActiveWorldChanged.PlayerId,
                            new ActiveWorldChangedEvent(gameEvent.ActiveWorldChanged.WorldId, gameEvent.ActiveWorldChanged.WorldSelected));

                        break;
                    }

                case GameEvent.EventOneofCase.CardAbilityTriggered:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.CardAbilityTriggered.SourceCardId,
                            new CardAbilityTriggeredEvent(new AbilityModel(gameEvent.CardAbilityTriggered.Ability), gameEvent.CardAbilityTriggered.TargetCardIds.ToList(), gameEvent.CardAbilityTriggered.SourceCardId));

                        break;
                    }

                case GameEvent.EventOneofCase.PlayerAbilityTriggered:
                    {
                        eventBusManager.CardEventBus?.Publish(gameEvent.PlayerAbilityTriggered.SourceCardId,
                            new PlayerAbilityTriggeredEvent(gameEvent.PlayerAbilityTriggered.Ability.TriggerID, gameEvent.PlayerAbilityTriggered.Ability.EffectID, gameEvent.PlayerAbilityTriggered.SourceCardId, gameEvent.PlayerAbilityTriggered.TargetPlayerIds.ToList()));

                        var abilityPresenter = ZonePresenter.GetCardPresenter(gameEvent.PlayerAbilityTriggered.SourceCardId);
                        var abilityDuration = abilityPresenter?.AbilityDuration ?? 0f;
                        if (abilityDuration > 0f)
                            yield return new WaitForSeconds(abilityDuration);

                        break;
                    }

                case GameEvent.EventOneofCase.CardWorldChanged:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.CardWorldChanged.CardId,
                            new CardWorldChangedEvent(gameEvent.CardWorldChanged.WorldId));

                        break;
                    }
                case GameEvent.EventOneofCase.ReapCreatureEvent:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.ReapCreatureEvent.CardId,
                            new ReapCreatureTriggered(gameEvent.ReapCreatureEvent.CardId));

                        var reapPresenter = ZonePresenter.GetCardPresenter(gameEvent.ReapCreatureEvent.CardId);
                        var reapDuration = reapPresenter?.ReapDuration ?? 0f;
                        if (reapDuration > 0f)
                            yield return new WaitForSeconds(reapDuration);

                        break;
                    }

                case GameEvent.EventOneofCase.FightCreatureEvent:
                    {
                        eventBusManager.CardEventBus.Publish(gameEvent.FightCreatureEvent.DefenderId,
                            new FightCreatureTriggered(gameEvent.FightCreatureEvent.AttackerId, gameEvent.FightCreatureEvent.DefenderId));

                        var fightPresenter = ZonePresenter.GetCardPresenter(gameEvent.FightCreatureEvent.AttackerId);
                        var fightDuration = fightPresenter?.FightDuration ?? 0f;
                        if (fightDuration > 0f)
                            yield return new WaitForSeconds(fightDuration);

                        break;
                    }

                default:
                    {
                        Debug.LogError("Event case not supported: " + gameEvent.EventCase);
                        break;
                    }
            }
        }

        private void UpdateGameModel(GameStateDTO gameState)
        {
            GameState = UnmarshalGameState(gameState);
            gamePresenter.Model = GameState;
            gamePresenter.UpdateModel();
        }

        private void UpdateGameView(GameStateDTO gameState, bool updateAllPlayers = false)
        {
            GameState = UnmarshalGameState(gameState);

            if (!updateAllPlayers)
            {
                eventBusManager.PlayerEventBus.Publish(gameState.CurrentPlayer.ID, new TurnStartedEvent());
                eventBusManager.PlayerEventBus.Publish(gameState.CurrentOpponent.ID,
                    new TurnStoppedEvent(gameState.CurrentOpponent.ID == GameState.localPlayer.ID));
            }
            gamePresenter.Model = GameState;
            gamePresenter.UpdateView(updateAllPlayers);

        }

        private void UpdateLogModels(GameStateDTO gameState)
        {
            GameState = UnmarshalGameState(gameState);
            gamePresenter.Model = GameState;
            gamePresenter.UpdateLogModels();
        }

        /// <summary>
        /// Updates the game state by calling the update view methods
        /// in each presenters with new data 
        /// </summary>
        private GameStateModel UnmarshalGameState(GameStateDTO gameState)
        {
            var gameStateModel = new GameStateModel();

            var currentPlayer = UnmarshalPlayer(gameState.CurrentPlayer, true, gameState.CurrentOpponent.MulliganCompleted);
            var currentOpponent = UnmarshalPlayer(gameState.CurrentOpponent, false, gameState.CurrentPlayer.MulliganCompleted);

            gameStateModel.localPlayer = currentPlayer.ID == GameController.Instance.LocalPlayerId ? currentPlayer : currentOpponent;
            gameStateModel.remoteOpponent = currentPlayer.ID == GameController.Instance.LocalPlayerId ? currentOpponent : currentPlayer;

            return gameStateModel;
        }


        #region Unmarshaling
        private PlayerModel UnmarshalPlayer(PlayerDTO playerDTO, bool isPlayerTurn, bool isOpponentMulliganCompleted)
        {
            Debug.Log("player unmarshaled with world " + playerDTO.ActiveWorldId);
            var playerModel = new PlayerModel()
            {
                ID = playerDTO.ID,
                Nickname = playerDTO.Username,
                Deckname = playerDTO.DeckName,
                IsPlayerTurn = isPlayerTurn,
                IsOpponentMulliganCompleted = isOpponentMulliganCompleted,
                Stats = UnmarshalStats(playerDTO.Stats),
                IsWorldSelected = playerDTO.IsWorldSelected,
                IsMulliganCompleted = playerDTO.MulliganCompleted,
                ActiveWorldId = playerDTO.ActiveWorldId,
                WorldIds = playerDTO.WorldIds.ToList(),
                Zones = new()
            };

            foreach (var zone in playerDTO.Zones)
            {
                playerModel.Zones.Add(UnmarshalZone(zone));
            }

            return playerModel;
        }

        private ZoneModel UnmarshalZone(ZoneDTO zoneDTO)
        {
            var cardModels = new List<CardModel>();

            foreach (var card in zoneDTO.Cards)
            {
                cardModels.Add(UnmarshalCard(card, zoneDTO.ZoneID));
            }

            return new ZoneModel()
            {
                InstanceId = zoneDTO.InstanceID,
                MaxCards = zoneDTO.MaxCards,
                NumCards = zoneDTO.NumCards,
                ZoneId = zoneDTO.ZoneID,
                IsStaticZone = zoneDTO.ZoneVisibility == 2, //Static zone
                Cards = cardModels,
            };
        }

        private CardModel UnmarshalCard(CardDTO card, string zoneId)
        {
            CardModel cardRecord = CardDatabase.GetCardById(card.ID);

            if (cardRecord == null)
            {
                Debug.LogError("Cannot find card with id " + card.ID);
                return null;
            }

            if (card == null || gamePresenter == null || (gamePresenter.GetPlayer(card.OwnerID) != null && gamePresenter.GetPlayer(card.OwnerID).Model == null))
            {
                return null;
            }

            bool isOwnerTurn = gamePresenter.GetPlayer(card.OwnerID) != null && gamePresenter.GetPlayer(card.OwnerID).Model.IsPlayerTurn;

            var unmarshaledAbilities = UnmarshalAbilities(card.Abilities);

            for (int i = 0; i < unmarshaledAbilities.Count; i++)
            {
                unmarshaledAbilities[i].EffectParameter = cardRecord.Abilities[i].EffectParameter;
            }

            var cardModel = new CardModel()
            {
                //Dynamic properties
                ID = card.ID,
                InstanceId = card.InstanceID,
                CanFight = card.CanFight,
                CanReap = card.CanReap,
                CanBePlayed = card.CanPlay,
                Abilities = unmarshaledAbilities,
                Keywords = card.Keywords.ToList(),
                Stats = UnmarshalStats(card.Stats),
                OwnerId = card.OwnerID,
                IsOwnerTurn = isOwnerTurn,

                AvailableTargetIds = UnmarshalAvailableTargets(card.AvailableTargets.ToArray()),
                IncandescenseLevel = card.Incandescense,
                ZoneId = zoneId,

                //Static properties,
                ArtworkPath = cardRecord.ArtworkPath,
                WorldId = cardRecord.WorldId,
                BodyText = cardRecord.BodyText,
                Name = cardRecord.Name,
                Rarity = cardRecord.Rarity,
                Type = cardRecord.Type,
            };

            return cardModel;
        }

        private Dictionary<string, List<int>> UnmarshalAvailableTargets(AvailableTargetsDTO[] availableTargets)
        {
            var dict = new Dictionary<string, List<int>>();

            foreach (var targetsDTO in availableTargets)
            {
                dict.Add(targetsDTO.ActionID, targetsDTO.AvailableTargetIDs.ToList());
            }

            return dict;
        }

        private List<StatModel> UnmarshalStats(IEnumerable<StatDTO> stats)
        {
            List<StatModel> statModels = new();

            foreach (var stat in stats)
            {
                statModels.Add(UnmarshalStat(stat));
            }

            return statModels;
        }

        private StatModel UnmarshalStat(StatDTO stat)
        {
            return new StatModel
            {
                ID = stat.ID,
                OriginalValue = stat.OriginalValue,
                Value = stat.Value,
            };
        }

        private List<AbilityModel> UnmarshalAbilities(IEnumerable<AbilityDTO> abilities)
        {
            var abilitiesModel = new List<AbilityModel>();

            foreach (var ability in abilities)
            {
                abilitiesModel.Add(UnmarshalAbility(ability));
            }

            return abilitiesModel;
        }

        private AbilityModel UnmarshalAbility(AbilityDTO ability)
        {
            return new AbilityModel
            {
                EffectId = ability.EffectID,
                TriggerId = ability.TriggerID,
            };
        }

        #endregion

    }
}