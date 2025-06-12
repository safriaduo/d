using System;
using System.Collections;
using Dawnshard.Network;
using Dawnshard.Presenters;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dawnshard.Views
{
    public class HandZoneView : HighlightZoneView
    {
        [Header("Hand Animation")]
        [SerializeField]
        private float verticalPosition = 2f;

        [SerializeField] private float highlightedVerticalPosition = 1.5f;
        [SerializeField] private DiscardAreaInteraction discardArea;
        [SerializeField] private ZoneView deckView;
        [SerializeField] private CardView dummyMulligan;
        [SerializeField] private RectTransform mobileCardParent;
        [SerializeField] protected GameObject backCardDummyPrefab;
        [SerializeField] private RectTransform mobileCardDummyParent;
        private bool isMobileHandShown = false;
        private bool hasPlayedCard = false;


        private float mobileVerticalPositionOffset = 0f;
        private float mobileYPositionOffset = 0f;

        private RectTransform mobileHighlight;


#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS

        private void Start()
        {
            mobileHighlight = cardsDummyParent;
            mobileHighlight.position += 8 * Vector3.back;
            cardsDummyParent = mobileCardParent;
            mobileVerticalPositionOffset = 14f;
            mobileYPositionOffset = 7.5f;
        }
#endif

        //private ICardPresenter selectedCard;
        private bool lockCardsInteraction = true;


        protected override void HighlightEnded()
        {
#if !(UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS)
            if (lockCardsInteraction)
                return;
            foreach (var cardPresenter in cardPresenters)
            {
                if (cardPresenter != null)
                    cardPresenter.SetKeywordList(false);
            }

            StartCoroutine(CoroutineReorderCards());
            DestroyNotFoundPresenterZoneTransform();
#endif

            //ReorderCards();

        }

        private void MobileHighlightEnded()
        {
            if (lockCardsInteraction)
                return;
            foreach (var cardPresenter in cardPresenters)
            {
                if (cardPresenter != null)
                    cardPresenter.SetKeywordList(false);
            }

            StartCoroutine(CoroutineReorderCards());
            DestroyNotFoundPresenterZoneTransform();
            //ReorderCards();
        }

        protected override void AddCard(ICardPresenter cardPresenter)
        {
            base.AddCard(cardPresenter);
            ReorderCards();
        }

        protected override void HighlightStarted(ICardPresenter card)
        {
            if (lockCardsInteraction)
                return;
            foreach (var cardPresenter in cardPresenters)
            {
                if (cardPresenter == null)
                    continue;

                if (cardPresenter.ZoneTransform == null)
                {
                    CreateZoneTransform(cardPresenter);
                }

                var basePos = cardPresenter.ZoneTransform.position;

                if (cardPresenter != card)
                {
                    basePos.z += verticalPosition + mobileVerticalPositionOffset;
                    basePos.y += mobileYPositionOffset;
                    cardPresenter.SetKeywordList(false);
                }
                else
                {
                    basePos.z += highlightedVerticalPosition + mobileVerticalPositionOffset;
                    basePos.y += mobileYPositionOffset + 3f;
                    cardPresenter.SetKeywordList(true);
                }

                cardPresenter.AnimateTo(basePos);
            }
        }

        protected override void OnCardMouseDown(ICardPresenter cardPresenter)
        {
            if (lockCardsInteraction)
                return;
            //selectedCard = cardPresenter;
#if UNITY_ANDROID || UNITY_IOS
            MobileHighlightEnded();
#elif UNITY_WEBGL
            HighlightEnded();
#endif

            if (cardPresenter.Model.CanBePlayed && cardPresenter.Model.IsOwnerTurn)
                discardArea.MiniGrow();

        }

        protected override void OnCardMouseUp(ICardPresenter cardPresenter)
        {
            if (lockCardsInteraction)
                return;
            //selectedCard = null;

            if (cardPresenter.Model.CanBePlayed)
                discardArea.MiniShrink();
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            HighlightStarted(cardPresenter);
#endif

        }

        protected override void ReorderCards()
        {
            //bool toAdd = false;
            //if (selectedCard != null)
            //{
            //    cardPresenters.Remove(selectedCard);
            //    toAdd = true;
            //}


            int i = 0;
            if (!lockCardsInteraction)
            {
                foreach (var cardPresenter in cardPresenters)
                {
                    if (cardPresenter == null)
                        continue;

                    cardPresenter.SetSortingGroupOrderInLayer(i++);

                    if (cardPresenter.ZoneTransform == null)
                    {
                        CreateZoneTransform(cardPresenter);
                    }

                    var basePos = cardPresenter.ZoneTransform.position;
                    basePos.z += i * 0.01f;
                    cardPresenter.AnimateTo(basePos);
                }
            }

            base.ReorderCards();
            if (isMobileHandShown && !isTakingAction)
                HighlightStarted(cardPresenters.First());
        }

        public void HighlightMulligan(bool enable)
        {
            if (!enable)
            {
                lockCardsInteraction = false;
                StartCoroutine(CoroutineReorderCards());
                return;
            }

            lockCardsInteraction = true;
            var i = 0;
            foreach (var cardPresenter in cardPresenters)
            {
                i++;
                if (cardPresenter != null)
                {
                    var dummyCardGameObject = Instantiate(dummyMulligan.gameObject, dummyMulligan.transform.parent);
                    new CardPresenter(dummyCardGameObject.GetComponent<CardView>(), cardPresenter.Model, null);
                }
            }
            dummyMulligan.transform.parent.localScale *= (1.5f - i * 0.1f);
            Destroy(dummyMulligan.gameObject);
        }

        protected override void DestroyCardPresenter(ICardPresenter cardPresenter)
        {
            if (cardPresenter == null)
                return;
            if (lockCardsInteraction)
            {
                //TODO: pensare ad una soluzione migliore, ma per ora risolve tutti i problemi
                cardPresenter.AnimateTo(cardPresenter.ZoneTransform.position * 100);
            }

            base.DestroyCardPresenter(cardPresenter);
        }

        public void HideMobileHand()
        {
            cardsDummyParent = mobileCardParent;
            isMobileHandShown = false;
            foreach (var cardPresenter in cardPresenters)
            {
                if (cardPresenter == null)
                    continue;

                if (cardPresenter.ZoneTransform == null)
                {
                    CreateZoneTransform(cardPresenter);
                }
                else
                {
                    DestroyZoneTransform(cardPresenter);
                    CreateZoneTransform(cardPresenter);
                }

                var basePos = cardPresenter.ZoneTransform.position;

                cardPresenter.SetKeywordList(false);

                cardPresenter.AnimateTo(basePos);
            }
        }

        public void ShowMobileHand()
        {
            cardsDummyParent = mobileHighlight;
            isMobileHandShown = true;
            foreach (var cardPresenter in cardPresenters)
            {
                if (cardPresenter == null)
                    continue;

                if (cardPresenter.ZoneTransform == null)
                {
                    CreateZoneTransform(cardPresenter);
                }
                else
                {
                    DestroyZoneTransform(cardPresenter);
                    CreateZoneTransform(cardPresenter);
                }

                var basePos = cardPresenter.ZoneTransform.position;


                basePos.z += highlightedVerticalPosition + mobileVerticalPositionOffset;
                basePos.y += 3f;
                cardPresenter.SetKeywordList(false);


                cardPresenter.AnimateTo(basePos);
            }
            HighlightStarted(cardPresenters.First());
        }

        public void SpawnMobileCardDummies()
        {
            /*if(mobileCardDummyParent.childCount==cardPresenters.Count)
                return;
            if (mobileCardDummyParent.childCount < cardPresenters.Count)
            {
                for (int i = mobileCardDummyParent.childCount; i < cardPresenters.Count; i++)
                {
                    var dummyCard = Instantiate(backCardDummyPrefab, mobileCardDummyParent);
                }
                return;
            }

            for (int i = mobileCardDummyParent.childCount; i > cardPresenters.Count; i--)
            {
                Destroy(mobileCardDummyParent.GetChild(0).gameObject);
            }*/
        }

    }
}