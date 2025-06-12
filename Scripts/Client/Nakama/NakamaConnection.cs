using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Nakama;
using Nakama.TinyJson;
using Newtonsoft.Json;
using UnityEngine;

namespace Dawnshard.Network
{
    [Serializable]
    [CreateAssetMenu(fileName = "new Connection", menuName = "Nakama/NakamaConnection")]
    public class NakamaConnection : ScriptableObject
    {
        public string Scheme = "http";
        public string Host = "localhost";
        public int Port = 7350;
        public string ServerKey = "defaultkey";
        public string HttpKey = "defaultkey";

        //private const string SessionPrefName = "nakama.session";

        //public IClient Client;
        //public ISession Session;
        //public ISocket Socket;
        //public IApiAccount Account;

        ///// <summary>
        ///// Connects to the Nakama server using device authentication and opens socket for realtime communication.
        ///// </summary>
        //public async Task Connect()
        //{
        //    // Connect to the Nakama server.
        //    Client = new Client(Scheme, Host, Port, ServerKey, UnityWebRequestAdapter.Instance);

        //    // Attempt to restore an existing user session.
        //    var authToken = PlayerPrefs.GetString(SessionPrefName);

        //    if (!string.IsNullOrEmpty(authToken))
        //    {
        //        var session = Nakama.Session.Restore(authToken);
        //        if (!session.IsExpired)
        //        {
        //            Session = session;
        //        }
        //        else
        //        {
        //            Session = null;
        //        }
        //    }

        //    await OnSessionEstablished();
        //}

        ///// <summary>
        ///// Create the socket
        ///// </summary>
        //private async Task OnSessionEstablished()
        //{
        //    if (Session != null)
        //    {
        //        await RefreshAccount();

        //        // Open a new Socket for realtime communication.
        //        Socket = Client.NewSocket();

        //        await Socket.ConnectAsync(Session, true);
        //    }
        //}

        //private async Task RefreshAccount()
        //{
        //    Account = await Client.GetAccountAsync(Session);
        //    Debug.Log($"User \"{Account.User.Username}\" logged in. Wallet {Account.Wallet}");
        //}

        ///// <summary>
        ///// Login or register with a mail
        ///// </summary>
        //public async Task DeviceLogin()
        //{
        //    // Should use a platform API to obtain a device identifier.
        //    var deviceId = Guid.NewGuid().ToString();
        //    Session = await Client.AuthenticateDeviceAsync(deviceId);

        //    // Store the auth token that comes back so that we can restore the session later if necessary.
        //    PlayerPrefs.SetString(SessionPrefName, Session.AuthToken);
        //    await OnSessionEstablished();
        //}

        ///// <summary>
        ///// Login or register with a mail
        ///// </summary>
        //public async Task EmailLogin(string email, string password, string username = null, string referral = null, bool createUser = true)
        //{
        //    Dictionary<string, string> vars = null;

        //    if (referral != null)
        //    {
        //        vars = new Dictionary<string, string>()
        //    {
        //        { "referral_id", referral }
        //    };
        //    }

        //    Session = await Client.AuthenticateEmailAsync(email, password, username, createUser, vars);

        //    // Store the auth token that comes back so that we can restore the session later if necessary.
        //    PlayerPrefs.SetString(SessionPrefName, Session.AuthToken);
        //    await OnSessionEstablished();
        //}

        ///// <summary>
        ///// Logout from current session
        ///// </summary>
        //public async Task Logout()
        //{
        //    if (Session != null)
        //    {
        //        await Client.SessionLogoutAsync(Session);

        //        // Store the auth token that comes back so that we can restore the session later if necessary.
        //        PlayerPrefs.SetString(SessionPrefName, "");
        //    }
        //}


        ///// <summary>
        ///// Get all friends even pending and blocked
        ///// </summary>
        //public async Task<IApiFriendList> GetFriends()
        //{
        //    return await Client.ListFriendsAsync(Session, null, 100);
        //}

        ///// <summary>
        ///// Adds a friend from the list
        ///// </summary>
        //public async void AddFriend(string username)
        //{
        //    await Client.AddFriendsAsync(Session, new List<string>(), new List<string> { username });
        //}

        ///// <summary>
        ///// Remove a friend from your list
        ///// </summary>
        //public async void DeleteFriend(string username)
        //{
        //    await Client.DeleteFriendsAsync(Session, new List<string>(), new List<string> { username });
        //}

        ///// <summary>
        ///// Join a chat with a user
        ///// </summary>
        //public async Task<IChannel> JoinUserChat(string userId)
        //{
        //    return await Socket.JoinChatAsync(userId, ChannelType.DirectMessage, false);
        //}

        ///// <summary>
        ///// Leaves a chat with a user
        ///// </summary>
        //public async void LeaveUserChat(IChannel channel)
        //{
        //    await Socket.LeaveChatAsync(channel);
        //}

    }
}
