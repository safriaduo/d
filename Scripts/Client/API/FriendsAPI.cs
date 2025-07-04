using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;

public static class FriendsAPI
{
    /// <summary>
    /// Retrieve the friend list for the current user.
    /// </summary>
    public static async Task<IApiFriendList> GetFriends()
    {
        return await GameController.Instance.Client.ListFriendsAsync(
            GameController.Instance.Session,
            null,
            100);
    }

    /// <summary>
    /// Send a friend request to the given username.
    /// Returns true if the request succeeded, false if the user was not found.
    /// Other exceptions will be propagated to the caller.
    /// </summary>
    public static async Task AddFriend(string username)
    {
        await GameController.Instance.Client.AddFriendsAsync(
            GameController.Instance.Session,
            new List<string>(),
            new List<string> { username });

    }

    /// <summary>
    /// Remove the given username from the friend list.
    /// </summary>
    public static async Task RemoveFriend(string username = null, string userId = null)
    {
        var ids = new List<string>();
        var usernames = new List<string>();

        if (userId != null)
        {
            ids.Add(userId);
        }

        if (username != null)
        {
            usernames.Add(username);
        }

        await GameController.Instance.Client.DeleteFriendsAsync(GameController.Instance.Session, ids, usernames);
    }

    /// <summary>
    /// Accept a pending friend request from the given username.
    /// </summary>
    public static async Task AcceptFriend(string username = null, string userId = null)
    {
        var ids = new List<string>();
        var usernames = new List<string>();

        if (userId != null)
        {
            ids.Add(userId);
        }

        if (username != null)
        {
            usernames.Add(username);
        }

        await GameController.Instance.Client.AddFriendsAsync(GameController.Instance.Session, ids, usernames);
    }

    private class FriendlyMatchPayload
    {
        [JsonProperty("friendId")]
        public string friendId;
    }

    /// <summary>
    /// Send a request for a friendly match to a friend
    /// </summary>
    public static async Task<string> SendFriendlyMatchRequest(string userId)
    {
        var matchId = await GameController.Instance.Socket.RpcAsync("friendly_match", JsonConvert.SerializeObject(new FriendlyMatchPayload() { friendId = userId }));
        return matchId.Payload;
    }

}
