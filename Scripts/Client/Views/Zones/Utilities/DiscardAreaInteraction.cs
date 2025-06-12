using System;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dawnshard.Views {
    public class DiscardAreaInteraction : MonoBehaviour {
        public Action OnPointerUpOnGraveyard;
        [SerializeField] MMFeedbacks growGraveyardEffectAnimation;
        [SerializeField] MMFeedbacks shrinkGraveyardEffectAnimation;
        [SerializeField] MMFeedbacks onShowGraveyard;
        [SerializeField] MMFeedbacks onHideGraveyard;
        [SerializeField] MMFeedbacks miniGrowGraveyardEffectAnimation;
        [SerializeField] MMFeedbacks miniShrinkGraveyardEffectAnimation;
        //bool ignoreMouse = false;
        private bool playingHover = false;
        private bool playingHint = false;



        public void OnMouseEnter()
        {
            if(UIManager.CurrentStateUI!=UIManager.StateUI.None)
                return;
            MiniGrow();
        }

        public void OnMouseExit()
        {
            MiniShrink();
        }

        public void MiniGrow()
        {
            if (playingHint) return;
            miniGrowGraveyardEffectAnimation.PlayFeedbacks();
            playingHint = true;
        }

        public void Shrink()
        {
            if(!playingHover) return;
            growGraveyardEffectAnimation.StopFeedbacks();
            playingHover = false;
            shrinkGraveyardEffectAnimation.PlayFeedbacks();
        }

        public void Grow()
        {
            if(playingHover || !playingHover) return;
            playingHover = true;
            growGraveyardEffectAnimation.PlayFeedbacks();
        }

        public void MiniShrink()
        {
            if (!playingHint) return;
            if(playingHover) miniGrowGraveyardEffectAnimation.StopFeedbacks();
            playingHint = false;
            miniShrinkGraveyardEffectAnimation.PlayFeedbacks();
        }

        public void OnMouseUp()
        {
            if(UIManager.CurrentStateUI!=UIManager.StateUI.None)
                return;
            UIManager.CurrentStateUI = UIManager.StateUI.Graveyard;
            OnPointerUpOnGraveyard?.Invoke();
           //onShowGraveyard.PlayFeedbacks();
        }

        public void OnHide()
        {
            //onHideGraveyard.PlayFeedbacks();
            UIManager.CurrentStateUI = UIManager.StateUI.None;
        }
    }
}
