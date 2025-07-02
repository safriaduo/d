using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private LeaderboardUI leaderboardUI;
    [SerializeField] private FriendsUI friendsUI;
    [SerializeField] private DailyQuestContainer dailyQuestUI;
    [SerializeField] private OptionButtonView optionButtonView;
    [SerializeField] private MMFeedbacks openFeedback;
    [SerializeField] private PopupInputField popupInputField;
    [SerializeField] private GameObject profileParent;


    private bool isOpen = false;

    private void Start()
    {
        CloseMenu();
        profileParent.SetActive(false);
    }

    private void OpenMenu(string title)
    {
        if (isOpen)
            return;
        playerNameText.text = GameController.Username;
        titleText.text = title;
        isOpen = true;
        openFeedback.PlayFeedbacks();
    }

    public void CloseMenu()
    {
        if (!isOpen)
            return;
        isOpen = false;
        profileParent.SetActive(false);
        openFeedback.PlayFeedbacks();
    }

    public void OpenAscendantLeaderboard()
    {
        if (isOpen)
        {
            CloseMenu();
            return;
        }
        friendsUI.ClearEntries();
        leaderboardUI.ClearEntries();
        dailyQuestUI.ClearEntries();

        optionButtonView.SetOptions(new Dictionary<string, Action>()
        {
            { "Top 10",  ()=>leaderboardUI.LoadTopLeaderboardAsync() },
            { "You",  ()=>leaderboardUI.LoadOwnerLeaderboardAsync() },
        }, "Top 10");
        OpenMenu("Ascendant leaderboard");
        leaderboardUI.LoadTopLeaderboardAsync();
    }

    public void OpenDailyQuest()
    {
        if (isOpen)
        {
            CloseMenu();
            return;
        }
        friendsUI.ClearEntries();
        leaderboardUI.ClearEntries();

        dailyQuestUI.Initialize();
        optionButtonView.SetOptions(new Dictionary<string, Action>()
        { }, "");
        OpenMenu("Daily Quests");

    }

    public void OpenTournamentLeaderboard()
    {
        if (isOpen)
        {
            CloseMenu();
            return;
        }
        friendsUI.ClearEntries();
        leaderboardUI.ClearEntries();
        dailyQuestUI.ClearEntries();

        optionButtonView.SetOptions(new Dictionary<string, Action>()
        {
            { "Top 10", ()=>leaderboardUI.LoadTopLeaderboardAsync(Constants.WeekendTournament) },
            { "You",  ()=>leaderboardUI.LoadOwnerLeaderboardAsync(Constants.WeekendTournament) },
        }, "Top 10");
        OpenMenu(Constants.WeekendTournament);
        leaderboardUI.LoadTopLeaderboardAsync(Constants.WeekendTournament);
    }

    public void OpenFriendList()
    {
        if (isOpen)
        {
            CloseMenu();
            return;
        }
        friendsUI.ClearEntries();
        leaderboardUI.ClearEntries();
        dailyQuestUI.ClearEntries();

        optionButtonView.SetOptions(new Dictionary<string, Action>()
        {
            { "Add Friend", OpenAddFriendPopup },
        }, "");

        OpenMenu("Your Friends");

        friendsUI.LoadFriendsAsync();
    }

    public void OpenProfile()
    {
        if (isOpen)
        {
            CloseMenu();
            return;
        }
        friendsUI.ClearEntries();
        leaderboardUI.ClearEntries();
        dailyQuestUI.ClearEntries();

        optionButtonView.SetOptions(new Dictionary<string, Action>(), "");

        OpenMenu("Profile");
        profileParent.SetActive(true);
    }

    private void OpenAddFriendPopup()
    {
        ClearPopupInput();
        popupInputField.Open();
        popupInputField.buttonText.text = "Invite";
        popupInputField.button.onClick.RemoveAllListeners();
        popupInputField.button.onClick.AddListener(() => SendFriendRequest(popupInputField));
        popupInputField.text.text = "Add a new friend!";
    }

    private void ClearPopupInput()
    {
        popupInputField.inputField.text = "";
        popupInputField.ShowMessage("");
    }

    private async void SendFriendRequest(PopupInputField popup)
    {
        var username = popup.inputField.text;
        if (string.IsNullOrEmpty(username))
        {
            popup.ShowMessage("Enter a username");
            return;
        }

        try
        {
            await FriendsAPI.AddFriend(username);
            popup.ShowMessage("Friend request sent!");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            popup.ShowMessage(e.Message);
        }
    }
}
