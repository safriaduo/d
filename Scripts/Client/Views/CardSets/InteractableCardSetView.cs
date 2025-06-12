using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

namespace Dawnshard.Views
{
public class InteractableCardSetView : CardSetView
{
    [SerializeField] private Button setButton;
    
    public void AddButtonListener(Action OnClickAction)
    {
        setButton.onClick.AddListener(() => OnClickAction());
    }

    public void RemoveButtonListener(Action OnClickAction)
    {
        setButton.onClick.RemoveListener(() => OnClickAction());
    }
    
    public void RemoveAllButtonListener()
    {
        setButton.onClick.RemoveAllListeners();
    }

    public void EnableInteraction(bool enable)
    {
        setButton.interactable = enable;
    }

    public void Click()
    { 
        setButton.onClick?.Invoke();
    }

    public override IEnumerator ToggleVisibilityAnimation(bool show, Action OnEndAction = null)
    {
        if(setButton==null)
            yield break;
        setButton.enabled = false;
        if (!isVisible && show)
        {
            isVisible = true;
            showAnimation.Direction = MMFeedbacks.Directions.TopToBottom;
            showAnimation.PlayFeedbacks();
            yield return new WaitUntil(() => !showAnimation.IsPlaying);
            OnEndAction?.Invoke();
            GetComponent<CanvasGroup>().alpha = 1;
        }
        else if(isVisible && !show)
        {
            isVisible = false;
            showAnimation.Direction = MMFeedbacks.Directions.BottomToTop;
            showAnimation.PlayFeedbacks();
            yield return new WaitUntil(() => !showAnimation.IsPlaying);
            OnEndAction?.Invoke();
        }
        setButton.enabled = true;
    }
}
}