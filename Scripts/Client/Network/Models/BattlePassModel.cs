using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawnshard.Menu;
using GameAnalyticsSDK;
using UnityEngine;


namespace Dawnshard.Network
{
    public class BattlePassModel
    {
        /// <summary>
        /// The name of this battle pass
        /// </summary>
        public string name;

        /// <summary>
        /// The cards of this card set.
        /// </summary>
        public List<BattlePassLevelModel> levels = new();

        public int CurrentExp = 0/* => GetRuntimeBattlePass().currentExp*/;

        public int CurrentLevel = -1 /*=> GetRuntimeBattlePass().currentLevel*/;

        public bool PremiumUnlocked = false /*=> GetRuntimeBattlePass().premiumUnlocked*/;


        public BattlePassModel(BattlePassDTO battlePassDTO)
        {
            name = battlePassDTO.Name;
            levels = battlePassDTO.Levels.ToList().ConvertAll(levelDTO => new BattlePassLevelModel(levelDTO));
        }

        /// <summary>
        /// Returns wheter the user can collect the level specified
        /// </summary>
        public async Task LevelUpAsync(BattlePassLevelModel battlePassLevel)
        {
            if (CanCollectLevel(battlePassLevel))
            {
                int levelID = levels.IndexOf(battlePassLevel);
                LoadingPopup.Instance.Open();
                var response = await BattlePassAPI.UnlockBattlePassLevel(levelID, name);
                LoadingPopup.Instance.Close();

                Debug.Log($"Collecting level {levelID} - Response : " + response.success + "\nMessage:" + response.message);

                if (response != null && response.success)
                {
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "BattlePassLevel", levelID.ToString(), 1);
                    CurrentLevel = levelID;
                }
            }
        }

        /// <summary>
        /// Returns wheter the user can collect the level specified
        /// </summary>
        public bool CanCollectLevel(BattlePassLevelModel battlePassLevel)
        {
            //Only the next level can be collected
            if (GetNextCollectableLevel() != battlePassLevel || (battlePassLevel == null && CurrentExp> GetLevelTotalExp(levels.Last())))
                return false;

            else
                return IsUnlockedLevel(battlePassLevel);
        }

        /// <summary>
        /// Returns the next level that is collectable or null if there is no next level
        /// </summary>
        public BattlePassLevelModel GetNextCollectableLevel()
        {
            for (int levelIdx = CurrentLevel+1; levelIdx < levels.Count; levelIdx++)
            {
                var level = levels[levelIdx];

                if (!level.IsPremium || PremiumUnlocked)
                {
                    return level;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the next level that is unlockable or null if there is no next level
        /// </summary>
        public int GetNextUnlockableLevel()
        {
            float currentExperience = CurrentExp;
            int nearestLevelIndex = -1;
            float nearestLevelExperience = float.MaxValue;

            for (int i = 0; i < levels.Count; i++)
            {
                float levelExp = GetLevelTotalExp(levels[i]);
                float experienceDifference = levelExp - currentExperience;
                if (experienceDifference > 0 && experienceDifference < nearestLevelExperience)
                {
                    nearestLevelIndex = i;
                    nearestLevelExperience = experienceDifference;
                }
            }

            return nearestLevelIndex;
        }

        /// <summary>
        /// Return wheter the user has unlocked the level specified
        /// An unlocked level is a level that the user has enough exp points to reach
        /// </summary>
        public bool IsUnlockedLevel(BattlePassLevelModel battlePassLevel)
        {
            return CurrentExp >= GetLevelTotalExp(battlePassLevel)
                   //&& (!battlePassLevel.IsPremium || battlePassLevel.IsPremium && PremiumUnlocked)
                   ;
        }

        /// <summary>
        /// Return the total exp points needed to get to specified level
        /// </summary>
        public int GetLevelTotalExp(BattlePassLevelModel battlePassLevel)
        {
            int levelIndex = levels.IndexOf(battlePassLevel);
            int totalExp = 0;

            for (int i = 0; i <= levelIndex; i++)
            {
                var currLevel = levels[i];
                totalExp += currLevel.LevelUpExperience;
            }

            return totalExp;
        }
    }
}
