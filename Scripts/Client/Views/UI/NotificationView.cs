using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationView : MonoBehaviour
{
    [SerializeField] private GameObject notificationIcon;

    private void Start()
    {
        //HideNotification();
    }

    public void HideNotification()
    {
        notificationIcon.SetActive(false);
    }

    public void ShowNotification()
    {
        notificationIcon.SetActive(true);
    }
}
 