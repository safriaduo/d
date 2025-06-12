using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


namespace Dawnshard.Network
{
    public class RankEntryModel
    {
        [JsonProperty("subScore")]
        public int SubScore { get; set; }

        [JsonProperty("currentRank")]
        public string CurrentRank { get; set; }

        [JsonProperty("currentLevel")]
        public int CurrentLevel { get; set; }
        
        [JsonProperty("hasShield")]
        public bool HasShield { get; set; }

        public RankEntryModel(string currentRank, int currentLevel, int subScore, bool hasShield)
        {
            CurrentRank = currentRank;
            CurrentLevel = currentLevel;
            SubScore = subScore;
            HasShield = hasShield;
        }

        public RankEntryModel()
        {
            CurrentRank = "";
            CurrentLevel = 0;
            SubScore = 0;
            HasShield = false;
        }
    }
}