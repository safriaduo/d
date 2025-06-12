using Dawnshard.Network;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Dawnshard.Presenters
{
    public interface IUserCardView : IGameCardView
    {
        /// <summary>
        /// Spawns the targeting arrow to select one of the targets, then calls the callback
        /// </summary>
        void CreateTargetingArrow(Action<int> onSelectTarget);

        /// <summary>
        /// Start the dragging of the card
        /// </summary>
        void StartDrag(Action onCardDragEnd);

        /// <summary>
        /// Checks wheter a raycast hit a given layer
        /// </summary>
        public bool RaycastHitOnGivenLayer(LayerMask layer);
    }
}