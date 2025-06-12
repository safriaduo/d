using Dawnshard.Network;
using Dawnshard.Presenters;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// This zone view component has the responsability to show the number of cards in a zone
    /// </summary>
    public class StaticZoneView : ZoneView
    {
        [SerializeField] private TMP_Text numCardsText;
        [SerializeField] protected GameObject numCardsParent;
        protected int numCards;
        public override void SetNumCards(int newNumCards)
        {
            numCards = newNumCards;
            numCardsText.text = newNumCards.ToString();
        }

        private void OnMouseOver() {
            if(UIManager.CurrentStateUI!=UIManager.StateUI.None)
                return;
            numCardsParent.SetActive(true);
        }

        private void OnMouseExit() {
            numCardsParent.SetActive(false);
        }
    }
}
