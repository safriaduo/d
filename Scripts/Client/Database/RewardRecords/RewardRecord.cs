using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Database
{
    [CreateAssetMenu(fileName = "new Reward", menuName = "Dawnshard/Reward", order = -1)]

    public class RewardRecord : ScriptableObject
    {
        public string rewardId;
        public Sprite rewardIcon;
    }
}
