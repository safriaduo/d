using System.Collections.Generic;
using System.Threading.Tasks;
using Nakama;

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
    public static async Task<bool> AddFriend(string username)
    {
        try
        {
            await GameController.Instance.Client.AddFriendsAsync(
                GameController.Instance.Session,
                new List<string>(),
                new List<string> { username });
            return true;
        }
        catch (ApiResponseException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    /// <summary>
    /// Remove the given username from the friend list.
    /// </summary>
    public static async Task RemoveFriend(string username)
    {
        await GameController.Instance.Client.DeleteFriendsAsync(
            GameController.Instance.Session,
            new List<string>(),
            new List<string> { username });
    }
}
