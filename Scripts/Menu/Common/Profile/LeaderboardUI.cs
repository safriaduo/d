using Nakama;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    public LeaderboardEntry leaderboardEntryPrefab;
    public Transform entryParent;

    private List<LeaderboardEntry> spawnedEntries = new List<LeaderboardEntry>();

    public async void LoadTopLeaderboardAsync(string customLeaderboard=null)
    {
        IApiLeaderboardRecordList records;
        if(customLeaderboard != null)
            records = await GameController.Instance.GetTopLeaderboardAsync(leaderboardId:customLeaderboard);
        else
            records = await GameController.Instance.GetTopLeaderboardAsync();
        Initialize(records, customLeaderboard != null);
    }

    public async void LoadOwnerLeaderboardAsync(string customLeaderboard=null)
    {
        IApiLeaderboardRecordList records;
        if(customLeaderboard != null)
            records = await GameController.Instance.GetOwnerLeaderboardAsync(leaderboardId:customLeaderboard);
        else
            records = await GameController.Instance.GetTopLeaderboardAsync();
        Initialize(records, customLeaderboard != null);
    }

    private void Initialize(IApiLeaderboardRecordList recordList, bool winLoseNotation=false)
    {
        ClearEntries();
        if (!winLoseNotation)
        {
            foreach (var record in recordList.Records)
            {
                var score = record.Score ?? "???";
                var entry = Instantiate(leaderboardEntryPrefab, entryParent);
                entry.Initialize(record.Username, score,
                    record.Rank, record.OwnerId == GameController.Instance.Session.UserId);
                spawnedEntries.Add(entry);
            }
            return;
        }
        foreach (var record in recordList.Records)
        {
            var score = record.Score==null?0:int.Parse(record.Score);
            var entry = Instantiate(leaderboardEntryPrefab, entryParent);
            entry.Initialize(record.Username, $"{score}W-{record.NumScore - score}L",
                record.Rank, record.OwnerId == GameController.Instance.Session.UserId);
            spawnedEntries.Add(entry);
        }
    }

    public void ClearEntries()
    {
        foreach (var entry in spawnedEntries)
        {
            Destroy(entry.gameObject);
        }
        spawnedEntries.Clear();
    }
}
