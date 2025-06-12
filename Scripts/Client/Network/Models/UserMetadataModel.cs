using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Dawnshard.Network
{
    public class UserMetadataModel
    {
        [JsonProperty("is_max_rank")]
        public bool IsMaxRank = false;

        [JsonProperty("player_rank")]
        public string PlayerRankData = "";

        public RankEntryModel PlayerRank = new("",0,0, false);

        [JsonProperty("completed_tutorials")]
        public int CompletedTutorials = 0;

        [JsonProperty("last_online_time_unix")]
        public long LastOnlineTimeUnix = 0;



        // public UserMetadataModel(bool isMaxRank, string playerRank, int tutorialCompleted,  int lastOnlineTime)
        // {
        //     PlayerRankData = playerRank;
        //     IsMaxRank = isMaxRank; ;
        //     CompletedTutorials = tutorialCompleted;
        //     LastOnlineTimeUnix = lastOnlineTime;
        // }

        public void UnmarshalPlayerRank()
        {
            PlayerRank = JsonConvert.DeserializeObject<RankEntryModel>(PlayerRankData) ?? new RankEntryModel();
        }
    }
}