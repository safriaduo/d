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
        if(isOpen)
            return;
        playerNameText.text = GameController.Username;
        titleText.text = title;
        isOpen = true;
        openFeedback.PlayFeedbacks();
    }

    public void CloseMenu()
    {
        if(!isOpen)
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
        {},"");
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

        // optionButtonView.SetOptions(new Dictionary<string, Action>()
        // {
        //     { "Add Friend", OpenAddFriendPopup },
        //     { "Referral",  OpenReferralPopup },
        // }, false);

        OpenMenu("Your Friends");

        //friendsUI.LoadFriendsAsync();
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

        optionButtonView.SetOptions(new Dictionary<string, Action>(),"");
        // optionButtonView.SetOptions(new Dictionary<string, Action>()
        // {
        //     { "Add Friend", OpenAddFriendPopup },
        //     { "Referral",  OpenReferralPopup },
        // }, false);

        OpenMenu("Profile");
        profileParent.SetActive(true);
        
        //friendsUI.LoadFriendsAsync();
    }


    private void OpenReferralPopup()
    {
        OpenPopup<PopupInputField>(popup =>
        {
            string referralId = GameController.Instance.Session.UserId;
            popup.buttonText.text = "Copy";
            popup.button.onClick.AddListener(() => GUIUtility.systemCopyBuffer = referralId );
            popup.text.text = "Send this to your friend!";
            popup.inputField.text = referralId;
        });
    }

    private void OpenAddFriendPopup()
    {
        OpenPopup<PopupInputField>(popup =>
        {
            popup.buttonText.text = "Invite";
            popup.button.onClick.AddListener(() => FriendsAPI.AddFriend(popup.inputField.text));
            popup.text.text = "Type the username of a player";
        });
    }
    
    private void OpenPopup<T>(Action<T> onOpened = null) where T : Popup    {
        popupInputField.gameObject.SetActive(true);
        popupInputField.transform.SetParent(popupInputField.transform, false);
        if (onOpened != null)
        {
            onOpened(popupInputField.GetComponent<T>());
        }
    }
    
    public void ClosePopup()
    {
        if (popupInputField != null)
        {
            popupInputField.gameObject.SetActive(false);
            popupInputField = null;
        }
    }

    public void OnPopupClosed(Popup popup)
    {
        if (popup == popupInputField)
            popupInputField = null;
    }
}
