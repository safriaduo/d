using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class MulliganView : MonoBehaviour
    {
        [SerializeField] private TMP_Text opponentText;
        [SerializeField] private TMP_Text playerTurnText;
        [SerializeField] private MMFeedbacks glowText;
        [SerializeField] private Button acceptMulliganButton;
        [SerializeField] private Button refuseMulliganButton;
        [SerializeField] private GameObject mulliganUI;
        [SerializeField] private HandZoneView handZoneView;
        [SerializeField] private GameObject cardHorizontalLayer;

        private const string WAITING_FOR_OPPONENT_MULLIGAN = "Waiting for the opponent...";
        private const string SECOND_PLAYER_MULLIGAN = "You go second!";
        // Update is called once per frame

        private void Start()
        {
            mulliganUI.SetActive(false);
        }

        public void ShowMulligan(bool firstTurn, Action<bool> ButtonClick)
        {
            mulliganUI.SetActive(true);
            UIManager.CurrentStateUI = UIManager.StateUI.Mulligan;
            handZoneView.HighlightMulligan(true);
            if (!firstTurn)
                SecondPlayerTurnText();
            acceptMulliganButton.onClick.AddListener(() => { ButtonClick(true); HideButtons(); ShowCustomText(); });
            refuseMulliganButton.onClick.AddListener(() => { ButtonClick(false); HideButtons(); ShowCustomText(); });
        }

        public void ShowCustomText()
        {
            opponentText.text = WAITING_FOR_OPPONENT_MULLIGAN;
            glowText.PlayFeedbacks();
        }


        public void SecondPlayerTurnText()
        {
            playerTurnText.text = SECOND_PLAYER_MULLIGAN;
        }

        private void HideButtons()
        {
            acceptMulliganButton.gameObject.SetActive(false);
            refuseMulliganButton.gameObject.SetActive(false);
        }

        public void HideMulligan()
        {
            UIManager.CurrentStateUI = UIManager.StateUI.None;

            handZoneView.HighlightMulligan(false);
            mulliganUI.SetActive(false);
        }
    }
}
