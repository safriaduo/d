using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameAnalyticsSDK;
using UnityEngine;

namespace Dawnshard.Views
{
    public class LocalPlayerView : PlayerView
    {
        [SerializeField] private EndTurnButton endTurnButton;
        private bool turnCounted = false;
        private void Start()
        {
            endTurnButton.onClick.AddListener(() => StopTurn?.Invoke());
            changeTurnString = "YOUR\nTURN";
        }

        public override void ChangeStat(string id, int originalValue, int value, int prevValue)
        {
            UpdateStat(id, prevValue, value);
            if (id == Constants.ShardStat)
            {
                gameSceneAnalytics.SetLocalPlayerShard(value);
            }        
        }

        public override void SetActivePlayer(bool isPlayerTurn)
        {
            base.SetActivePlayer(isPlayerTurn);

            if (isPlayerTurn)
            {
                if (!turnCounted)
                {
                    gameSceneAnalytics.IncreaseTurnDuration();
                    turnCounted = true;
                }
                //endTurnButton.SetText("End Turn");
                endTurnButton.SetInteractable(true);
            }
            else
            {
                turnCounted = false;
                //endTurnButton.SetText("");
                endTurnButton.SetInteractable(false);
            }
        }

        public override void NoMoreAction()
        {
            endTurnButton.SetNoMoreActionAnimationLoop(true);
        }

        public override void SetDeckWorlds(string[] worldIds)
        {
            base.SetDeckWorlds(worldIds);
            gameSceneAnalytics.SetWorldsAnalytics(worldIds.ToList());
        }
    }
}
