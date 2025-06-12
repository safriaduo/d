using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawnshard.Network;
using GameAnalyticsSDK.Setup;
using Nakama;
using Newtonsoft.Json;
using UnityEngine;

public class TutorialStorageAPI {
    public class TutorialStorage
    {
        [JsonProperty("already_registered")] public bool AlreadyRegistered { get; set; } = false;

        [JsonProperty("all_quests_completed")]
        public bool AllQuestsCompleted { get; set; } = false;
        [JsonProperty("first_deck_created")]
        public bool FirstDeckCreated { get; set; } = false;
        [JsonProperty("launch_arena_key")]
        public bool LaunchArenaKey { get; set; } = false;
        [JsonProperty("reward_for_step")]
        public int RewardForStep { get; set; } = 0;
    }
    
    private const string TUTORIAL_COLLECTION = "tutorial";

    public static TutorialStorage TutorialStoragePlayer { get; private set; } = new TutorialStorage();

    
    public static async Task SaveTutorialStorage(bool alreadyRegistered=false, bool launchArenaKey=false, int rewardForStep=0, bool firstDeckCreated=false, bool firstQuestCompleted=false)
    {
        var request = new TutorialStorage()
        {
            AlreadyRegistered = alreadyRegistered,
            AllQuestsCompleted = firstQuestCompleted,
            FirstDeckCreated = firstDeckCreated,
            LaunchArenaKey = launchArenaKey,
            RewardForStep = rewardForStep
        };
        IApiWriteStorageObject[] storageObjects = new IApiWriteStorageObject[1];
        Nakama.WriteStorageObject storageObject = new WriteStorageObject();

        storageObject.Value = JsonConvert.SerializeObject(request);
        storageObject.Key = GameController.Instance.Session.UserId;
        storageObject.Collection = TUTORIAL_COLLECTION;
        storageObjects[0] = storageObject;

        await GameController.Instance.WriteStorageObject(storageObjects);

        TutorialStoragePlayer = request;
    }
    
    public static async Task ReadTutorialStorage()
    {
        {
            var storageObjectRead = new StorageObjectId()
            {
                Collection = TUTORIAL_COLLECTION,
                Key = GameController.Instance.Session.UserId,
                UserId = GameController.Instance.Session.UserId,
            };

            try
            {
                var storageList = await GameController.Instance.ReadStorageObject(new IApiReadStorageObjectId[1]
                    { storageObjectRead });
                if(storageList == null || !storageList.Objects.Any()) return;
                var userDataObject = storageList.Objects.Single();
                var tutorial = JsonConvert.DeserializeObject<TutorialStorage>(userDataObject.Value);
                TutorialStoragePlayer =  tutorial;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }


        }
    }

    
}
