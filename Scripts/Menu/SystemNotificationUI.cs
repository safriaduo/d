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

        [JsonProperty("username")]
        public string Username;
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

        if (notificationText != null)
        {
            notificationText.text = notification.Subject;
        }

        bool requiresResponse = notification.Code == -2 || notification.Code == 201; //-2 is friend request, 201 is friendly challenge

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

        if (currentNotification.Code == 201)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<FriendlyMatchInviteData>(currentNotification.Content);
                FriendlyMatchManager.StartFriendlyMatch(data.Username, data.MatchId);
                MenuManager.Instance.GoToState(Constants.PlayState, true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowStatusText(e.Message);
                return;
            }
        }
        else if (currentNotification.Code == -2)
        {
            try
            {
                await FriendsAPI.AcceptFriend(userId: currentNotification.SenderId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowStatusText(e.Message);
                return;
            }
        }

        ShowStatusText("Accepted");
    }

    private async void RejectRequest()
    {
        if (currentNotification == null)
            return;

        if (currentNotification.Code == 201)
        {
            FriendlyMatchManager.Clear();
        }
        else if (currentNotification.Code == -2)
        {

            try
            {
                await FriendsAPI.RemoveFriend(userId: currentNotification.SenderId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowStatusText(e.Message);
                return;
            }
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
