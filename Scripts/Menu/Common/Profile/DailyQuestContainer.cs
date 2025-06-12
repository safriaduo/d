using System.Collections.Generic;
using System.Linq;
using Dawnshard.Network;
using UnityEngine;

public class DailyQuestContainer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DailyQuestView dailyQuestViewPrefab;
    [SerializeField] private Transform questParent;

    private List<DailyQuestView> dailyQuestsEntrySpawned = new List<DailyQuestView>();

    public void Initialize()
    {
        ClearEntries();
        PopulateDailyQuests();
    }

    private void PopulateDailyQuests()
    {
        var dailyQuests = DailyQuestAPI.userDailyQuest.Values; // This must be already initialized by the time this script runs

        foreach (var quest in dailyQuests)
        {
            var questView = Instantiate(dailyQuestViewPrefab, questParent);
            questView.ShowDailyQuest(
                quest.Title,
                quest.Description,
                quest.CurrentProgress,
                quest.MaxProgress,
                quest.Reward.RewardId,
                quest.Reward.Amount
            );
            dailyQuestsEntrySpawned.Add(questView);

            if (DailyQuestAPI.HasChangedQuests == false)
                questView.EnableChangeQuestButton(quest.Id);
        }
    }

    public void ClearEntries()
    {
        foreach (var entry in dailyQuestsEntrySpawned.ToList())
        {
            Destroy(entry.gameObject);
        }
        dailyQuestsEntrySpawned.Clear();
    }
}