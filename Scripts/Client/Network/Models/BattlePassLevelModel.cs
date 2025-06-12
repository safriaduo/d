using Dawnshard.Database;
using UnityEngine;

namespace Dawnshard.Network
{
    public class BattlePassLevelModel
    {
        /// <summary>
        /// The reward given once reached
        /// </summary>
        
        public RewardModel Reward { get; set; }

        /// <summary>
        /// Is only for premium users?
        /// </summary>
        public bool IsPremium { get; set; }

        /// <summary>
        /// The experience you need to get from the previous level
        /// </summary>
        public int LevelUpExperience {get;set;}

        /// <summary>
        /// Constructor.
        /// </summary>
        public BattlePassLevelModel(BattlePassLevelDTO levelDto)
        {
            Reward=new RewardModel(levelDto.Reward.ResourceId, levelDto.Reward.Amount);
            LevelUpExperience = levelDto.LevelUpExperience;
            IsPremium = levelDto.IsPremium;
        }
    }
}