using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


namespace Dawnshard.Network
{
    public class RankModel
    {
        [JsonProperty("Name")]

        public string Name { get; set; }
        //levels are the RankLevels and they have as int the exp needed to level up
        [JsonProperty("Levels")]
        public List<RankLevel> Levels { get; set; }

        public class RankLevel
        {
            [JsonProperty("PointsToRankUp")]
            public int ExpToLevelUp { get; set; }

            public RankLevel(int exp)
            {
                ExpToLevelUp = exp;
            }
        }

        public RankModel(string name, List<RankLevel> levels)
        {
            Name = name;
            Levels = levels;
        }
    }
}