using Dawnshard.Network;
using MoreMountains.Feel;
using Nakama;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAnalyticsSDK;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public static class BattlePassAPI
{
    private const string UNLOCK_BATTLE_PASS_LEVEL_RPC = "unlock_battle_pass_level";
    private const string BATTLE_PASS_COLLECTION = "battle_pass";
    private const string BATTLE_PASS_PATH = "BattlePass/battle_pass";

    public static BattlePassModel BattlePass { get; private set; }

    private class BattlePassEntry
    {
        [JsonProperty("currentExp")]
        public int currentExperience;

        [JsonProperty("currentLevel")]
        public int currentLevel;

        [JsonProperty("premiumUnlocked")]
        public bool premiumUnlocked;
    }

    private static BattlePassDTO LoadDatabase()
    {
        var battlePassText = Resources.Load<TextAsset>(BATTLE_PASS_PATH);

        var battlePassRecord = Newtonsoft.Json.JsonConvert.DeserializeObject<BattlePassDTO>(battlePassText.text);

        return battlePassRecord;
    }

    /// <summary>
    /// Loads all the assets of the player from the server
    /// </summary>
    public static async Task LoadBattlePass()
    {
        var storageList = await GameController.Instance.Client.ListUsersStorageObjectsAsync(GameController.Instance.Session, BATTLE_PASS_COLLECTION, GameController.Instance.Session.UserId);

        var battlePassDTO = LoadDatabase();

        //Reads the saved data from storage and see the user unlocks

            var battlePassModel = new BattlePassModel(battlePassDTO);

            var storageObject = storageList.Objects.ToList().Find(obj => obj.Key == battlePassDTO.Name);

            if (storageObject != null)
            {
                var entry = JsonConvert.DeserializeObject<BattlePassEntry>(storageObject.Value);

                battlePassModel.CurrentExp = entry.currentExperience;
                battlePassModel.CurrentLevel = entry.currentLevel;
                battlePassModel.PremiumUnlocked = entry.premiumUnlocked;
            }

            BattlePass = battlePassModel;
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "BattlePassLevel", battlePassModel.GetNextUnlockableLevel().ToString(), 1);
    }

    private class UnlockLevelRequest
    {
        [JsonProperty("level")]
        public int level;

        [JsonProperty("battlePassId")]
        public string battlePassId;
    }

    public class UnlockLevelResponse
    {
        [JsonProperty("message")]
        public string message;

        [JsonProperty("success")]
        public bool success;

    }

    /// <summary>
    /// Unlocks the specified level for a battle pass
    /// </summary>
    public static async Task<UnlockLevelResponse> UnlockBattlePassLevel(int level, string battlePass = "Battle Pass")
    {
        var request = new UnlockLevelRequest()
        {
            battlePassId = battlePass,
            level = level
        };

        var response = await GameController.Instance.Socket.RpcAsync(UNLOCK_BATTLE_PASS_LEVEL_RPC, JsonConvert.SerializeObject(request));
        await GameController.Instance.RefreshSession();
        await GameController.Instance.RefreshAssets();
        return JsonConvert.DeserializeObject<UnlockLevelResponse>(response.Payload);
    }
    
    
}
