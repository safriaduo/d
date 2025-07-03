using Dawnshard.Network;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Presenters
{
    public interface IZoneView
    {

        /// <summary>
        /// Set the id of the zone view
        /// </summary>
        string ZoneId { get; set; }

        /// <summary>
        /// Get the transform under which the cards will spawn
        /// </summary>
        Transform GetCardParent();

        /// <summary>
        /// Set cards on this zone
        /// </summary>
        void SetCards(List<ICardPresenter> cardPresenter);

        /// <summary>
        /// Set the max cards on this zone
        /// </summary>
        void SetMaxCards(int maxCards);

        /// <summary>
        /// Set the number of cards in the zone
        /// </summary>
        void SetNumCards(int numCards);

    }
}