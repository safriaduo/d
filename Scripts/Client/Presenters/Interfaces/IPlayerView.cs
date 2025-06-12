using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using UnityEngine;


namespace Dawnshard.Presenters
{
    public interface IPlayerView
    {
        /// <summary>
        /// Called when the user activates a world
        /// </summary>
        Action<string> SelectActiveWorld { get; set; }

        /// <summary>
        /// Called when the user pass it's turn
        /// </summary>
        Action StopTurn { get; set; }

        /// <summary>
        /// The world position of the player for effect purposes
        /// </summary>
        Vector3 GetWorldPosition(string effectID);

        /// <summary>
        /// Sets the nickname of the player
        /// </summary>
        /// <param name="nickname"></param>
        void SetNickname(string nickname);

        /// <summary>
        /// Set the active world status
        /// </summary>
        void SetActiveWorld(string activeWorldId, bool isWorldSelected);

        /// <summary>
        /// Set if the player is the active one in the current turn
        /// </summary>
        void SetActivePlayer(bool isPlayerTurn);

        /// <summary>
        /// A stat changed it's value
        /// </summary>
        void ChangeStat(string id, int originalValue, int value, int prevValue);

        /// <summary>
        /// Set a stat tu a value
        /// </summary>
        void SetStat(string id, int originalValue, int value);

        /// <summary>
        /// Set the name of the player's deck
        /// </summary>
        void SetDeckName(string deckName);

        /// <summary>
        /// Set the worlds available to select in the deck
        /// </summary>
        void SetDeckWorlds(string[] worldIds);

        /// <summary>
        /// Set the position of the card that has changed the shard stat
        /// </summary>
        void SetShardChangedCardPosition(Vector3 cardPosition, bool isSteal, bool hasShardAnimation,
            bool isOpponentShard);

        /// <summary>
        /// Checks wheter a raycast hit a given layer and give the gameobject hit
        /// </summary>
        public bool RaycastHitOnGivenLayer(LayerMask layer);
        
        /// <summary>
        /// If true checks wheter the given layer has an interaction and play animations accordingly, if false stop observing that layer
        /// </summary>
        void CheckForLayerMaskInteraction(LayerMask layer, bool enable);

        /// <summary>
        /// Set the timer to the lenght of the turn
        /// </summary>
        void SetTimer(float seconds);

        /// <summary>
        /// Show the mulligan ui
        /// </summary>
        void ShowMulligan(bool firstTurn, bool enable, Action<bool> acceptAction);

        void NoMoreAction();

        /// <summary>
        /// Method that plays all the Win animation
        /// </summary>
        ///
        IEnumerator PlayCompleteWinAnimation(Queue<IEnumerator> coroutines, Action OnEnd);
        IEnumerator WinAnimationCoroutine();

        IEnumerator BattlePassExpUIAnimationCoroutine(int battlePassExperience,
            string battlePassId);

        IEnumerator RankUpUIAnimationCoroutine(int rankExp, string currentRank, string nextRank,
            int currentLevel, int nextLevel, int currentExp, int expToNextLevel);

        IEnumerator RankPositionUpUIAnimationCoroutine(int startPosition, int endPosition);
        
        IEnumerator TournamentPositionUpUIAnimationCoroutine(int previousWin, int previousLoss, int nextWin, int nextLoss);

    }
}