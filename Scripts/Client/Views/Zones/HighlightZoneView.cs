using Dawnshard.Network;
using Dawnshard.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// This zone handles the interaction of hovering cards
    /// </summary>
    public abstract class HighlightZoneView : DynamicZoneView
    {
        [Header("Highlight Settings")]
        [SerializeField] protected float secondsBeforeHighlightStart = 1f;
        [SerializeField] protected float secondsBeforeHighlightEnd = .5f;

        private Coroutine startHighlightCoroutine;
        private Coroutine endHighlightCoroutine;
        protected bool isTakingAction;

        private void HighlightCard(ICardPresenter card)
        {
            CancelEndHighlight();
            CancelStartHighlight();

            startHighlightCoroutine = StartCoroutine(StartHighlight(card));
        }

        private IEnumerator StartHighlight(ICardPresenter card)
        {
            yield return new WaitForSeconds(secondsBeforeHighlightStart);

            HighlightStarted(card);

            startHighlightCoroutine = null;
        }

        private IEnumerator EndHighlight()
        {
            yield return new WaitForSeconds(secondsBeforeHighlightEnd);

            HighlightEnded();

            endHighlightCoroutine = null;
        }

        private void CancelStartHighlight()
        {
            if (startHighlightCoroutine != null)
            {
                StopCoroutine(startHighlightCoroutine);
                startHighlightCoroutine = null;
            }
        }

        private void CancelEndHighlight()
        {
            if (endHighlightCoroutine != null)
            {
                StopCoroutine(endHighlightCoroutine);
                endHighlightCoroutine = null;
            }
        }

        protected override void InitializeCardPresenter(ICardPresenter cardPresenter)
        {
            if(cardPresenter==null)
                return;
            base.InitializeCardPresenter(cardPresenter);
            void onHover()
            {
                if(UIManager.CurrentStateUI!=UIManager.StateUI.None)
                    return;
                if (!isTakingAction)
                {
                    HighlightCard(cardPresenter);
                }
            }

            void onHoverEnd()
            {
                if (!isTakingAction)
                {
                    CancelStartHighlight();
                    endHighlightCoroutine = StartCoroutine(EndHighlight());
                }
            }

            void onMouseDown()
            {
                if(UIManager.CurrentStateUI!=UIManager.StateUI.None)
                    return;
                CancelStartHighlight();
                HighlightEnded();
                isTakingAction = true;
                OnCardMouseDown(cardPresenter);
            }

            void onMouseUp()
            {
                if(UIManager.CurrentStateUI!=UIManager.StateUI.None)
                    return;
                isTakingAction = false;
                OnCardMouseUp(cardPresenter);
            }

            cardPresenter.RegisterInputCallback(UserInput.HoverStarted, onHover);
            cardPresenter.RegisterInputCallback(UserInput.HoverEnded, onHoverEnd);
            cardPresenter.RegisterInputCallback(UserInput.MouseDown, onMouseDown);
            cardPresenter.RegisterInputCallback(UserInput.MouseUp, onMouseUp);
        }


        protected override void DestroyCardPresenter(ICardPresenter cardPresenter)
        {
            if(cardPresenter==null)
                return;
            base.DestroyCardPresenter(cardPresenter);

            cardPresenter.UnregisterInputCallback(UserInput.HoverStarted);
            cardPresenter.UnregisterInputCallback(UserInput.HoverEnded);
            cardPresenter.UnregisterInputCallback(UserInput.MouseDown);
            cardPresenter.UnregisterInputCallback(UserInput.MouseUp);
        }

        /// <summary>
        /// A card has began the selection
        /// </summary>
        protected abstract void OnCardMouseDown(ICardPresenter cardPresenter);

        /// <summary>
        /// A card has ended the selection
        /// </summary>
        protected abstract void OnCardMouseUp(ICardPresenter cardPresenter);

        /// <summary>
        /// The highlight has ended
        /// </summary>
        protected abstract void HighlightEnded();

        /// <summary>
        /// The highlight started on a card
        /// </summary>
        protected abstract void HighlightStarted(ICardPresenter card);
    }
}
