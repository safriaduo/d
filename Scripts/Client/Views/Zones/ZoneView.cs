using Dawnshard.Network;
using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    /// <summary>
    /// This zone view component is the base class for each zone view
    /// </summary>
    public class ZoneView : MonoBehaviour, IZoneView
    {
        [SerializeField] private Transform cardParent;
        public string ZoneId { get; set; }

        public Transform GetCardParent()
        {
            return cardParent;
        }

        protected virtual void AddCard(ICardPresenter cardPresenter)
        {
        }

        protected virtual void RemoveCard(ICardPresenter cardPresenter)
        {
        }

        public virtual void SetCards(List<ICardPresenter> cardPresenters)
        {
        }

        public virtual void SetMaxCards(int maxCards)
        {
        }

        public virtual void SetNumCards(int numCards)
        {
        }
    }
}
