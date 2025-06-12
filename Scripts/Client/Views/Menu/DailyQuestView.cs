using Dawnshard.Database;
using Dawnshard.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This class manages the UI representation of a Daily Quest
public class DailyQuestView : MonoBehaviour
{
    // UI fields serialized for easy linking from the inspector
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private Image rewardImage;
    [SerializeField] private TMP_Text rewardAmountText;
    [SerializeField] private TMP_Text currentProgressText;
    [SerializeField] private TMP_Text maxProgressText;
    [SerializeField] private Button changeQuestButton;

    private string questId;

    private void Awake()
    {
        changeQuestButton.onClick.AddListener(ChangeDailyQuest);
        changeQuestButton.gameObject.SetActive(false);
    }

    // Method to set up the quest view with provided data
    public void ShowDailyQuest(string title, string description, int currentProgress, int maxProgress, string rewardId, int rewardAmount)
    {
        // Set title and description
        titleText.text = title;
        descriptionText.text = description;

        // Clamp progress between 0 and max progress to avoid issues
        float progressFraction = Mathf.Clamp01((float)currentProgress / maxProgress);

        // Set progress bar fill amount
        progressBarFill.fillAmount = progressFraction;
        maxProgressText.text = maxProgress.ToString();
        currentProgressText.text = currentProgress.ToString();


        // Load and set reward image based on the reward Id
        rewardImage.sprite = AssetDatabase.Instance.GetRewardRecord(rewardId)?.rewardIcon;

        // Set reward amount
        if (rewardAmount > 1)
        {
            rewardAmountText.text = $"<size=20>x</size>{rewardAmount}";
        }
        else
        {
            rewardAmountText.gameObject.SetActive(false);
        }
    }

    public void EnableChangeQuestButton(string id)
    {
        questId = id;
        changeQuestButton.gameObject.SetActive(true);
    }

    public async void ChangeDailyQuest()
    {
        var newQuest = await DailyQuestAPI.ChangeDailyQuest(questId);

        ShowDailyQuest(newQuest.Title, newQuest.Description, newQuest.CurrentProgress, newQuest.MaxProgress, newQuest.Reward.RewardId, newQuest.Reward.Amount);
    }
}