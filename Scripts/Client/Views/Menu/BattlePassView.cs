using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dawnshard.Network;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class BattlePassView : MonoBehaviour
{
    [SerializeField] private LevelView levelViewPrefab;
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private LevelProgressBarView levelProgressBar;

    private List<LevelView> spawnedLevelViews = new List<LevelView>();

    private BattlePassModel battlePass;
    private LevelView prevExperienceView;

    /// <summary>
    /// Shows the UI for the specified battle pass
    /// </summary>
    public void Initialize(BattlePassModel battlePass, bool centerOnUnlockedLevel=false)
    {
        this.battlePass = battlePass;
        var levelExperiences = 0;

        for (int i = 0; i < battlePass.levels.Count; i++)
        {
            levelExperiences += battlePass.GetLevelTotalExp(battlePass.levels[i]);
        }
        levelProgressBar.BattlePassFill(battlePass.CurrentExp, battlePass.levels.ConvertAll(level=>level.LevelUpExperience).ToArray());

        DestroySpawnedViews();

        StartCoroutine(SpawnLevelViews(centerOnUnlockedLevel));
        
    }
    
    public float CalculateFillAmount(int currentLevel, float currentXP, float xpToNextLevel, int totalLevels)
    {
        // Ensure totalLevels is not zero to avoid division by zero
        if (totalLevels <= 0)
            return 0f;

        // Calculate the base fill amount based on the completed levels
        float baseFill = (float)currentLevel / totalLevels;

        // Calculate the additional fill from the current level progress
        float levelProgress = (xpToNextLevel > 0) ? (currentXP / xpToNextLevel) : 0f;

        // Total fill is the sum of the base fill and the level progress portion
        float totalFill = baseFill + (1f / totalLevels) * levelProgress;

        // Clamp the result between 0 and 1 to avoid overflow
        return Mathf.Clamp01(totalFill);
    }

    private IEnumerator SpawnLevelViews(bool centerOnUnlockedLevel)
    {
        foreach (var level in battlePass.levels)
        {
            var levelView = Instantiate(levelViewPrefab, scrollView.content);
            levelView.Initialize(level, (l) => OnCollectLevelAsync(l));
            spawnedLevelViews.Add(levelView);
        }

        yield return null;

        Refresh(centerOnUnlockedLevel);
    }

    /// <summary>
    /// Focus the scroll view on the collectable level
    /// </summary>
    public void FocusCollectableLevel()
    {
        // if (battlePass.levels.Count > battlePass.CurrentLevel && battlePass.CurrentLevel > 0)
        // {
            //scrollView.DOKill();
            
            var view = spawnedLevelViews.Find(v => v.GetBattlePassLevel == battlePass.levels[Mathf.Min(battlePass.CurrentLevel+1,49)]);
            var targetRect = view.transform as RectTransform;

            // get the rect transform of the scroll view content
            RectTransform scrollViewContent = scrollView.content;

            // calculate the normalized position of the target game object
            var i = 0;

          
            float normalizedPosition = (float)(battlePass.GetNextCollectableLevel()==null? battlePass.CurrentLevel+.5f: battlePass.levels.IndexOf(battlePass.GetNextCollectableLevel())+.5f)/battlePass.levels.Count;
            normalizedPosition=normalizedPosition>1?1:normalizedPosition;
            // set the horizontalNormalizedPosition property of the ScrollRect component to scroll to the target object
            scrollView.DOHorizontalNormalizedPos(normalizedPosition, 1f).SetEase(Ease.OutExpo);
                //.Tween(scrollView.horizontalNormalizedPosition,normalizedPosition, .3f, Ease.Cubic);
       // }
    }

    private void DestroySpawnedViews()
    {
        foreach (var view in spawnedLevelViews)
        {
            Destroy(view.gameObject);
        }

        prevExperienceView = null;

        spawnedLevelViews.Clear();
    }

    /// <summary>
    /// Updates the UI
    /// </summary>
    public void Refresh(bool centerOnUnlockedLevel)
    {
        foreach (var view in spawnedLevelViews)
        {
            int levelNumber = battlePass.levels.IndexOf(view.GetBattlePassLevel);
            LevelView.LevelState levelState = ComputeLevelState(view, levelNumber);

            view.Refresh(levelNumber, levelState);
        }

        levelProgressBar.AnimationUpdateFillAmount();

        ShowExperienceToNextLevel();

        if (centerOnUnlockedLevel)
        {
            FocusUnlockedLevel();
        }
        else
        {
            FocusCollectableLevel();
        }
    }

    private void FocusUnlockedLevel()
    {
        // get the rect transform of the scroll view content

        // calculate the normalized position of the target game object
        float normalizedPosition = (float)(battlePass.GetNextUnlockableLevel()-1)/(battlePass.levels.Count-2);

        // set the horizontalNormalizedPosition property of the ScrollRect component to scroll to the target object
        scrollView.DOHorizontalNormalizedPos(normalizedPosition, 2f).SetEase(Ease.OutExpo);
        //.Tween(scrollView.horizontalNormalizedPosition,normalizedPosition, .3f, Ease.Cubic);
        // }
    }

    private LevelView.LevelState ComputeLevelState(LevelView view, int levelNumber)
    {
        LevelView.LevelState levelState;

        if (battlePass.IsUnlockedLevel(view.GetBattlePassLevel) &&  view.GetBattlePassLevel.IsPremium && !battlePass.PremiumUnlocked)
        {
            levelState = LevelView.LevelState.PremiumUnlocked;
        }
        else if (battlePass.CurrentLevel+1 > levelNumber)
        {

            if (battlePass.IsUnlockedLevel(view.GetBattlePassLevel))
                levelState = LevelView.LevelState.Collected;

            else
                levelState = LevelView.LevelState.Locked;

        }
        else if (battlePass.CanCollectLevel(view.GetBattlePassLevel))
        {
            levelState = LevelView.LevelState.Collectable;
        }
        else if (battlePass.IsUnlockedLevel(view.GetBattlePassLevel))
        {
            levelState = LevelView.LevelState.Unlocked;
        }
        else
        {
            levelState = LevelView.LevelState.Locked;
        }

        return levelState;
    }

    private void ShowExperienceToNextLevel()
    {
        int battlePassLevel = battlePass.GetNextUnlockableLevel();

        if (battlePassLevel != -1)
        {
            var nextLevel = battlePass.levels[battlePassLevel];
            var nextLevelView = spawnedLevelViews.Find(l => l.GetBattlePassLevel == nextLevel);

            if (nextLevelView != null)
            {
                prevExperienceView?.ToggleExperienceText(false);
                nextLevelView.ToggleExperienceText(true, $"{battlePass.GetLevelTotalExp(nextLevel) - battlePass.CurrentExp} exp");
                prevExperienceView = nextLevelView;
            }
        }
    }

    private async void OnCollectLevelAsync(BattlePassLevelModel battlePassLevel)
    {
        await battlePass.LevelUpAsync(battlePassLevel);
        Refresh(false);
    }

    public void AddExperience(int experience)
    {
        levelProgressBar.BattlePassFill(battlePass.CurrentExp+experience, battlePass.levels.ConvertAll(level=>level.LevelUpExperience).ToArray());
    }

    public List<LevelView> GetLevelViews()
    {
        return spawnedLevelViews;
    }
}
