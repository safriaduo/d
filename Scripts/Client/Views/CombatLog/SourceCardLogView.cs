using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class SourceCardLogView : LogView, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Image cardArtwork;
        private bool inside = false;

        public override void SetSourceCard(CardModel sourceCard)
        {
            if(sourceCard!=null)
                cardArtwork.sprite = Resources.Load<Sprite>(sourceCard.ArtworkPath);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(inside)
                return;
            OnPointerEnter_LogEntry();
            inside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!inside) return;
            OnPointerExit_LogEntry();
            inside = false;
        }
    }
}
