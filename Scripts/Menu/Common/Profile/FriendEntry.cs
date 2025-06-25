
using Nakama;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendEntry : UserEntry
{
    public GameObject mutualFriendsParent;
    public GameObject pendingParent;
    public GameObject blockedParent;
    public GameObject invitationSentParent;

    private IApiUser user;

    public void Initialize(IApiUser user, Color color, int state)
    {
        this.user = user;

        base.Initialize(user.Username, color);
          

        switch (state)
        {
            //Mutual friends
            case 0:
                mutualFriendsParent.SetActive(true);
                break;

            //Sent invitation and pending acceptance
            case 1:
                invitationSentParent.SetActive(true);
                break;

            //Received invitation and pending acceptance
            case 2:
                pendingParent.SetActive(true);
                break;

            //Blocked user
            case 3:
                blockedParent.SetActive(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Respond to a pending invitation
    /// </summary>
    public async void RespondPendingInvitation(bool accept)
    {
        if (accept)
        {
            await FriendsAPI.AcceptFriend(user.Username);
            pendingParent.SetActive(false);
            mutualFriendsParent.SetActive(true);
        }
        else
        {
            await FriendsAPI.RemoveFriend(user.Username);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Opens the chat with the player
    /// </summary>
    public async void OpenChatAsync()
    {
        //TODO: add asset referencer and uncomment
        //var channel = await AssetReferencer.Instance.connection.JoinUserChat(user.Id);

        //MenuManager.Instance.topState.OpenPopup<PopupChat>("PopupChat", popup =>
        //{
        //    popup.Initialize(channel, user.Username);
        //});
    }

    /// <summary>
    /// Send an invitation to the player to fight
    /// </summary>
    public async void SendMatchInvitationAsync()
    {
        //add asset referencer and uncomment
        //NakamaConnection conn = AssetReferencer.Instance.connection;

        //var match = await conn.FindMatch(false);
        //conn.SendFriendlyMatchInvitation(user.Id, match.matchId);

        //FriendlyMatchState.StartFriendlyMatch(user.Username, match.matchId);
    }

    /// <summary>
    /// Remove this friend from the list
    /// </summary>
    public async void RemoveFriendAsync()
    {
        await FriendsAPI.RemoveFriend(user.Username);
        Destroy(gameObject);
    }
}
