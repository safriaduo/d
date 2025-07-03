using System;
using Nakama;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using Dawnshard.Menu;

public class SystemNotificationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private MMFeedbacks newNotificationFeedback;

    private IApiNotification currentNotification;

    private class FriendlyMatchInviteData
    {
        [JsonProperty("matchId")]
        public string MatchId;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameController.Instance != null && GameController.Instance.Socket != null);
        GameController.Instance.Socket.ReceivedNotification += OnNotificationReceived;
    }

    private void OnDestroy()
    {
        if (GameController.Instance != null && GameController.Instance.Socket != null)
        {
            GameController.Instance.Socket.ReceivedNotification -= OnNotificationReceived;
        }
    }

    private void OnNotificationReceived(IApiNotification notification)
    {
        ClearNotification();

        currentNotification = notification;

        if (notification.Code == -3)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<FriendlyMatchInviteData>(notification.Content);
                FriendlyMatchManager.StartFriendlyMatch(notification.Subject, data.MatchId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return;
        }

        if (notificationText != null)
        {
            notificationText.text = notification.Subject;
        }

        bool requiresResponse = notification.Code == -2;

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
            await FriendsAPI.AcceptFriend(userId: currentNotification.SenderId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        ShowStatusText("Accepted");
    }

    private async void RejectRequest()
    {
        if (currentNotification == null)
            return;

        try
        {
            await FriendsAPI.RemoveFriend(userId: currentNotification.SenderId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        ShowStatusText("Declined");
    }

    private void ClearNotification()
    {
        if (notificationText != null)
        {
            notificationText.text = string.Empty;
        }
        if (statusText != null)
        {
            statusText.gameObject.SetActive(false);
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

    private void ShowStatusText(string text)
    {
        statusText.text = text;
        statusText.gameObject.SetActive(true);
        rejectButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(false);
    }
}
