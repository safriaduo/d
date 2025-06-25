using Nakama;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FriendsUI : MonoBehaviour
{
    public FriendEntry userEntryPrefab;
    public Transform onlineParent;
    public Transform offlineParent;
    public Transform pendingParent;
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
        ToggleFriendsParent(false);
    }

    public async void LoadFriendsAsync()
    {
        ClearEntries();
        var friendResponse = await FriendsAPI.GetFriends();

        foreach (var friend in friendResponse.Friends)
        {
            Transform parent = friend.State switch
            {
                0 => friend.User.Online ? onlineParent : offlineParent,
                1 => pendingParent,
                2 => pendingParent,
                _ => offlineParent,
            };
            var entry = Instantiate(userEntryPrefab, parent);
            entry.Initialize(friend.User, friend.User.Online ? onlineColor : offlineColor, friend.State);
            spawnedEntries.Add(entry);
        }
        ToggleFriendsParent(true);
    }

    public void ToggleFriendsParent(bool toggle)
    {
        onlineParent.gameObject.SetActive(toggle && onlineParent.childCount > 1);
        offlineParent.gameObject.SetActive(toggle && offlineParent.childCount > 1);
        pendingParent.gameObject.SetActive(toggle && pendingParent.childCount > 1);
    }
}