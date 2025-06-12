using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawnshard.Network;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

public class RankAPI
{
    private const string RANK_COLLECTION = "rank";
    private const string RANK_PATH = "Rank/ranks";
    private const string LEADERBOARD_ID = "Ascendant";


    public static List<RankModel> Ranks { get; private set; } = new();
    public static RankEntryModel RankEntry { get; private set; }
    public static int RankPosition { get; private set; }
    
    private static List<RankModel> LoadDatabase()
    {
        var ranks = Resources.Load<TextAsset>(RANK_PATH);
        
        return JsonConvert.DeserializeObject<List<RankModel>>(ranks.text);
    }
    
    /// <summary>
    /// Loads the rank of the player
    /// </summary>
    public static async Task LoadPlayerRank()
    {
        Ranks = LoadDatabase();
        var userMetadata = (await GameController.Instance.LoadUserMetadata());
        RankEntry = userMetadata.PlayerRank;
        IsPlayerAscendantRank = userMetadata.IsMaxRank;
        if(IsPlayerAscendantRank)
        {
            RankPosition =
                int.Parse((await GameController.Instance.GetOwnerLeaderboardAsync()).Records.First(record=>record.Username == GameController.Username).Rank);
        }        
        else
            RankPosition = 0;
    }

    public static bool IsPlayerAscendantRank { get; private set; }
    
}
