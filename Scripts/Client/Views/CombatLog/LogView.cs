using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class LogView : MonoBehaviour, ILogView
    {
        protected readonly Color LOCAL_PLAYER_COLOR = Color.blue;
        protected readonly Color OPPONENT_PLAYER_COLOR = Color.red;

        [SerializeField] protected Image actionIcon;
        [SerializeField] protected Image playerColorBorder;

        public Action OnPointerEnter_LogEntry { get; set; }
        public Action OnPointerExit_LogEntry { get; set; }

        public virtual void SetIcon(Sprite iconSprite, string text, bool localPlayer)
        {
            actionIcon.sprite = iconSprite;
            playerColorBorder.color = localPlayer ? LOCAL_PLAYER_COLOR : OPPONENT_PLAYER_COLOR;
        }

        public virtual void SetSourceCard(CardModel sourceCard)
        {
        }

        public virtual void SetTargets(List<CardModel> targetCards)
        {
        }

        public void DestroyView()
        {
            Destroy(gameObject);
        }

    }
}
