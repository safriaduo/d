using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Dawnshard.Network
{
    public class RewardModel
    {
        [JsonProperty("resourceId")]
        public string RewardId { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }

        public RewardModel(string rewardId, int amount)
        {
            RewardId = rewardId;
            Amount = amount;
        }
    }
}