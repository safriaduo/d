using Dawnshard.Presenters;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Menu;
using Dawnshard.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// The player view has the responsability to expose easy to access method
    /// to show the player's data and to handle the interactions of the user
    /// </summary>
    public class PlayerView : MonoBehaviour, IPlayerView
    {
        [Header("Base UI")]
        [SerializeField] private TMP_Text nicknameText;
        [SerializeField] private DeckView deckView;
        [SerializeField] private HoverPopup[] setHintPopups;
        [SerializeField] private SelectWorldView selectWorldView;
        [SerializeField] private Transform defaultEffectTransform;
        [SerializeField] private TimerView timer;
        [SerializeField] private MMFeedbacks changeTurnUIAnimation;
        [SerializeField] protected TMP_Text turnText;

        [Header("Dynamic UI")]
        [SerializeField] protected UnityDictionary<StatView> statsById;
        [SerializeField] private UnityDictionary<Transform> effectTransformById;
        [SerializeField] private Transform gameBoardTransform;
        [SerializeField] private MulliganView mulliganUI;
        [SerializeField] private PostBattleBattlePassView postBattleBattlePassView;
        [SerializeField] private RankView rankView;


        [Header("InteractionAreas")]
        [SerializeField] private ReapAreaInteraction reapAreaInteraction;
        [SerializeField] private DiscardAreaInteraction discardAreaInteraction;
        
        [SerializeField] private ShaderPropertyAnimator boardColorAnimator;
        
        [SerializeField] protected GameSceneAnalytics gameSceneAnalytics;


        private bool checkLayer = false;
        private bool animActive = false;
        private bool wasPlayerTurn = false;
        protected string changeTurnString = "OPPONENT TURN";
        private LayerMask layerToCheck;

        private void Update()
        {
            if (!checkLayer || reapAreaInteraction == null || discardAreaInteraction == null)
                return;
            InteractionAreaAnimations();
        }

        private void InteractionAreaAnimations()
        {
            if (layerToCheck == LayerMask.GetMask(Constants.GraveyardLayer))
            {
                if (RaycastHitOnGivenLayer(layerToCheck) && !animActive)
                {
                    discardAreaInteraction.Grow();
                    animActive = true;
                }
                else if (!RaycastHitOnGivenLayer(layerToCheck) && animActive)
                {
                    discardAreaInteraction.Shrink();
                    animActive = false;
                }
            }
            else if (layerToCheck == LayerMask.GetMask(Constants.ReapLayer))
            {
                if (RaycastHitOnGivenLayer(layerToCheck) && !animActive)
                {
                    reapAreaInteraction.PlayReapHover();
                    animActive = true;
                }
                else if (!RaycastHitOnGivenLayer(layerToCheck) && animActive)
                {
                    reapAreaInteraction.StopReapHover();
                    animActive = false;

                }
            }
        }

        #region IPlayerView
        public Action<string> SelectActiveWorld { get; set; }

        public Action StopTurn { get; set; }

        public virtual void ChangeStat(string id, int originalValue, int value, int prevValue)
        {
            UpdateStat(id, prevValue, value);
            if (id == Constants.ShardStat)
            {
                gameSceneAnalytics.SetOpponentPlayerShard(value);
            }
        }

        public void SetStat(string id, int originalValue, int value)
        {
            UpdateStat(id, originalValue, value);
        }

        public Vector3 GetWorldPosition(string effectID)
        {
            var item = effectTransformById.GetItem(effectID);

            if (item != null)
                return item.position;
            if (defaultEffectTransform == null)
            {
                Debug.LogWarning("defaultEffectTransform is null");
                return Vector3.zero;
            }
            return defaultEffectTransform.position;
        }

        public virtual void SetActivePlayer(bool isPlayerTurn)
        {
            if (isPlayerTurn != wasPlayerTurn)
            {
                (statsById.GetItem(Constants.ShardStat) as ShardStatView).ChangeActivePlayer(isPlayerTurn);
                if (isPlayerTurn)
                {
                    turnText.text = changeTurnString;
                    changeTurnUIAnimation.PlayFeedbacks();
                    boardColorAnimator.StartAnimation();
                }
                else
                {
                    boardColorAnimator.StartAnimation(true);
                }
            }
            timer.ToggleTimer(isPlayerTurn);
            wasPlayerTurn = isPlayerTurn;

            //if (!isPlayerTurn) {
            //    StartCoroutine(SetActiveWorld(string.Empty, false));
            //}
        }

        public void SetNickname(string nickname)
        {
            nicknameText.text = nickname;
        }

        public void SetDeckName(string deckName)
        {
            deckView.SetName(deckName);
        }

        public virtual void SetDeckWorlds(string[] worldIds)
        {
            deckView.SetWorlds(worldIds);
            int i = 0;
            foreach (var setHintPopup in setHintPopups)
            {
                if (i >= worldIds.Length)
                {
                    setHintPopup.SetText("");
                }
                else
                {
                    setHintPopup.SetText(worldIds[i++]);
                }
            }
        }

        public IEnumerator PlayCompleteWinAnimation(Queue<IEnumerator> coroutines, Action OnEnd)
        {
            if (coroutines == null)
            {
                OnEnd?.Invoke();
                yield break;
            }
            while (coroutines.Count>0)
            {
                yield return coroutines.Dequeue();
            }
            OnEnd?.Invoke();
        }

        public void SetActiveWorld(string activeWorldId, bool isWorldSelected)
        {
            selectWorldView.SetActiveWorld(activeWorldId, isWorldSelected);
        }

        public void SetTimer(float seconds)
        {
            timer.SetMaxTimer(seconds);
        }

        public void ShowMulligan(bool firstTurn, bool enable, Action<bool> acceptAction)
        {
            if (enable)
                mulliganUI.ShowMulligan(firstTurn, acceptAction);
            else
            {
                mulliganUI.HideMulligan();
            }
        }

        public virtual void NoMoreAction()
        {
        }
        public IEnumerator WinAnimationCoroutine()
        {
            if (this is LocalPlayerView)
            {
                gameSceneAnalytics.SetGameEndType(GameSceneAnalytics.GameEndType.Win);
            }
            else
            {
                gameSceneAnalytics.SetGameEndType(GameSceneAnalytics.GameEndType.Loss);
            }
            gameSceneAnalytics.SetGameType(GameController.Instance.IsInRankedMatch);
            UIManager.CurrentStateUI = UIManager.StateUI.Victory;
            yield return (statsById.GetItem(Constants.ShardStat) as ShardStatView)?.WinAnimation();
        }
        
        public IEnumerator BattlePassExpUIAnimationCoroutine(int battlePassExperience, string battlePassId)
        {
            yield return postBattleBattlePassView.ShowCoroutine(battlePassId, battlePassExperience);
            yield return postBattleBattlePassView.ExpUpCoroutine(battlePassExperience);
        }
        
        public IEnumerator RankUpUIAnimationCoroutine(int rankExp, string currentRank, string nextRank,
            int currentLevel, int nextLevel, int currentExp, int expToNextLevel)
        {
            yield return rankView.PlayRankExpChangeAnimation(currentRank, nextRank ,currentLevel, nextLevel,currentExp, rankExp ,expToNextLevel);
        }
        
        public IEnumerator RankPositionUpUIAnimationCoroutine(int startPosition, int endPosition)
        { 
            yield return rankView.PlayAscendantRankLeaderboardAnimation(startPosition, endPosition);
        }

        public IEnumerator TournamentPositionUpUIAnimationCoroutine(int previousWin, int previousLoss, int nextWin, int nextLoss)
        {
            yield return rankView.PlayTournamentLeaderboardAnimation(previousWin, previousLoss, nextWin, nextLoss);
        }

        #endregion

 
        
        protected void UpdateStat(string id, int originalValue, int value)
        {
            var statView = statsById.GetItem(id);

            if (statView != null)
            {
                statView.SetStat(originalValue, value);
            }
            else
            {
                Debug.LogWarning($"There is no view to show the stat {id}. Do you need to implement one?");
            }
        }

        public void SetShardChangedCardPosition(Vector3 cardPosition, bool isSteal, bool hasShardAnimation,
            bool isOpponentShard)
        {
            ShardStatView shardStatView = (statsById.GetItem(Constants.ShardStat) as ShardStatView);
            shardStatView.SetShardAnimationInfo(cardPosition, isSteal, hasShardAnimation, isOpponentShard);
        }

        public bool RaycastHitOnGivenLayer(LayerMask layer)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, layer);
        }

        public void CheckForLayerMaskInteraction(LayerMask layer, bool enable)
        {
            checkLayer = enable;
            if (!checkLayer) return;
            layerToCheck = layer;
        }
    }
}
