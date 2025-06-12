using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Database;
using Dawnshard.Network;
using Dawnshard.Views;
using UnityEngine;
namespace Dawnshard.Presenters
{
    public class CardSetPresenter
    {
        public CardSetModel Model { get; set; }

        protected CardSetView setView;

        private List<CardModel> cards = new();
        private IBaseCardPresenter currentCard;
        
        public CardSetPresenter(CardSetView view, CardSetModel model, bool noCards = false)
        {
            setView = view;
            Model = model;
            if (!noCards)
                SetStaticFields();
            else
                SetStaticFieldNoCards();
        }

        public void SetStaticFieldNoCards()
        {
            cards.Clear();
            foreach (var cardID in Model.Cards)
            {
                var newCardModel = CardDatabase.GetCardByName(cardID);
                if(newCardModel != null)
                    cards.Add(newCardModel);
            }

            setView.SetName(Model.Name);
            setView.SetIncandescense(Model.IncandescenseLevel);
            setView.SetWorld(Model.WorldId);
            setView.SetArtwork(
                Resources.Load<Sprite>(
                    cards
                        .First().ArtworkPath)
            );
            setView.SetCardTypeNumber(
                cards.Count(card => card.Type == Constants.CreatureType),
                cards.Count(card => card.Type == Constants.ArtifactType),
                cards.Count(card => card.Type == Constants.ActionType));
        }

        private void SetStaticFields()
        {
            foreach (var cardID in Model.Cards)
            {
                var newCardModel = CardDatabase.GetCardByName(cardID);
                if (newCardModel != null)
                {
                    newCardModel.IncandescenseLevel = Model.IncandescenseLevel;
                    cards.Add(newCardModel);
                }
            }
            
            foreach (var card in cards)
            {
                var cardPres = CardFactory.Instance.CreateBaseUICard(card, setView.GetCardListParent());
                cardPres.RegisterInputCallback(UserInput.HoverStarted, () => ShowCardInfo(card));
                cardPres.RegisterInputCallback(UserInput.HoverEnded, HideCardInfo);
            }

            setView.SetName(Model.Name);
            setView.SetIncandescense(Model.IncandescenseLevel);
            setView.SetWorld(Model.WorldId);
            setView.SetArtwork(
                Resources.Load<Sprite>(
                    cards
                        .First().ArtworkPath)
            );
            setView.SetCardTypeNumber(
                cards.Count(card => card.Type == Constants.CreatureType),
                cards.Count(card => card.Type == Constants.ArtifactType),
                cards.Count(card => card.Type == Constants.ActionType));
        }

        private void HideCardInfo()
        {
            setView.HideCardHighlight();

        }

        private void ShowCardInfo(CardModel card)
        {
            card.IncandescenseLevel = Model.IncandescenseLevel;
            setView.ShowCardHighlight(card);
        }

        /*
        private void SetIncandescense(int incandescenseLevel)
        {
            string deckIncandescense;
            switch (incandescenseLevel)
            {
                case 0:
                    {
                        deckIncandescense = Constants.VoidIncandescense;
                        break;
                    }
                case 1:
                    {
                        deckIncandescense = Constants.ShinyIncandescense;
                        break;
                    }
                case 2:
                    {
                        deckIncandescense = Constants.BlazingIncandescense;
                        break;
                    }
                case 3:
                    {
                        deckIncandescense = Constants.OneiricIncandescense;
                        break;
                    }
                default:
                    {
                        deckIncandescense = Constants.VoidIncandescense;
                        break;
                    }

            }
            //deckView.SetIncandescense(AssetDatabase.Instance.GetIncandescenseRecord(deckIncandescense).deckFrame);
        }
*/
        public void Destroy()
        {
            setView.Destroy();
        }

        public void HideAllCards()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                setView.ShowNextCard(false,0);
            }
        }

        public void ShowNextCard(int cardShown)
        {
            setView.ShowNextCard(true, cardShown);
        }
        
        public void ShowSet(bool enable)
        {
            setView.ShowSet(enable);
        }
        
        public virtual void ToggleVisibility(bool enable, Action OnEndAction=null)
        {
            CoroutineHelper.Instance.StartCoroutineHelper(setView.ToggleVisibilityAnimation(enable, OnEndAction));
        }

        public bool IsVisible()
        {
            return setView.isVisible;
        }
        
        public void ToggleInfoButtonListener(bool enable, Action OnClickAction)
        {
            if(enable)
                setView.AddButtonInfoListener(OnClickAction);
            else
                setView.RemoveButtonInfoListener(OnClickAction);
        }
    }
}