using Dawnshard.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Database
{
    /// <summary>
    /// This is the database for assets used in the game. Used to have only one access point
    /// to the data and not scatter them around inside the scripts.
    /// </summary>
    public class AssetDatabase : MonoBehaviour
    {
        [SerializeField] private List<WorldRecord> worldRecords;
        [SerializeField] private List<RarityRecord> rarityRecords;
        [SerializeField] private List<RewardRecord> rewardRecords;
        [SerializeField] private List<RankRecord> rankRecords;
        [SerializeField] private List<IncandescenseRecord> incandescenseRecords;
        //[SerializeField] private List<LaunchArenaKeyRecord> launchArenaKeyRecords;

        public static AssetDatabase Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public RarityRecord GetRarityRecord(string rarityId)
        {
            var rarityRecord = rarityRecords.Find(w => w.rarityId == rarityId);
            if (rarityRecord == null)
            {
                Debug.LogError($"Cannot show the graphics for inexistent rarity id: {rarityId}. Please check puntaction");
                return null;
            }

            return rarityRecord;
        }

        public IncandescenseRecord GetIncandescenseRecord(string incandescenseId)
        {
            var incandescenseRecord = incandescenseRecords.Find(w => w.incandescenseId == incandescenseId);
            if (incandescenseRecord == null)
            {
                incandescenseRecord = incandescenseRecords.Find(w => w.incandescenseId == Constants.VoidIncandescense);

                Debug.LogWarning($"Cannot show the graphics for inexistent incandescense id: {incandescenseId}. Please check puntaction");
            }
            return incandescenseRecord;
        }

        public WorldRecord GetWorldRecord(string worldId)
        {
            var worldRecord = worldRecords.Find(w => w.worldId == worldId);
            if (worldRecord == null)
            {
                Debug.LogError($"Cannot show the graphics for inexistent world id: {worldId}. Please check puntaction");
                return null;
            }
            return worldRecord;
        }
        
        public RewardRecord GetRewardRecord(string rewardId)
        {
            var rewardRecord = rewardRecords.Find(r => r.rewardId == rewardId);
            if (rewardRecord == null)
            {
                Debug.LogError($"Cannot show the graphics for inexistent reward id: {rewardId}. Please check puntaction");
                return null;
            }
            return rewardRecord;
        }
        
        public RankRecord GetRankRecord(string rankID)
        {
            var rankRecord = rankRecords.Find(r => r.rankID == rankID);
            if (rankRecord == null)
            {
                Debug.LogError($"Cannot show the graphics for inexistent rank id: {rankID}. Please check puntaction");
                return null;
            }
            return rankRecord;
        }
        
        // public LaunchArenaKeyRecord GetLaunchArenaKeyRecord(int nftId)
        // {
        //     var launchRecord = launchArenaKeyRecords.Find(r => r.nftId == nftId);
        //     if (launchRecord == null)
        //     {
        //         Debug.LogError($"Cannot show the graphics for inexistent launchArenaKeyRecord id: {launchRecord.nftId}. Please check puntaction");
        //         return null;
        //     }
        //     return launchRecord;
        // }

    }
}