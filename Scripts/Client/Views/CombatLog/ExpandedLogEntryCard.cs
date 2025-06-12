using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class ExpandedLogEntryCard : MonoBehaviour
    {
        [SerializeField] private Image playerColorBorder;
        [SerializeField] private Transform cardParent;

        private readonly Color LOCAL_PLAYER_COLOR = Color.blue;
        private readonly Color OPPONENT_PLAYER_COLOR = Color.red;

        public void UpdateEntryCardColor(bool localPlayer)
        {
            playerColorBorder.color = localPlayer ? LOCAL_PLAYER_COLOR : OPPONENT_PLAYER_COLOR;
        }

        public Transform GetCardParentLogEntry()
        {
            return cardParent;
        }
    }
}
