using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Dawnshard.Network
{
    public class DailyQuestModel
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("title")] public string Title;
        [JsonProperty("description")] public string Description;
        [JsonProperty("type")] public string Type;
        [JsonProperty("maxProgress")] public int MaxProgress;
        [JsonProperty("reward")] public RewardModel Reward;
        [JsonProperty("isTutorial")] public bool IsTutorial;

        [JsonIgnore] public int CurrentProgress;

        public DailyQuestModel()
        {
            CurrentProgress = 0;
        }
    }
}