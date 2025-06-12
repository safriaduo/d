using System;
using Dawnshard.Network;
using Dawnshard.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dawnshard.Presenters
{
    public class GamePresenter : MonoBehaviour, IGamePresenter
    {
        [Header("User Card Refs")]
        [SerializeField] private UserCardView userCardPrefab;
        [SerializeField] private Transform userCardParent;

        [Header("Opponent Card Refs")]
        [SerializeField] private CardView opponentCardPrefab;
        [SerializeField] private Transform opponentCardsParent;

        [Header("Client Refs")]
        [SerializeField] private PlayerView localPlayerView;
        [SerializeField] private UnityDictionary<ZoneView> localZonesById;

        [Header("Opponent Player Refs")]
        [SerializeField] private PlayerView opponentPlayerView;
        [SerializeField] private UnityDictionary<ZoneView> opponentZonesById;
        
        [Header("Utils")]
        [SerializeField] private CombactLogManager combactLogManager;
        [SerializeField] private TutorialManager tutorialManager;

        private EventBusManager eventBusManager;

        public static readonly Dictionary<int, IZonePresenter> zonePresenters = new();

        public static readonly Dictionary<string, IPlayerPresenter> playerPresenters = new();

        private bool firstTimeMulliganCompleted=true;

        public GameStateModel Model { get; set; }

        private void OnDestroy()
        {
            zonePresenters.Clear();
            playerPresenters.Clear();
        }

        private void Awake()
        {
            GameController.Instance.EventDispatcher.RegisterGamePresenter(this);
        }

        private void Start()
        {
            ZonePresenter.ClearPresenters();
        }

        #region IGamePresenter

        public void Initialize(EventBusManager eventBusManager)
        {
            this.eventBusManager = eventBusManager;
            combactLogManager.Initialize(eventBusManager);
            tutorialManager.Initialize(eventBusManager);
        }

        public IZonePresenter GetZone(int instanceId)
        {
            return zonePresenters.GetValueOrDefault(instanceId, null);
        }

        public IPlayerPresenter GetPlayer(string id)
        {
            return playerPresenters.GetValueOrDefault(id, null);
        }

        public IPlayerPresenter CreatePlayer(PlayerModel playerModel)
        {
            PlayerPresenter presenter;

            if (playerModel.IsLocalPlayer)
            {
                presenter = new PlayerPresenter(localPlayerView, playerModel, eventBusManager);
            }
            else
            {
                presenter = new PlayerPresenter(opponentPlayerView, playerModel, eventBusManager);
            }

            playerPresenters[presenter.Model.ID] = presenter;
            
            combactLogManager.AddPlayer(presenter.Model.ID);
            if(playerModel.IsLocalPlayer)
                tutorialManager.AddPlayer(presenter.Model.ID);

            return presenter;
        }

        public IZonePresenter CreateZone(ZoneModel zoneModel, string playerId)
        {
            var zoneDict = playerId == GameController.Instance.LocalPlayerId ? localZonesById : opponentZonesById;

            ZoneView zoneView = zoneDict.GetItem(zoneModel.ZoneId);

            if (zoneView == null)
            {
                return null;
            }

            var presenter = new ZonePresenter(zoneModel, zoneView, eventBusManager);

            zonePresenters[zoneModel.InstanceId] = presenter;

            return presenter;
        }

        public List<IPlayerPresenter> GetBothPlayers()
        {
            return playerPresenters.Values.ToList();
        }

        public void ResponseElaborationEnded()
        {
            foreach (var zoneId in zonePresenters.Keys)
            {
                eventBusManager.ZoneEventBus.Publish(zoneId, new ResponseElaborationEndedEvent());
            }
        }

        public void UpdateLogModels()
        {
            List<CardModel> cardModels = new List<CardModel>();
            foreach (var zoneModel in Model.localPlayer.Zones)
            {
                cardModels.AddRange(zoneModel.Cards);
            }
            foreach (var zoneModel in Model.remoteOpponent.Zones)
            {
                cardModels.AddRange(zoneModel.Cards);
            }
            combactLogManager.AddCardModels(cardModels);             
            tutorialManager.AddCardModels(cardModels); 

        }

        public void UpdateModel()
        {
            var localPlayerPresenter = GetPlayer(Model.localPlayer.ID) ?? CreatePlayer(Model.localPlayer);
            var remotePlayerPresenter = GetPlayer(Model.remoteOpponent.ID) ?? CreatePlayer(Model.remoteOpponent);
            
            UpdatePlayerModels(localPlayerPresenter, Model.localPlayer);
            UpdatePlayerModels(remotePlayerPresenter, Model.remoteOpponent);

            localPlayerPresenter.CheckForPlayableCards();
        }

        public void UpdateView(bool updateAllPlayers)
        {
            var localPlayerPresenter = GetPlayer(Model.localPlayer.ID)?? CreatePlayer(Model.localPlayer);
            var remotePlayerPresenter = GetPlayer(Model.remoteOpponent.ID)?? CreatePlayer(Model.remoteOpponent);
            if (updateAllPlayers || firstTimeMulliganCompleted)
            {
                UpdatePlayerPresenter(localPlayerPresenter, Model.localPlayer);
                UpdatePlayerPresenter(remotePlayerPresenter, Model.remoteOpponent);
                if (Model.localPlayer.IsMulliganCompleted && Model.localPlayer.IsOpponentMulliganCompleted)
                {
                    firstTimeMulliganCompleted = false;
                }
            }
            else if(Model.localPlayer.IsPlayerTurn)
                UpdatePlayerPresenter(localPlayerPresenter, Model.localPlayer);
            else if(Model.remoteOpponent.IsPlayerTurn)
                UpdatePlayerPresenter(remotePlayerPresenter, Model.remoteOpponent);

        }

        private void UpdatePlayerPresenter(IPlayerPresenter playerPresenter, PlayerModel playerModel)
        {
            playerPresenter.Model = playerModel;
            foreach (var zoneModel in playerPresenter.Model.Zones)
            {
                var zonePresenter = GetZone(zoneModel.InstanceId) ?? CreateZone(zoneModel, playerPresenter.Model.ID);
                zonePresenter.Model = zoneModel;
                zonePresenter.UpdateView();
            }
            playerPresenter.UpdateView();
        }
        
        private void UpdatePlayerModels(IPlayerPresenter playerPresenter, PlayerModel playerModel)
        {
            playerPresenter.Model = playerModel;
            foreach (var zoneModel in playerModel.Zones)
            {
                var zonePresenter = GetZone(zoneModel.InstanceId) ?? CreateZone(zoneModel, playerModel.ID);
                zonePresenter.Model = zoneModel;
                zonePresenter.UpdateCardsModels();
            }
        }

        #endregion
    }
}
