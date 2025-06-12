using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Presenters;
using Dawnshard.Views;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class InteractableDeckView : DeckView
{
    [SerializeField] private Button deckButton;
    [SerializeField] private MMFeedbacks hoverFeedback;
    [SerializeField] private MMFeedbacks selectFeedback;

    public void AddButtonListener(Action OnClickAction)
    {
        deckButton.onClick.AddListener(() => OnClickAction());
    }

    public void RemoveButtonListener(Action OnClickAction)
    {
        deckButton.onClick.RemoveListener(() => OnClickAction());
    }

    /*public Action<UserInput> OnUserInput { get; set; }

    #region Interactions
    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
        OnUserInput?.Invoke(UserInput.HoverStarted);
    }

    private void OnMouseExit()
    {
        OnUserInput?.Invoke(UserInput.HoverEnded);
    }

    private void OnMouseDown()
    {
        OnUserInput?.Invoke(UserInput.MouseDown);
    }

    #endregion*/

    public void EnableInteraction(bool enable)
    {
        deckButton.interactable = enable;
    }
}
