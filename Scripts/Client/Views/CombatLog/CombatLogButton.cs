using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class CombatLogButton : MonoBehaviour
{
    [SerializeField] private MMFeedbacks showLog;
    [SerializeField] private MMFeedbacks rotateArrow;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            ShowCombatLog();
            ChangeArrowDirection();
        });
    }

    private void ShowCombatLog()
    {
        showLog.PlayFeedbacks();
    }

    private void ChangeArrowDirection()
    {
        rotateArrow.PlayFeedbacks();
    }
}
