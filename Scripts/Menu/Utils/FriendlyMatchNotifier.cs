using System.Collections.Generic;
using System.Linq;
using Dawnshard.Menu;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendlyMatchNotifier : MonoBehaviour
{
    [SerializeField] private FriendlyMatchPopup friendlyPopup;
    [SerializeField] private Button challengeButtonPrefab;
    [SerializeField] private Transform challengeButtonParent;

    private const float CHECK_INTERVAL = 5f;
    private float timer = 0f;

    private readonly List<Button> spawnedButtons = new();

    private async void Update()
    {
        if (GameController.Instance == null || GameController.Instance.Session == null)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        timer = CHECK_INTERVAL;

        var notifications = await GameController.Instance.ReadNotification();
        foreach (var notification in notifications)
        {
            if (notification.Code != 1)
                continue;
            if (notification.Content != null && notification.Content.ContainsKey("matchId"))
            {
                string matchId = notification.Content["matchId"].ToString();
                string username = notification.Content.ContainsKey("username") ? notification.Content["username"].ToString() : "";

                SpawnChallengeButton(matchId, username);
                await GameController.Instance.DeleteNotification(new List<string> { notification.Id });
            }
        }
    }

    private void SpawnChallengeButton(string matchId, string username)
    {
        var button = Instantiate(challengeButtonPrefab, challengeButtonParent);
        var text = button.GetComponentInChildren<TMPro.TMP_Text>();
        if (text != null)
            text.text = username + " has challenged you!";

        button.onClick.AddListener(() => OnChallengeButtonClicked(matchId, button));
        spawnedButtons.Add(button);
    }

    private void OnChallengeButtonClicked(string matchId, Button button)
    {
        friendlyPopup.OpenAsReceiver(matchId);
        spawnedButtons.Remove(button);
        Destroy(button.gameObject);
    }
}
