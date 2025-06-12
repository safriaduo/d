using Dawnshard.Network;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Dawnshard.Presenters
{
    public interface ICardView
    {

        /// <summary>
        /// The user taken an action
        /// </summary>
        Action<UserInput> OnUserInput { get; set; }

        /// <summary>
        /// Set a stat to a value
        /// </summary>
        void SetStat(string id, int originalValue, int value, int maxValue);

        /// <summary>
        /// Set the name of the card
        /// </summary>
        void SetCardName(string name);

        /// <summary>
        /// Set the body tex
        /// </summary>
        void SetCardBodyText(string text);

        /// <summary>
        /// Set the type of the card
        /// </summary>
        void SetCardType(string cardType, string incandescense);

        /// <summary>
        /// Set the artwork of the card
        /// </summary>
        void SetArtwork(Sprite sprite);

        /// <summary>
        /// Set the rarity
        /// </summary>
        void SetRarity(string rarity);

        /// <summary>
        /// Set the world of the card
        /// </summary>
        public void SetWorld(string worldId);

        /// <summary>
        /// Set an action to be executed when the CardView is destroyed
        /// </summary>
        void SetOnDestroy(Action onDestroy);
        
        /// <summary>
        /// Destory the gameobject that has this component attached
        /// </summary>
        void Destroy();

        /// <summary>
        /// Set the card highlight enabled
        /// </summary>
        void ToggleHighlight(bool enabled, Color color = default, bool showHandHighlight = true);

        /// <summary>
        /// Set the keywords list to the keywords passed and enable it
        /// </summary>
        void SetKeywordList(bool enabled, List<string> keywords, List<string> effects, List<string> createdCards=null);
    }
}