using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Network
{
    public interface ICardPresenter : IBaseCardPresenter
    {
        /// <summary>
        /// This is the transform where the card belongs to
        /// </summary>
        Transform ZoneTransform { get; set; }

        /// <summary>
        /// Get the world position of the target
        /// </summary>
        Vector3 GetTargetWorldPosition(string effectId);

        /// <summary>
        /// Refreshes the dynamic fields of the card
        /// </summary>
        void UpdateView();

        /// <summary>
        /// Animate the card to go to a position. If it's a modifier, it only 
        /// adds the position to the current position
        /// </summary>
        void AnimateTo(Vector3 position);

        /// <summary>
        /// Set the default highlight status for the card. This takes into account
        /// the playability of the card, the ready, etc.
        /// </summary>
        void SetDefaultHighlight();

        /// <summary>
        /// Set the SortingGroup order in layer of a card and its element
        /// </summary>
        void SetSortingGroupOrderInLayer(int i);

        /// <summary>
        /// Resize the frame of the card to big or small
        /// </summary>
        void ChangeCardFrame(bool growFrame);

        /// <summary>
        /// Duration of the move animation between two zones
        /// </summary>
        float GetMoveDuration(string origZone, string destZoneId);

        /// <summary>
        /// Duration of the reap animation
        /// </summary>
        float ReapDuration { get; }

        /// <summary>
        /// Duration of the fight animation
        /// </summary>
        float FightDuration { get; }

        /// <summary>
        /// Duration of the ready change animation
        /// </summary>
        float ReadyChangeDuration { get; }

        /// <summary>
        /// Duration of the ability trigger animation
        /// </summary>
        float AbilityDuration { get; }
    }
}