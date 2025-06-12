using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// Handles the animations of the reap zone
    /// </summary>
    public class ReapAreaInteraction : MonoBehaviour
    {
        [SerializeField] private MMFeedbacks lightCircles;
        [SerializeField] private MMFeedbacks heavyCircles;
        [SerializeField] private MMFeedbacks stopHeavyCircles;
        [SerializeField] private MMFeedbacks stopCircles;
        private bool playingHover = false;
        private bool playingHint = false;

        public void PlayHint()
        {
            if (playingHint) return;
            lightCircles.PlayFeedbacks();
            playingHint = true;
        }

        public void PlayReapHover()
        {
            if(playingHover||!playingHint) return;
            heavyCircles.PlayFeedbacks();
            playingHover = true;
        }

        public void StopReapHover()
        {
            if(!playingHover) return;
            heavyCircles.StopFeedbacks();
            stopHeavyCircles.PlayFeedbacks();
            playingHover = false;
        }

        public void StopHint()
        {
            if (!playingHint) return;
            if(playingHover) StopReapHover();
            lightCircles.StopFeedbacks();
            stopCircles.PlayFeedbacks();
            playingHint = false;
        }
    }
}
