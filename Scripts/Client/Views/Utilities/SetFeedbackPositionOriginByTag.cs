using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFeedbackPositionOriginByTag : MonoBehaviour
{
    [SerializeField] private MMFeedbackPosition feedback;
    [SerializeField] private string originTag;

    private void OnEnable() {
        feedback.InitialPositionTransform = GameObject.FindGameObjectWithTag(originTag).transform;
    }
}
