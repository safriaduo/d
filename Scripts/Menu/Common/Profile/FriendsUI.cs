using Nakama;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FriendsUI : MonoBehaviour
{
    public FriendEntry userEntryPrefab;
    public Transform onlineParent;
    public Transform offlineParent;
    public Color onlineColor;
    public Color offlineColor;

    private List<FriendEntry> spawnedEntries = new List<FriendEntry>();

    public void ClearEntries()
    {
        foreach (var entry in spawnedEntries)
        {
            Destroy(entry.gameObject);
        }
        spawnedEntries.Clear();
    }

    public async void LoadFriendsAsync()
    {
        ClearEntries();
        var friendResponse = await FriendsAPI.GetFriends();

        foreach (var friend in friendResponse.Friends)
        {
            var parent = friend.User.Online ? onlineParent : offlineParent;
            var entry = Instantiate(userEntryPrefab, parent);
            entry.Initialize(friend.User, friend.User.Online ? onlineColor : offlineColor, friend.State);
            entry.SetMatchInviteAvailable(friend.User.Online);
            spawnedEntries.Add(entry);
        }
    }

}