using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dawnshard.Views
{

    /// <summary>
    /// This component create a queue of feedbacks, where a feedback is played only
    /// if there aren't any other feedback like this playing
    /// </summary>
    [RequireComponent(typeof(MMFeedbacks))]
    public class FeedbackQueue : MonoBehaviour
    {
        // Coda delle animazioni
        private static readonly Queue<MMFeedbacks> animationQueue = new();

        private MMFeedbacks animationFeedback;

        private void Awake()
        {
            animationFeedback = GetComponent<MMFeedbacks>();

            animationFeedback.Events.OnPlay.AddListener(AddToQueue);
        }

        public void AddToQueue()
        {
            animationQueue.Enqueue(animationFeedback);

            // Se è l'unica animazione nella coda, la riproduciamo
            if (animationQueue.Count == 1)
            {
                PlayNextAnimation();
            }
            else
            {
                // otherwise it is paused
                animationFeedback.PauseFeedbacks();
            }
        }

        // Metodo per riprodurre l'animazione successiva
        private static void PlayNextAnimation()
        {
            if (animationQueue.Count > 0)
            {

                var nextAnimation = animationQueue.Peek();
                nextAnimation.ResumeFeedbacks();
                nextAnimation.Events.OnComplete.AddListener(OnAnimationCompleted);
            }
        }

        private static void OnAnimationCompleted()
        {
            // Rimuoviamo l'animazione dalla coda e riproduciamo la successiva
              animationQueue.Dequeue();

            PlayNextAnimation();
        }
    }
}
