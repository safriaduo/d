using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Dawnshard.Network
{
    public interface IPlayerPresenter
    {
        /// <summary>
        /// The model this presenter is displaying
        /// </summary>
        PlayerModel Model { get; set; }

        /// <summary>
        /// The target position of the player for the effects
        /// </summary>
        Vector3 GetTargetWorldPosition(string effectID);

        /// <summary>
        /// The player active world has changed
        /// </summary>
        void OnActiveWorldChanged(string worldId, bool worldSelected);

        /// <summary>
        /// The player stat has changed
        /// </summary>
        void OnPlayerStatChanged(StatModel stat, int prevValue);

        /// <summary>
        /// The player has won the game
        /// </summary>
        Task OnPlayerWon(Action OnEnd, string battlePassId, int battlePassExperience);

        /// <summary>
        /// Set the duration of each turn
        /// </summary>
        void SetTimer(float seconds);

        /// <summary>
        /// Toggles wheter the player presenter need to check for zone interactions when a card is being dragged
        /// </summary>
        void ToggleDragInteraction(bool enable, CardModel cardModel);

        /// <summary>
        /// Refreshes the view 
        /// </summary>
        void UpdateView();

        /// <summary>
        /// Set the view associated with this presenter as the active player
        /// </summary>
        void SetActivePlayer();

        void CheckForPlayableCards();
        void SetRank();
    }
}