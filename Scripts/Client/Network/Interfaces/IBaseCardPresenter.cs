using Dawnshard.Presenters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Network
{
    public interface IBaseCardPresenter
    {
        /// <summary>
        /// The card model this presenter is displaying
        /// </summary>
        CardModel Model { get; set; }

        /// <summary>
        /// Sets the card fields that are not going to change throught the match
        /// </summary>
        public void SetStaticFields();

        /// <summary>
        /// Set the card highlight enabled
        /// </summary>
        void ToggleHighlight(bool enabled, Color color = default);

        /// <summary>
        /// Set the keywords list to enable
        /// </summary>
        void SetKeywordList(bool enabled);

        /// <summary>
        /// Destroys the gameobject of the card view
        /// </summary>
        void Destroy();

        /// <summary>
        /// Register a new callback to call when the user interacts 
        /// with the card
        /// </summary>
        void RegisterInputCallback(UserInput input, Action callback);

        /// <summary>
        /// Unregister an existing callback 
        /// </summary>
        void UnregisterInputCallback(UserInput input);
    }
}