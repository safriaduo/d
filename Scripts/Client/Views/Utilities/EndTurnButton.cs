using System;
using System.Collections;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Dawnshard.Views
{
    public class EndTurnButton : MonoBehaviour
    {

        public UnityEvent onClick;
        
        [SerializeField] private TMP_Text text;
        [SerializeField] private MMFeedbacks noMoreActionAnimationLoop;
        [SerializeField] private MMFeedbacks moreActionAnimationLoop;
        [SerializeField] private MMFeedbacks opponentTurnAnimation;
        [SerializeField] private MMFeedbacks hoverAnimation;
        [SerializeField] private MMFeedbacks mouseDownAnimation;
        [SerializeField] private MMFeedbacks mouseUpAnimation;
        [SerializeField] private GameObject showIsMoreAction;

        private bool isNoMoreAction;
        private bool isButtonDown = false;
        private bool interactable = true;

        public void SetInteractable(bool enabled)
        {
            if (enabled)
            {
                mouseDownAnimation.PlayFeedbacks();

                IEnumerator LocalTurnButton()
                {
                    yield return new WaitForSeconds(.2f);
                    SetNoMoreActionAnimationLoop(false);
                    interactable = enabled;
                }
                
                StartCoroutine(LocalTurnButton());
            }
            else
            {
                noMoreActionAnimationLoop.StopFeedbacks();
                opponentTurnAnimation.PlayFeedbacks();
                isNoMoreAction = true;
                interactable = enabled;

            }
        }
        
        public void OnMouseEnter()
        {
            if (interactable)
            {
                hoverAnimation.PlayFeedbacks();
            }
            
        }

        public void SetText(string textStr)
        {
            text.text = textStr;
        }

        public void SetNoMoreActionAnimationLoop(bool enabled)
        {
            if (enabled && !isNoMoreAction)
            {
                moreActionAnimationLoop.StopFeedbacks();
                noMoreActionAnimationLoop.PlayFeedbacks();
                isNoMoreAction = true;
            }
            else if(!enabled && isNoMoreAction)
            {
                noMoreActionAnimationLoop.StopFeedbacks();
                moreActionAnimationLoop.PlayFeedbacks();
                isNoMoreAction = false;
            }
        }

        private void OnMouseDown()
        {
            if(TutorialManager.currentTutorialStep!=TutorialManager.TutorialSteps.NoTutorial && !isNoMoreAction)
            {
                IEnumerator HideAfter4Seconds()
                {
                    showIsMoreAction.SetActive(true);
                    yield return new WaitForSeconds(4f);
                    showIsMoreAction.SetActive(false);
                }
                StartCoroutine(HideAfter4Seconds());
                return;
            }
            if (UIManager.CurrentStateUI != UIManager.StateUI.None)
                return;
           
            Debug.Log("End turn clicked");

            if (interactable)
            {
                mouseDownAnimation.PlayFeedbacks();
                onClick?.Invoke();
                isButtonDown = true;
            }
        }

        private void OnMouseUp()
        {
            if (UIManager.CurrentStateUI != UIManager.StateUI.None)
                return;
           
            Debug.Log("End turn clicked");

            if (isButtonDown)
            {
                mouseUpAnimation.PlayFeedbacks();
                isButtonDown = false;
            }
        }
    }
}