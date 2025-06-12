using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Presenters
{
    public interface ILogView
    {
        public Action OnPointerEnter_LogEntry { get; set; }
        public Action OnPointerExit_LogEntry { get; set; }

        /// <summary>
        /// Set the icon and the player of the action
        /// </summary>
        void SetIcon(Sprite iconSprite, string text, bool isLocalPlayer);

        /// <summary>
        /// Set the source card
        /// </summary>
        void SetSourceCard(CardModel sourceCard);

        /// <summary>
        /// Set the card targets
        /// </summary>
        void SetTargets(List<CardModel> targetCards);

        /// <summary>
        /// Destroy the view's GameObject
        /// </summary>
        void DestroyView();
    }
}