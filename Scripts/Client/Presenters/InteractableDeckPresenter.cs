
using System;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Views;
using safriaduo.UI;
using UnityEngine;

namespace Dawnshard.Presenters
{
    public class InteractableDeckPresenter : DeckPresenter
    {
        //protected Dictionary<UserInput, Action> callbackByInput = new();
        protected InteractableDeckView interactableDeckView => deckView as InteractableDeckView;
        public InteractableDeckPresenter(InteractableDeckView view, DeckModel model) : base(view, model)
        {
            //view.OnUserInput += OnUserInput;
        }

        public void ToggleButtonListener(bool enable, Action OnClickAction)
        {
            if(enable)
                interactableDeckView.AddButtonListener(OnClickAction);
            else
                interactableDeckView.RemoveButtonListener(OnClickAction);
        }

        public void EnableInteraction(bool enable)
        {
            interactableDeckView.EnableInteraction(enable);
        }
        /*public void RegisterInputCallback(UserInput input, Action callback)
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
        
        public virtual void OnUserInput(UserInput input)
        {
            foreach (var callback in callbackByInput)
            {
                if (callback.Key == input)
                    callback.Value.Invoke();
            }
        }*/
    }
}