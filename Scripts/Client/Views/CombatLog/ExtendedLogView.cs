using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dawnshard.Network;
using Dawnshard.Presenters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
    public class ExtendedLogView : SourceCardLogView
    {
        //[SerializeField] private TextMeshProUGUI actionText;
        [SerializeField] private Transform sourceCardParent;
        [SerializeField] private Transform targetCardGrid;
        [SerializeField] private Transform targetCardTemplate;
        [SerializeField] private GameObject unknownTargetPrefab;
        [SerializeField] private Transform singleTargetParent;
        [SerializeField] private Transform singleTargetView;

        private float hideTimer = 5f;
        private float currentTimer = 0f;

        #region ILogView

        public override void SetIcon(Sprite iconSprite, string text, bool isLocalPlayer)
        {
        }

        override public void SetSourceCard(CardModel sourceCard)
        {
            var presenter = CardFactory.Instance.CreateBaseCard(sourceCard, sourceCardParent);
            presenter.ToggleHighlight(true, sourceCard.IsOwnerLocalPlayer ? LOCAL_PLAYER_COLOR : OPPONENT_PLAYER_COLOR);
        }

        private void Update()
        {
            if(!gameObject.activeSelf)
                return;
            currentTimer += Time.deltaTime;
            if (currentTimer >= hideTimer)
            {
                currentTimer = 0f;
                gameObject.SetActive(false);
            }
        }

        override public void SetTargets(List<CardModel> targetCards)
        {
            if (targetCards == null)
            {
                singleTargetView.gameObject.SetActive(false);
                targetCardGrid.gameObject.SetActive(false);
                actionIcon.gameObject.SetActive(false);
                return;
            }
            actionIcon.gameObject.SetActive(true);
            if (targetCards.Count == 1)
            {
                CreateSingleTargetEntry(targetCards[0]);
            }
            else
            {
                CreateMultiTargetEntry(targetCards);
            }
        }
        #endregion

        /// <summary>
        /// Create all the multiple targets for the extended log
        /// </summary>
        private void CreateMultiTargetEntry(List<CardModel> targetCards)
        {
            singleTargetView.gameObject.SetActive(false);
            targetCardGrid.gameObject.SetActive(true);
            foreach (CardModel targetModel in targetCards)
            {
                Transform targetCardViewParent = Instantiate(targetCardTemplate, targetCardGrid);
                var cardParent = targetCardViewParent.gameObject.GetComponent<ExpandedLogEntryCard>().GetCardParentLogEntry();
                if (targetModel != null)
                {
                    CardFactory.Instance.CreateBaseCard(targetModel, cardParent)
                        .ToggleHighlight(true, targetModel.IsOwnerLocalPlayer ? LOCAL_PLAYER_COLOR : OPPONENT_PLAYER_COLOR);
                }
                else
                {
                    if (unknownTargetPrefab != null)
                        Instantiate(unknownTargetPrefab, cardParent, false);
                }

            }
        }

        /// <summary>
        /// Create the single target for the extended log
        /// </summary>
        private void CreateSingleTargetEntry(CardModel targetModel)
        {
            singleTargetView.gameObject.SetActive(true);
            targetCardGrid.gameObject.SetActive(false);
            if (targetModel != null)
            {
                CardFactory.Instance.CreateBaseCard(targetModel, singleTargetParent)
                    .ToggleHighlight(true, targetModel.IsOwnerLocalPlayer ? LOCAL_PLAYER_COLOR : OPPONENT_PLAYER_COLOR);
            }
            else
            {
                if (unknownTargetPrefab != null)
                    Instantiate(unknownTargetPrefab, singleTargetParent, false);
            }

        }
    }
}