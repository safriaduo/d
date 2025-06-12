using System;
using System.Collections;
using Dawnshard.Network;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Presenters;
using Unity.Mathematics;
using UnityEngine;

namespace Dawnshard.Views
{
    public class GraveyardZoneView : DynamicZoneView
    {
        [SerializeField] private LayerMask graveyardLayer;
        [SerializeField] private GameObject UICardsList;
        [SerializeField] private SliderCardsAnimation sliderCardsAnimation;
        [SerializeField] private CardListSelector cardListSelector;
        [SerializeField] private DiscardAreaInteraction discardAreaInteraction;
        [SerializeField] private Transform cardParentTransform;
        
        private List<IBaseCardPresenter> gyCardPresentersShown = new();

        private void Start()
        {
            discardAreaInteraction.OnPointerUpOnGraveyard += OnPointerUpOnGraveyard;
        }
        private void OnPointerUpOnGraveyard()
        {
            ShowGraveyard();
        }

        protected override void CreateZoneTransform(ICardPresenter cardPresenter)
        {
            base.CreateZoneTransform(cardPresenter);

            StartCoroutine(WaitAndPlaceInGraveyard(cardPresenter));
            //TODO: fare un comportamento più giusto e non richiamare la createzonetransform
        }

        private void Update()
        {
            while (gyCardPresentersShown.Count > 3)
            {
                var lastCardPresenterShown = gyCardPresentersShown[0];
                gyCardPresentersShown.RemoveAt(0);
                lastCardPresenterShown.Destroy();
            }
        }

        private IEnumerator WaitAndPlaceInGraveyard(ICardPresenter cardPresenter)
        {
            // while (gyCardPresentersShown.Count >= 3)
            // {
            //     var lastCardPresenterShown = gyCardPresentersShown[0];
            //     gyCardPresentersShown.RemoveAt(0);
            //     lastCardPresenterShown.Destroy();
            // }
            yield return new WaitForSeconds(0.5f);

            gyCardPresentersShown.Add(CardFactory.Instance.CreateBaseCard(cardPresenter.Model, cardsDummyParent));
        }

        protected override void DestroyZoneTransform(ICardPresenter cardPresenter)
        {
            base.DestroyZoneTransform(cardPresenter);
            
            foreach (var cardPresenterShown in gyCardPresentersShown.ToList())
            {
                if (cardPresenterShown.Model.InstanceId == cardPresenter.Model.InstanceId)
                {
                    var lastCardPresenterShown = cardPresenterShown;
                    gyCardPresentersShown.Remove(cardPresenterShown);
                    lastCardPresenterShown.Destroy();
                }
            }
            
        }

        protected override void ReorderCards()
        {
            
        }

        public void ShowGraveyard()
        {
            UICardsList.SetActive(true);
            CardFactory cardFactory = CardFactory.Instance;
            List<ICardPresenter> cardPresentersUI = new();
            foreach (var card in cardPresenters)
            {
                cardPresentersUI.Add(cardFactory.CreateGameCard(card.Model,null, cardParentTransform));
            }
            List<GameObject> cardsGO = new();
            foreach (Transform child in cardParentTransform)
            {
                cardsGO.Add(child.gameObject);
            }
            sliderCardsAnimation.AddCardList(cardsGO);
            cardListSelector.Initialize(0, () => HideGraveyard());
        }

        private void HideGraveyard()
        {
            foreach (Transform child in cardParentTransform)
            {
                Destroy(child.gameObject);
            }
            UICardsList.SetActive(false);
            discardAreaInteraction.OnHide();
        }

        protected override void DestroyNotFoundPresenterZoneTransform()
        {
        }
    }
}
