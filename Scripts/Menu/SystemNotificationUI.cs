using System;
using Nakama;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemNotificationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private MMFeedbacks newNotificationFeedback;

    private IApiNotification currentNotification;

    private void OnEnable()
    {
        if (GameController.Instance != null && GameController.Instance.Socket != null)
        {
            GameController.Instance.Socket.ReceivedNotification += OnNotificationReceived;
        }
    }

    private void OnDisable()
    {
        if (GameController.Instance != null && GameController.Instance.Socket != null)
        {
            GameController.Instance.Socket.ReceivedNotification -= OnNotificationReceived;
        }
    }

    private void OnNotificationReceived(IApiNotification notification)
    {
        currentNotification = notification;

        if (notificationText != null)
        {
            notificationText.text = notification.Subject;
        }

        bool requiresResponse = notification.Code == 1;

        if (acceptButton != null)
        {
            acceptButton.gameObject.SetActive(requiresResponse);
            acceptButton.onClick.RemoveAllListeners();
            if (requiresResponse)
            {
                acceptButton.onClick.AddListener(AcceptRequest);
            }
        }

        if (rejectButton != null)
        {
            rejectButton.gameObject.SetActive(requiresResponse);
            rejectButton.onClick.RemoveAllListeners();
            if (requiresResponse)
            {
                rejectButton.onClick.AddListener(RejectRequest);
            }
        }

        newNotificationFeedback?.PlayFeedbacks();
    }

    private async void AcceptRequest()
    {
        if (currentNotification == null)
            return;

        try
        {
            await FriendsAPI.AcceptFriend(currentNotification.SenderUsername);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        ClearNotification();
    }

    private async void RejectRequest()
    {
        if (currentNotification == null)
            return;

        try
        {
            await FriendsAPI.RemoveFriend(currentNotification.SenderUsername);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        ClearNotification();
    }

    private void ClearNotification()
    {
        if (notificationText != null)
        {
            notificationText.text = string.Empty;
        }
        if (acceptButton != null)
        {
            acceptButton.gameObject.SetActive(false);
        }
        if (rejectButton != null)
        {
            rejectButton.gameObject.SetActive(false);
        }

        currentNotification = null;
    }
}
