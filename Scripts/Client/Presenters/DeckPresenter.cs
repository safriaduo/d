using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dawnshard.Database;
using Dawnshard.Network;
using Dawnshard.Views;
using Unity.VisualScripting;
using UnityEngine;


namespace Dawnshard.Presenters
{
    public class DeckPresenter
    {
        public DeckModel Model { get; set; }

        protected DeckView deckView;

        public DeckPresenter(DeckView view, DeckModel model)
        {
            deckView = view;
            Model = model;

            if (model != null)
            {
                UpdateView();
            }
        }

        public void UpdateView()
        {
            SetStaticFields();
        }

        private void SetStaticFields()
        {
            deckView.SetName(Model.Name);
            List<CardSetModel> cardSetModels = new List<CardSetModel>();
            foreach (var setId in Model.CardSetIds)
            {
                cardSetModels.Add(
                    CardSetAPI.CardSets.Find(set => set.ItemId == setId)
                    );
            }
            SetIncandescense(cardSetModels);
            deckView.SetWorlds(cardSetModels.ConvertAll(set => set.WorldId).ToArray());
            deckView.SetArtwork(
                Resources.Load<Sprite>(
                CardDatabase.GetCardByName(cardSetModels[0].Cards.First()).ArtworkPath)
            );
        }

        private void SetIncandescense(List<CardSetModel> cardSetModels)
        {
            if(cardSetModels==null)
                return;
            List<int> incandescenseLevel = cardSetModels.ConvertAll(set => Constants.IncandescenseStringToInt(set.IncandescenseLevel));
            incandescenseLevel.Sort();
            var deckIncandescense = incandescenseLevel.First();
            deckView.SetIncandescense(Constants.IncandescenseIntToString(deckIncandescense));
        }
        
        public void ToggleInfoButtonListener(bool enable, Action OnClickAction)
        {
            if(enable)
                deckView.AddButtonInfoListener(OnClickAction);
            else
                deckView.RemoveButtonInfoListener(OnClickAction);
        }
    }
}