using Dawnshard.Network;
using Dawnshard.Views;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dawnshard.Presenters
{
    /// <summary>
    /// This component is responsible for elaborating the zone model
    /// and set the zone view with it's parameters
    /// </summary>
    public class ZonePresenter : IZonePresenter
    {
        private readonly IZoneView zoneView;
        private readonly EventBusManager eventBusManager;

        private static Dictionary<int, ICardPresenter> cardPresenters = new();

        public ZoneModel Model { get; set; }

        public ZonePresenter(ZoneModel zoneModel, IZoneView zoneView, EventBusManager eventBusManager)
        {
            this.Model = zoneModel;
            this.zoneView = zoneView;
            this.eventBusManager = eventBusManager;

            eventBusManager.ZoneEventBus.Subscribe(Model.InstanceId, OnGameEventReceived);

            UpdateView();
        }

        private void OnGameEventReceived(IGameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case ZoneCardAddedEvent zoneCardAdded:
                {
                    AddCard(zoneCardAdded.CardId, zoneCardAdded.Card);
                    break;
                }
                case ZoneCardRemovedEvent zoneCardRemoved:
                {
                    RemoveCard(zoneCardRemoved.CardId);
                    break;
                }
                case UpdateBoardEvent updateBoard:
                {
                    UpdateView();
                    break;
                }
            }
        }

        public void UpdateView()
        {
            zoneView.ZoneId = Model.ZoneId;
            zoneView.SetMaxCards(Model.MaxCards);
            zoneView.SetNumCards(Model.NumCards);

            //Spawns the new cards on the zone. Static zones don't have card models
            foreach (var cardModel in Model.Cards)
            {
                var cardPresenter = cardPresenters.GetValueOrDefault(cardModel.InstanceId) ?? CreateCard(cardModel);
                cardPresenter.Model = cardModel;
                cardPresenter.SetDefaultHighlight();
                cardPresenter.UpdateView();
            }

            var presenters = new List<ICardPresenter>();

            foreach (var cardModel in Model.Cards)
            {
                var cardPresenter = cardPresenters.GetValueOrDefault(cardModel.InstanceId);

                if (cardPresenter != null)
                {
                    presenters.Add(cardPresenter);
                }
            }

            zoneView.SetCards(presenters);
        }

        public void AddCard(int cardId, CardModel cardModel = null)
        {
            Model.NumCards++;

            var cardPresenter = cardPresenters.GetValueOrDefault(cardId);

            if (Model.IsStaticZone)
            {
                zoneView.SetNumCards(Model.NumCards);
                if(cardPresenters.ContainsKey(cardId))
                {
                    cardPresenters.Remove(cardId);
                    cardPresenter.Destroy();
                }
            }
            else
            {
                //If the model is null...
                if (cardModel == null)
                {
                    //And the presenter is null too, it's a warning (probably static zone)
                    if (cardPresenter == null)
                    {
                        Debug.LogWarning($"Card Model and Card Presenter not found for {cardId}");
                        return;
                    }

                    //otherwise, get teh model from the presenter
                    cardModel = cardPresenter.Model;
                }
                //If the presenter is null and the model exists...
                else if (cardPresenter == null)
                {
                    //We need to spawn a new card
                    cardPresenter = CreateCard(cardModel);
                }

                Model.Cards.Add(cardModel);
            }

            if (Model.ZoneId == Constants.BoardZone)
            {
                CoroutineHelper.Instance.StartCoroutineHelper(UpdateViewAfterSeconds(1.7f));
            }
            else
            {
                UpdateView();
            }
        }

        private IEnumerator UpdateViewAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            UpdateView();
        }

        public static void ClearPresenters()
        {
            cardPresenters.Clear();
        }

        public static ICardPresenter GetCardPresenter(int cardId)
        {
            return cardPresenters.GetValueOrDefault(cardId, null);
        }

        public void RemoveCard(int cardId)
        {
            Model.NumCards--;

            if (Model.IsStaticZone)
            {
                zoneView.SetNumCards(Model.NumCards);
                UpdateView();
                return;
            }
            
            var cardPresenter = cardPresenters.GetValueOrDefault(cardId);

            if (cardPresenter == null)
            {
                Debug.LogWarning($"Card Model and Card Presenter not found for {cardId}");
                return;
            }

            var cardModel = cardPresenter.Model;

            Model.Cards.Remove(cardModel);

            if (Model.ZoneId == Constants.GraveyardZone || cardModel.Keywords.Contains(Constants.EphemeralKeyword))
            {
                var cardToRemove = cardPresenters.GetValueOrDefault(cardId);
                cardPresenters.Remove(cardId);
                cardToRemove.Destroy();
            }

            UpdateView();
        }

        public bool IsZoneInteractable()
        {
            return Model.ZoneId != Constants.GraveyardZone
                   && Model.ZoneId != Constants.DeckZone
                   && Model.ZoneId != Constants.ActionZone;
        }

        public void UpdateCardsModels()
        {
            foreach (var cardModel in Model.Cards)
            {
                var cardPresenter = cardPresenters.GetValueOrDefault(cardModel.InstanceId) ?? CreateCard(cardModel);
                cardPresenter.Model = cardModel;
                cardPresenter.SetDefaultHighlight();
            }
        }

        private ICardPresenter CreateCard(CardModel cardModel)
        {
            ICardPresenter presenter;

            //TODO: spawna le carte chiedendo alla view
            if (cardModel.IsOwnerLocalPlayer)
            {
                presenter = CardFactory.Instance.CreateUserCard(cardModel, eventBusManager, zoneView.GetCardParent());
            }
            else
            {
                presenter = CardFactory.Instance.CreateGameCard(cardModel, eventBusManager, zoneView.GetCardParent());
            }

            cardModel.ZoneId = Model.ZoneId;

            //cardPresenters.Add(presenter);
            cardPresenters[cardModel.InstanceId] = presenter;

            return presenter;
        }
    }
}
