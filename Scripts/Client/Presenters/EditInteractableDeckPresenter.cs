using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Network;
using Dawnshard.Presenters;
using UnityEngine;

public class EditInteractableDeckPresenter : InteractableDeckPresenter
{
    protected EditInteractableDeckView editInteractableDeckView => deckView as EditInteractableDeckView;
    public EditInteractableDeckPresenter(EditInteractableDeckView view, DeckModel model) : base(view, model)
    {
        //view.OnUserInput += OnUserInput;
    }

    public void ToggleEditListener(bool enable, Action<string> OnClickAction)
    {
        if(enable)
            editInteractableDeckView.AddEditListener(OnClickAction);
        else
            editInteractableDeckView.RemoveEditListener(OnClickAction);
    }

    public void EnableEditInteraction(bool enable)
    {
        editInteractableDeckView.EnableEditInteraction(enable);
    }
}
