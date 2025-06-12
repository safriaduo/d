using Dawnshard.Network;
using Dawnshard.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking.Types;

namespace Dawnshard.Presenters
{
    /// <summary>
    /// This class is the presenter of a card view. It's responsible to communicate game updates the card view 
    /// </summary>
    public class BaseCardPresenter : IBaseCardPresenter
    {

        protected Dictionary<UserInput, Action> callbackByInput = new();

        protected readonly ICardView cardView;

        public CardModel Model { get; set; }

        public BaseCardPresenter(ICardView view, CardModel model)
        {
            cardView = view;
            Model = model;
            
            cardView.OnUserInput += OnUserInput;

            SetStaticFields();
        }


        public virtual void OnUserInput(UserInput input)
        {
            foreach (var callback in callbackByInput.ToList())
            {
                if (callback.Key == input)
                    callback.Value.Invoke();
            }
        }

        public void RegisterInputCallback(UserInput input, Action callback)
        {
            if (!callbackByInput.ContainsKey(input))
                callbackByInput.Add(input, callback);
            else
                Debug.LogWarning("You're trying to add the same input two times");
        }

        public void UnregisterInputCallback(UserInput input)
        {
            callbackByInput.Remove(input);
        }

        /// <summary>
        /// Sets the card fields that are not going to change throught the match
        /// </summary>
        public void SetStaticFields()
        {
            cardView.SetCardName(Model.Name);
            cardView.SetCardBodyText(Model.BodyText);

            if (Model.ArtworkPath != null)
            {
                cardView.SetArtwork(Resources.Load<Sprite>(Model.ArtworkPath));
            }
            else
            {
                cardView.SetArtwork(null);
            }

            cardView.SetCardType(Model.Type, Model.IncandescenseLevel);
            cardView.SetRarity(Model.Rarity);
            cardView.SetWorld(Model.WorldId);

            foreach (var stat in Model.Stats)
            {
                cardView.SetStat(stat.ID, stat.OriginalValue, stat.OriginalValue, stat.OriginalValue);
            }
        }

        public virtual void ToggleHighlight(bool enabled, Color color = default)
        {
            cardView.ToggleHighlight(enabled, color);
        }

        public void SetKeywordList(bool enabled)
        {
            cardView.SetKeywordList(enabled, Model.Keywords, 
                Model.Abilities.Select(ability => ability.EffectId).ToList(), 
                Model.Abilities.ConvertAll(abilities  => abilities.EffectId).Contains(Constants.CreateCardEffect)?
                    Model.Abilities.FindAll(model => model.EffectId == Constants.CreateCardEffect).Select(ability => ability.EffectParameter.Split(",")[0]).ToList()
                :null);
        }

        public virtual void Destroy()
        {
            cardView.Destroy();
        }
        

    }
}