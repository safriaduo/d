using System;
using Dawnshard.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawnshard.Views;
using safriaduo.UI;
using UnityEngine;


namespace Dawnshard.Presenters
{
    /// <summary>
    /// This component has the responsability to elaborate the data of the player
    /// model and update the view accordingly
    /// </summary>
    public class PlayerPresenter : IPlayerPresenter
    {
        private readonly IPlayerView playerView;
        private readonly EventBusManager eventBusManager;

        private RankEntryModel currentPlayerRank;
        private int rankPosition;

        public PlayerModel Model { get; set; }

        public IPlayerPresenter Opponent { get; set; }
        
        private bool isMulliganActive = false;

        public PlayerPresenter(IPlayerView view, PlayerModel model, EventBusManager eventBusManager)
        {
            Model = model;
            playerView = view;
            this.eventBusManager = eventBusManager;

            playerView.SelectActiveWorld += SelectActiveWorld;
            playerView.StopTurn += StopTurn;

            eventBusManager.PlayerEventBus.Subscribe(Model.ID, OnGameEventReceived);

            SetStaticFields();
        }

        private void SetStaticFields()
        {
            playerView.SetNickname(Model.Nickname);
            playerView.SetDeckName(Model.Deckname);
            playerView.SetDeckWorlds(Model.WorldIds.ToArray());
        }

        private void OnGameEventReceived(IGameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case TurnStartedEvent turnStarted:
                {
                    Model.IsPlayerTurn = true;
                    SetActivePlayer();
                    break;
                }
                case TurnStoppedEvent turnStopped:
                {
                    Model.IsPlayerTurn = false;
                    SetActivePlayer();
                    break;
                }

                case ActiveWorldChangedEvent activeWorldChanged:
                {
                    OnActiveWorldChanged(activeWorldChanged.WorldId, activeWorldChanged.WorldSelected);
                    break;
                }

                case PlayerStatChangedEvent playerStatChanged:
                {
                    OnPlayerStatChanged(playerStatChanged.Stat, playerStatChanged.PrevValue);
                    break;
                }
                case EndDragEvent endDrag:
                {
                    ToggleDragInteraction(false, endDrag.CardModel);
                    break;
                }
                case StartDragEvent endDrag:
                {
                    ToggleDragInteraction(true, endDrag.CardModel);
                    break;
                }
                case ChangeShardEvent changeShard:
                {
                    OnCardWillChangePlayerStats(changeShard.CardPosition, changeShard.Steal, changeShard.HasShard, changeShard.IsOpponentShard);
                    break;
                }
            }
        }

        public void UpdateView()
        {
            if (Model.IsLocalPlayer)
                ManageMulligan();

            SetStaticFields();
            
            foreach (var stat in Model.Stats)
            {
                playerView.SetStat(stat.ID, stat.OriginalValue, stat.Value);
            }

            //playerView.SetActiveWorld(Model.ActiveWorldId, Model.IsWorldSelected);
        }
        

        private void ManageMulligan()
        {

            if (!Model.IsMulliganCompleted && Model.IsLocalPlayer && !isMulliganActive)
            {
                isMulliganActive = true;
                playerView.ShowMulligan(Model.IsPlayerTurn, true, eventBusManager.GameInteractor.AcceptMulligan);
            }

            if (Model.IsMulliganCompleted && Model.IsOpponentMulliganCompleted && Model.IsLocalPlayer)
            {
                isMulliganActive = false;
                playerView.ShowMulligan(Model.IsPlayerTurn, false, eventBusManager.GameInteractor.AcceptMulligan);
            }
        }

        #region PlayerActions

        public void SelectActiveWorld(string world)
        {
            Debug.Log("Selecting world " + Model.ID + " world " + world);
            eventBusManager.GameInteractor.SelectActiveWorld(world);
        }

        public void StopTurn()
        {
            Debug.Log("Stopping turn " + Model.ID);
            eventBusManager.GameInteractor.StopTurn();
        }
        #endregion

