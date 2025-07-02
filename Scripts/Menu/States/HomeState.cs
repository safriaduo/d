using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dawnshard.Network;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace Dawnshard.Menu
{
    public class HomeState : AState
    {
        [SerializeField] private TMP_Text versionText;
        [SerializeField] private PlayerProfileUI playerProfileUI;
        [SerializeField] private PlayerStatsPopup playerStatsPopup;
        [SerializeField] private Transform dailyQuestParent;
        [SerializeField] private PopupOneButton dailyQuestCompletedPopup;
        [SerializeField] private GameObject tournamentLeaderboardButton;
        [SerializeField] private GameObject newsButton;
        [SerializeField] private GameObject battlePassNotification;
        [SerializeField] private DailyQuestView dailyQuestView;

        private class DailyQuestNotificationData
        {
            [JsonProperty("title")]
            public string title;
            [JsonProperty("amount")]
            public int amount;
            [JsonProperty("reward")]
            public string reward;
            [JsonProperty("description")]
            public string description;
        }

        private const string CARD_LAYER = "Card";

        public override void Enter(AState from)
        {
            base.Enter(from);
            playerProfileUI.CloseMenu();
            versionText.text = Application.version;
            tournamentLeaderboardButton.SetActive(false);
            newsButton.SetActive(false);
            DestroyAllCards();
            LoadUserData();
        }

        private void Start()
        {
            //LoadDecksAndSets();
            //LoadUserData();
        }

        private async void LoadUserData()
        {
            try
            {
                await GameController.Instance.RefreshAssets();
                var currencies = GameController.Instance.Wallet;
                playerStatsPopup.Initialize(currencies.ContainsKey(Constants.Debristar) ?
                    currencies[Constants.Debristar] : 0,
                    currencies.ContainsKey(Constants.Tethras) ?
                    currencies[Constants.Tethras] : 0);
                await ManageUserNotification();
                if (TournamentAPI.isWeekendTournamentActive)
                {
                    tournamentLeaderboardButton.SetActive(true);
                    newsButton.SetActive(true);
                }
                battlePassNotification.SetActive(BattlePassAPI.BattlePass.CanCollectLevel(BattlePassAPI.BattlePass.GetNextCollectableLevel()));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async Task ManageUserNotification()
        {
            var notifications = await GameController.Instance.ReadNotification();
            ClearDailyQuestsNotifications();


            int spawnedQuests = 0;
            foreach (var notification in notifications)
            {
                if (notification.Code == 2 || notification.Code == 3)
                {
                    if (spawnedQuests >= 3)
                    {
                        break;
                    }

                    dailyQuestCompletedPopup.Open();

                    //var title = notification.Code == 2 ? "Quest Completed!" : "New Quest Received!";
                    dailyQuestCompletedPopup.SetUpPopupButton(notification.Subject, onClick: dailyQuestCompletedPopup.Close, titleText: notification.Subject);

                    var questData = JsonConvert.DeserializeObject<DailyQuestNotificationData>(notification.Content);
                    SpawnQuestFromNotification(questData, notification.Code == 3);
                    spawnedQuests++;
                }
            }

            await GameController.Instance.DeleteNotification(
                notifications.ConvertAll(notification => notification.Id));
        }

        private void ClearDailyQuestsNotifications()
        {
            foreach (Transform child in dailyQuestParent)
            {
                if (child.GetComponent<DailyQuestView>() != null)
                    Destroy(child.gameObject);
            }
        }

        private void SpawnQuestFromNotification(DailyQuestNotificationData questData, bool completed = false)
        {
            var dailyQuest = Instantiate(dailyQuestView, dailyQuestParent);
            dailyQuest.transform.localScale = Vector3.one * 1.3f;
            dailyQuest.ShowDailyQuest(questData.title, questData.description, completed ? 1 : 0, 1, questData.reward, questData.amount);
        }

        public static void DestroyAllCards()
        {
            // Get the layer index for "Cards"
            int cardsLayer = LayerMask.NameToLayer(CARD_LAYER);

            // Find all active GameObjects in the scene.
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                // Check if the object's layer matches the Cards layer.
                if (obj.layer == cardsLayer)
                {
                    Destroy(obj);
                }
            }
        }
        
        public override string GetName() => Constants.HomeState;
    }
}
