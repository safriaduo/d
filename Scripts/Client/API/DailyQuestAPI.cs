using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

namespace Dawnshard.Network
{
    public static class DailyQuestAPI
    {
        private const string DAILY_QUEST_PATH = "DailyQuest/daily_quests";
        private const string DAILY_QUEST_RPC_CALL = "get_daily_quests";
        private const string CHANGE_DAILY_QUEST_RPC_CALL = "change_daily_quest";

        public class ChangeDailyQuestPayload
        {
            [JsonProperty("QuestId")] public string QuestId;
        }

        public class UserQuestProgressModel
        {
            [JsonProperty("id")] public string Id;
            [JsonProperty("currentProgress")] public int CurrentProgress;
        }

        public class UserQuestProgressStruct
        {
            [JsonProperty("quests")] public UserQuestProgressModel[] quests;
            [JsonProperty("lastClaimTime")] int lastClaimTime;
            [JsonProperty("hasChangedQuest")] public bool hasChangedQuest;
        }


        private static Dictionary<string, DailyQuestModel> _dailyQuestDict;
        public static Dictionary<string, DailyQuestModel> userDailyQuest;
        public static bool HasChangedQuests { get; private set; }


        // Load quests once
        public static void ReadQuestsFromJson()
        {
            var dailyQuestFile = Resources.Load<TextAsset>(DAILY_QUEST_PATH);
            var dailyQuestList = JsonConvert.DeserializeObject<List<DailyQuestModel>>(dailyQuestFile.text);
            _dailyQuestDict = dailyQuestList.ToDictionary(q => q.Id, q => q);
            return;
        }

        // Merge progress into quests and return ready-to-use list
        public static async Task GetUserDailyQuests()
        {

            var storageObjectRead = new StorageObjectId()
            {
                Collection = "rewards",
                Key = "daily_quests",
                UserId = GameController.Instance.Session.UserId,
            };
            await GameController.Instance.Socket.RpcAsync(DAILY_QUEST_RPC_CALL);
            var userStorageQuestStruct = new UserQuestProgressStruct();
            try
            {
                var storageList = await GameController.Instance.ReadStorageObject(new IApiReadStorageObjectId[1]
                    { storageObjectRead });
                var userDataObject = storageList.Objects.Single();
                userStorageQuestStruct = JsonConvert.DeserializeObject<UserQuestProgressStruct>(userDataObject.Value);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            HasChangedQuests = userStorageQuestStruct.hasChangedQuest;
            var userQuestList = new List<DailyQuestModel>();

            foreach (var progressEntry in userStorageQuestStruct.quests)
            {
                if (_dailyQuestDict.TryGetValue(progressEntry.Id, out var quest))
                {
                    // Clone quest data to avoid modifying static data accidentally
                    var userQuest = new DailyQuestModel
                    {
                        Id = quest.Id,
                        Title = quest.Title,
                        Description = quest.Description,
                        Type = quest.Type,
                        MaxProgress = quest.MaxProgress,
                        Reward = quest.Reward,
                        CurrentProgress = progressEntry.CurrentProgress
                    };

                    userQuestList.Add(userQuest);
                }
                else
                {
                    Debug.LogError($"Quest ID '{progressEntry.Id}' from user metadata not found in quest definitions!");
                }
            }

            userDailyQuest = userQuestList.ToDictionary(q => q.Id, q => q);

        }

        public static async Task<DailyQuestModel> ChangeDailyQuest(string questId)
        {
            var task = await GameController.Instance.Socket.RpcAsync(CHANGE_DAILY_QUEST_RPC_CALL, JsonConvert.SerializeObject(new ChangeDailyQuestPayload() { QuestId = questId }));

            var newQuest = JsonConvert.DeserializeObject<UserQuestProgressModel>(task.Payload);

            if (_dailyQuestDict.TryGetValue(newQuest.Id, out var quest))
            {
                HasChangedQuests = true;

                // Clone quest data to avoid modifying static data accidentally
                return new DailyQuestModel
                {
                    Id = quest.Id,
                    Title = quest.Title,
                    Description = quest.Description,
                    Type = quest.Type,
                    MaxProgress = quest.MaxProgress,
                    Reward = quest.Reward,
                };

            }
            else
            {
                Debug.LogError($"Quest ID '{newQuest.Id}' from user metadata not found in quest definitions!");
                return null;
            }
        }
    }

}