        #region IPlayerPresenter
        public void SetActivePlayer()
        {
            playerView.SetActivePlayer(Model.IsPlayerTurn);
        }

        public void CheckForPlayableCards()
        {
            if (!Model.IsPlayerTurn || !Model.IsLocalPlayer)
                return;
            foreach (var zone in Model.Zones)
            {
                foreach (var card in zone.Cards)
                {
                    if (card.CanBePlayed || card.CanFight || card.CanReap)
                        return;
                }
            }
            playerView.NoMoreAction();
        }

        public void OnActiveWorldChanged(string worldId, bool worldSelected)
        {
            Model.ActiveWorldId = worldId;
            Model.IsWorldSelected = worldSelected;

            Debug.Log("Active world changed " + Model.ID + " world " + worldId + " selected? " + worldSelected);
            playerView.SetActiveWorld(worldId, worldSelected);
        }

        public void OnPlayerStatChanged(StatModel stat, int prevValue)
        {
            var statToUpdate = Model.Stats.Find(s => s.ID == stat.ID);
            if (statToUpdate != null)
            {
                statToUpdate.Value = stat.Value;
                statToUpdate.OriginalValue = stat.OriginalValue;
            }
            else
            {
                Model.Stats.Add(stat);
            }

            Debug.Log("Player stat changed " + Model.ID + " stat " + stat.ID + " prev value " + prevValue + " curr value " + stat.Value);
            playerView.ChangeStat(stat.ID, stat.OriginalValue, stat.Value, prevValue);
        }

        public async Task OnPlayerWon(Action OnEnd, string battlePassId, int battlePassExperience)
        {
            //TODO: uncomment for full release
            if (isMulliganActive)
            {
                Model.IsMulliganCompleted = true;
                Model.IsOpponentMulliganCompleted = true;
                ManageMulligan();
            }
            
            await RankAPI.LoadPlayerRank();
            
            Queue<IEnumerator> coroutinesToPlay = new Queue<IEnumerator>();
            var nextRankEntry = RankAPI.RankEntry;
            var playerRank = currentPlayerRank;
            
            try
            {
                coroutinesToPlay.Enqueue(playerView.WinAnimationCoroutine());
                if (string.IsNullOrEmpty(battlePassId))
                {
                    CoroutineHelper.Instance.StartCoroutineHelper(playerView.PlayCompleteWinAnimation(coroutinesToPlay,OnEnd));
                    return;
                }
                if(GameController.Instance.UserMetadata.CompletedTutorials>1 && battlePassId != null && battlePassExperience != 0)
                    coroutinesToPlay.Enqueue(playerView.BattlePassExpUIAnimationCoroutine(battlePassExperience, battlePassId));
                Debug.Log("subscores: "+playerRank.SubScore+" "+nextRankEntry.SubScore);
                if(!RankAPI.IsPlayerAscendantRank)
                {
                    coroutinesToPlay.Enqueue(playerView.RankUpUIAnimationCoroutine(!PlayerRankWasModified(playerRank, nextRankEntry)?
                            0:CheckForRankUp(playerRank, nextRankEntry)?
                                1:-1,
                        playerRank.CurrentRank, 
                        nextRankEntry.CurrentRank, 
                        playerRank.CurrentLevel, 
                        nextRankEntry.CurrentLevel, 
                        playerRank.SubScore, 
                        CheckForRankUp(playerRank, nextRankEntry)?
                            RankAPI.Ranks.Find(rank => rank.Name==playerRank.CurrentRank).Levels[playerRank.CurrentLevel-1].ExpToLevelUp:
                            RankAPI.Ranks.Find(rank => rank.Name==playerRank.CurrentRank).Levels[nextRankEntry.CurrentLevel-1].ExpToLevelUp)
                    );
                }
                else if (RankAPI.IsPlayerAscendantRank)
                {
                    coroutinesToPlay.Enqueue(playerView.RankPositionUpUIAnimationCoroutine(Model.RankPosition, RankAPI.RankPosition));
                }

                if (TournamentAPI.isWeekendTournamentActive)
                {
                    bool hasLocalPlayerWon = Model.IsLocalPlayer;
                    if(TournamentAPI.playerWins+TournamentAPI.playerLoss+1<=TournamentAPI.MaxScore && GameController.Instance.IsInRankedMatch)
                        coroutinesToPlay.Enqueue(playerView.TournamentPositionUpUIAnimationCoroutine(TournamentAPI.playerWins, TournamentAPI.playerLoss, 
                        hasLocalPlayerWon?TournamentAPI.playerWins+1:TournamentAPI.playerWins, hasLocalPlayerWon?TournamentAPI.playerLoss:TournamentAPI.playerLoss+1));
                }
                
                CoroutineHelper.Instance.StartCoroutineHelper(
                    playerView.PlayCompleteWinAnimation(coroutinesToPlay, OnEnd));
            }
            catch (Exception exception)
            {
                coroutinesToPlay.Clear();
                coroutinesToPlay.Enqueue(playerView.WinAnimationCoroutine());
                Debug.LogException(exception);
                try
                {
                    CoroutineHelper.Instance.StartCoroutineHelper(
                        playerView.PlayCompleteWinAnimation(coroutinesToPlay, OnEnd));
                }
                catch
                {
                    CoroutineHelper.Instance.StartCoroutineHelper(
                        playerView.PlayCompleteWinAnimation(null, OnEnd));
                }
            }
        }

