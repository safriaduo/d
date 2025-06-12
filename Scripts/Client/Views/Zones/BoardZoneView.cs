using Dawnshard.Network;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Presenters;
using UnityEngine;

namespace Dawnshard.Views
{
    public class BoardZoneView : ShowCardInfoZoneView
    {
        [SerializeField] protected RectTransform battlefieldParent;
        [SerializeField] protected RectTransform benchParent;
        [SerializeField] protected ReapAreaInteraction reapAreaInteraction;

        protected override void ReorderCards()
        {
            ComputeBenchCreatures();

            base.ReorderCards();

            //cardIds.ForEach(cardId =>
            //{
            //    if(ZonePresenter.GetCardPresenter(cardId)!=null)
            //ZonePresenter.GetCardPresenter(cardId).OnCardReadyChanged();
            //});
        }

        private void ComputeBenchCreatures()
        {
            foreach (var cardPresenter in cardPresenters)
            {
                if (cardPresenter == null)
                    continue;

                bool isReady = cardPresenter.Model.CanReap || cardPresenter.Model.CanFight;

                StartCoroutine(UpdateBoardCardView(isReady, cardPresenter));
            }
        }

        private IEnumerator UpdateBoardCardView(bool isReady, ICardPresenter cardPresenter)
        {
            if (isReady)
            {
                //If it's not on the board
                if (cardPresenter.ZoneTransform.parent != battlefieldParent)
                {
                    UpdateZoneTransform(cardPresenter);
                }
                cardPresenter.ChangeCardFrame(growFrame: true);
            }
            else
            {
                cardPresenter.ChangeCardFrame(growFrame: false);
                yield return new WaitForSeconds(2f);
                //If it's on the board
                if (cardPresenter.ZoneTransform == null)
                {
                    CreateZoneTransform(cardPresenter);
                }
                else if (cardPresenter.ZoneTransform.parent != benchParent)
                {
                    UpdateZoneTransform(cardPresenter);
                }
            }
        }

        private void UpdateZoneTransform(ICardPresenter presenter)
        {
            //Destroy the previous transform
            DestroyZoneTransform(presenter);
            CreateZoneTransform(presenter);
            StartCoroutine(CoroutineReorderCards());
        }

        protected override void CreateZoneTransform(ICardPresenter cardPresenter)
        {
            bool isReady = cardPresenter.Model.CanReap || cardPresenter.Model.CanFight;
            //bool isPlayerTurn = cardPresenter.Model.Owner.Model.IsPlayerTurn;
            //TODO: bisogna trovare un modo che la view sappia di chi e il turno
            cardsDummyParent = isReady /*|| !isPlayerTurn*/ ? battlefieldParent : benchParent;

            base.CreateZoneTransform(cardPresenter);
        }

        protected override void OnCardMouseDown(ICardPresenter cardPresenter)
        {
            if (ZoneId == Constants.BoardZone && cardPresenter.Model.CanReap && cardPresenter.Model.IsOwnerTurn)
                reapAreaInteraction.PlayHint();

        }

        protected override void OnCardMouseUp(ICardPresenter cardPresenter)
        {
            if (ZoneId == Constants.BoardZone && cardPresenter.Model.CanReap && cardPresenter.Model.IsOwnerTurn)
                reapAreaInteraction.StopHint();
        }
    }
}