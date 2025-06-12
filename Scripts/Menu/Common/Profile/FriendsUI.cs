using Nakama;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FriendsUI : MonoBehaviour
{
    public FriendEntry userEntryPrefab;
    public Transform entryParent;
    public Color onlineColor;
    public Color offlineColor;

    private List<UserEntry> spawnedEntries = new List<UserEntry>();

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
        var friendResponse = await GameController.Instance.GetFriends();

        foreach (var friend in friendResponse.Friends)
        {
            var entry = Instantiate(userEntryPrefab, entryParent);
            entry.Initialize(friend.User, friend.User.Online ? onlineColor : offlineColor, friend.State);
            spawnedEntries.Add(entry);
        }
    }

}