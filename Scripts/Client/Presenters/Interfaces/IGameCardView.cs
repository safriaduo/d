using Dawnshard.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dawnshard.Presenters
{
    public interface IGameCardView : ICardView
    {

        /// <summary>
        /// The instance id of the card
        /// </summary>
        int InstanceId { get; set; }

        /// <summary>
        /// The transform of the card position in the zone
        /// </summary>
        Transform ZoneTransform { get; set; }

        /// <summary>
        /// A keyword is added
        /// </summary>
        void AddKeyword(string id);

        /// <summary>
        /// A keyword is removed
        /// </summary>
        void RemoveKeyword(string id);

        /// <summary>
        /// Set all the keywords of a card
        /// </summary>
        void SetKeywords(List<string> ids);

        /// <summary>
        /// Set the icon of this card ability
        /// </summary>
        void SetTrigger(string triggerId);

        /// <summary>
        /// A stat changed it's value
        /// </summary>
        void ChangeStat(string id, int originalValue, int value, int prevValue, int maxValue);

        /// <summary>
        /// Set the ready state of a card
        /// </summary>
        void GrowFrame();

        /// <summary>
        /// Set the card as exhausted
        /// </summary>
        void ShrinkFrame();

        /// <summary>
        /// Get the target position in world space
        /// </summary>
        Vector3 GetPosition();

        /// <summary>
        /// Triggers an ability of the card, directed to a position
        /// </summary>
        void TriggerAbility(string triggerID, string effectID);

        /// <summary>
        /// Animate the object to a position
        /// </summary>
        void AnimateTo(Vector3 position);

        /// <summary>
        /// Play an animation after the card moved to a different zone
        /// </summary>
        void OnZoneChanged(string origZone, string destZone, Action OnEndAction=null);
         
        /// <summary>
        /// Play an animation for the card reap
        /// </summary>
        void PlayReapAnimation();

        /// <summary>
        /// Play an animation for the card fight
        /// </summary>
        void PlayFightAnimation(Vector3 destination);

        /// <summary>
        /// Set the target behaviour of the card view to replace or target
        /// </summary>
        void ToggleTarget(bool replace, bool fight);

        /// <summary>
        /// Set the SortingGroup order in layer of a card and its element
        /// </summary>
        void SetSortingGroupOrderInLayer(int i);

        void ExhaustEffect(bool enabled);
        
        void PlaySelectAnimation(bool selected);
    }
}