using Dawnshard.Network;
using Dawnshard.Presenters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// Shows a new card at the side of the screen to let the player know their effects and details
    /// </summary>
    public class ShowCardInfoZoneView : HighlightZoneView
    {
        [Header("Board Settings")]

        [SerializeField] protected BaseCardView dummyCardView;
        [SerializeField] protected BaseCardView miniDummyCardView;

        //private IBaseCardPresenter cardPresenter;

        protected override void RemoveCard(ICardPresenter card)
        {
            base.RemoveCard(card);
            HighlightEnded();
        }

        protected override void HighlightEnded()
        {
            HideInfoCard();
        }

        protected override void HighlightStarted(ICardPresenter card)
        {
            ShowInfoCard(card);
        }

        protected override void OnCardMouseDown(ICardPresenter cardPresenter)
        {
        }

        protected override void OnCardMouseUp(ICardPresenter cardPresenter)
        {
        }

        protected void ShowPlayInfoCard(CardModel cardModel)
        {
            if(cardModel==null)
                return;
            // if (cardPresenter == null)
            // {
                var cardPresenter = new BaseCardPresenter(dummyCardView, cardModel);
                cardPresenter.SetStaticFields();
            // }
            // else
            // {
            //     cardPresenter.Model = GamePresenter.cardPresenters[cardId].Model;
            //     cardPresenter.SetStaticFields();
            // }
            dummyCardView.gameObject.SetActive(true);
        }

        protected void HidePlayInfoCard()
        {
            dummyCardView.gameObject.SetActive(false);
        }

        protected void ShowInfoCard(ICardPresenter card)
        {
            //if (cardPresenter == null)
            var cardPresenter = new BaseCardPresenter(miniDummyCardView, card.Model);
            // else
            // {
            //     cardPresenter.Model = card.Model;
            //     cardPresenter.SetStaticFields();
            // }
            miniDummyCardView.transform.position = card.GetTargetWorldPosition(null) + new Vector3(6f, 2f, 6f);
            cardPresenter.SetKeywordList(true);
            miniDummyCardView.gameObject.SetActive(true);
        }

        protected void HideInfoCard()
        {
            miniDummyCardView.gameObject.SetActive(false);
        }
    }
}
