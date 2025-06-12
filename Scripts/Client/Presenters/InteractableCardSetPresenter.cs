using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Views;
using UnityEngine;

namespace Dawnshard.Presenters
{
    public class InteractableCardSetPresenter : CardSetPresenter
    {
        protected InteractableCardSetView InteractableCardSetView => setView as InteractableCardSetView;
        
        public InteractableCardSetPresenter(CardSetView view, CardSetModel model, bool noCards = false) : base(view, model, noCards)
        {
        }
        public void ToggleButtonListener(bool enable, Action OnClickAction, bool removeAll = false)
        {
            if(removeAll)
                InteractableCardSetView.RemoveAllButtonListener();
            else if(enable)
                InteractableCardSetView.AddButtonListener(OnClickAction);
            else
                InteractableCardSetView.RemoveButtonListener(OnClickAction);
        }

        public void EnableInteraction(bool enable)
        {
            InteractableCardSetView.EnableInteraction(enable);
        }

        public override void ToggleVisibility(bool enable, Action OnEndAction=null)
        {
            CoroutineHelper.Instance.StartCoroutineHelper(InteractableCardSetView.ToggleVisibilityAnimation(enable, OnEndAction));
        }

        public void Click()
        {
            InteractableCardSetView.Click();
        }
    }
}