        private bool PlayerRankWasModified(RankEntryModel playerRank, RankEntryModel nextRankEntry)
        {
            return playerRank.CurrentRank != nextRankEntry.CurrentRank || playerRank.CurrentLevel != nextRankEntry.CurrentLevel || playerRank.SubScore != nextRankEntry.SubScore;
        }

        private static bool CheckForRankUp(RankEntryModel playerRank, RankEntryModel nextRankEntry)
        {
            if (playerRank.CurrentRank != nextRankEntry.CurrentRank)
                return true;
            if(nextRankEntry.CurrentLevel > playerRank.CurrentLevel)
                return true;
            if (nextRankEntry.SubScore > playerRank.SubScore)
                return true;
            return false;
        }

        public void OnCardWillChangePlayerStats(Vector3 position, bool isSteal, bool hasShard,
            bool isOpponentShard)
        {
            playerView.SetShardChangedCardPosition(position, isSteal, hasShard, isOpponentShard);
        }

        public Vector3 GetTargetWorldPosition(string effectId)
        {
            return playerView.GetWorldPosition(effectId);
        }

        public void ToggleDragInteraction(bool enable, CardModel model)
        {
            if (!enable)
            {
                playerView.CheckForLayerMaskInteraction(LayerMask.GetMask(LayerMask.LayerToName(0)), false);
                return;
            }
            if (model.ZoneId == Constants.HandZone)
            {
                Debug.Log("HandZoneCheck");
                playerView.CheckForLayerMaskInteraction(LayerMask.GetMask(Constants.GraveyardLayer), true);

            }
            else if (model.ZoneId == Constants.BoardZone)
            {
                playerView.CheckForLayerMaskInteraction(LayerMask.GetMask(Constants.ReapLayer), true);
            }

        }

        public void SetTimer(float seconds)
        {
            playerView.SetTimer(seconds);
        }

        public async void SetRank()
        {
            await RankAPI.LoadPlayerRank();
            currentPlayerRank = RankAPI.RankEntry;
            await GetRankPosition();
            await LoadTpurnamentRatio();
        }
        #endregion

        private async Task GetRankPosition()
        {
            if(RankAPI.IsPlayerAscendantRank)
            {
                await RankAPI.LoadPlayerRank();
                Model.RankPosition = RankAPI.RankPosition;
            }
        }
        
        private async Task LoadTpurnamentRatio()
        {
            if(TournamentAPI.isWeekendTournamentActive)
            {
                await TournamentAPI.LoadPlayerRecord();
            }
        }
    }
}