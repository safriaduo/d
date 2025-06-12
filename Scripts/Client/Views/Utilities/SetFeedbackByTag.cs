using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views
{
    public class SetFeedbackByTag : MonoBehaviour
    {
        [SerializeField] private MMFeedbackDestinationTransform feedback;
        [SerializeField] private string originTag;
        [SerializeField] private string destinationTag;

        private void OnEnable()
        {
            if (feedback.ForceOrigin)
                feedback.Origin = GameObject.FindGameObjectWithTag(originTag).transform;

            feedback.Destination = GameObject.FindGameObjectWithTag(destinationTag).transform;
        }
    }
}
