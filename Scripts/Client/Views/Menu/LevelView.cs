using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoLetterbox;
using Dawnshard.Database;
using Dawnshard.Menu;
using Dawnshard.Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] protected GameObject premiumParent;
    [SerializeField] protected GameObject freeParent;
    [SerializeField] protected TMP_Text nextLevelExperienceText;
    [SerializeField] protected Image rewardImage;
    [SerializeField] protected TMP_Text amountText;
    [SerializeField] protected TMP_Text levelText;
    [SerializeField] protected Button collectButton;
    [SerializeField] protected Animator animator;
    [SerializeField] protected MMFeedbacks collectFeedbacks;
    [SerializeField] protected StateByParent[] stateByParents;
    [SerializeField] private Button buyPremiumButton;
    [SerializeField] private ConfirmPopup goToStoreConfirmPopup;
    [SerializeField] private GameObject userIdCopy;

    
    private LevelState levelState;

    private static readonly int lockedTrigger = Animator.StringToHash("Locked");
    private static readonly int collectableTrigger = Animator.StringToHash("Collectable");
    private static readonly int collectedTrigger = Animator.StringToHash("Collected");
    private static readonly int unlockedTrigger = Animator.StringToHash("Unlocked");
    


    public BattlePassLevelModel GetBattlePassLevel { get; private set; }

    public Action<BattlePassLevelModel> CollectCallback { get; set; }

    private void Awake()
    {
        collectButton.onClick.AddListener(CollectReward);
    }

    public void Initialize(BattlePassLevelModel battlePassLevel, Action<BattlePassLevelModel> collectAction)
    {
        this.GetBattlePassLevel = battlePassLevel;
        CollectCallback = collectAction;
    }

    private void Start()
    {
    }
    
    

    public void Refresh(int level, LevelState levelState)
    {
        this.levelState = levelState;

        UpdateLevelState();

        UpdateUI(level);
    }

    private void UpdateUI(int level)
    {
        levelText.text = (level+1).ToString();
        premiumParent.SetActive(GetBattlePassLevel.IsPremium);
        freeParent.SetActive(!GetBattlePassLevel.IsPremium);

        RewardRecord reward = AssetDatabase.Instance.GetRewardRecord(GetBattlePassLevel.Reward.RewardId);
        rewardImage.sprite = reward.rewardIcon;
        amountText.text = $"+{GetBattlePassLevel.Reward.Amount} {GetBattlePassLevel.Reward.RewardId.Replace("_"," ")}";
    }

    public void SetPremiumButton(Action onButtonPressed)
    {
        buyPremiumButton.onClick.RemoveAllListeners();
        buyPremiumButton.onClick.AddListener(()=>onButtonPressed());
    }

    private void UpdateLevelState()
    {
        switch (levelState)
        {
            case LevelState.Locked:
                collectButton.interactable = false;
                animator.SetTrigger(lockedTrigger);
                break;

            case LevelState.Unlocked:
                collectButton.interactable = false;
                animator.SetTrigger(unlockedTrigger);
                break;

            case LevelState.Collectable:
                animator.SetTrigger(collectableTrigger);
                collectButton.interactable = true;
                break;

            case LevelState.Collected:
                collectButton.interactable = false;
                animator.SetTrigger(collectedTrigger);
                break;
            case LevelState.PremiumUnlocked:
                collectButton.interactable = false;
                break;
            default:
                throw new ArgumentException();
        }

        foreach (var stateByParent in stateByParents)
        {
            stateByParent.parent.SetActive(stateByParent.levelState == levelState);
        }
    }

    private void CollectReward()
    {
        collectFeedbacks.PlayFeedbacks();
        CollectCallback?.Invoke(GetBattlePassLevel);
    }

    //Method to toggle the visibility of exp next level text
    public void ToggleExperienceText(bool active, string text = "")
    {
        nextLevelExperienceText.text = text;
        nextLevelExperienceText.gameObject.SetActive(active);
    }

    public enum LevelState
    {
        Locked,
        Unlocked,
        Collectable,
        Collected,
        PremiumUnlocked,
    }

    [Serializable]
    public class StateByParent
    {
        public GameObject parent;
        public LevelState levelState;
    }
}
