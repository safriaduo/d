using System.Collections.Generic;
using Dawnshard.Network;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBarView : MonoBehaviour
{
    public Image fillImage;

    private float start;
    private float end;

    private float fill = 0;
    
    public void InitializeStandardFill(float start, float fill, float end)
    {
        this.start = start;
        this.end = end;
        fillImage.fillAmount = start/end;
        if (start + fill < 0)
        {
            this.fill = (end + fill) / end;
        }
        else
        {
            this.fill = (start + fill) / (end);
        }
    }
    
    public void BattlePassFill(float currentExp, int[] xpPerLevel)
    {
        int totalLevels = xpPerLevel.Length;
        float remainingExp = currentExp;
        int completedLevels = 0;

        // Loop through each level's XP requirement.
        for (int i = 0; i < totalLevels; i++)
        {
            // If the player has more XP than needed for this level,
            // consider this level as fully complete.
            if (remainingExp >= xpPerLevel[i])
            {
                completedLevels++;
                remainingExp -= xpPerLevel[i];
            }
            else
            {
                // The player is in the middle of this level.
                float fractionOfCurrentLevel = remainingExp / xpPerLevel[i];
                // Each level contributes an equal fraction (1/totalLevels) to the bar.
                this.fill = ((float)completedLevels-1 + fractionOfCurrentLevel) / (totalLevels-1);
                fill= fill < 0 ? 0 : fill;
                fill= fill > 1 ? 1 : fill;
                return;
            }
        }

        // If the currentExp is equal to or exceeds the sum of all xpPerLevel (i.e. max exp), the bar is full.
        this.fill = 1f;
    }

    public void NewFill(float fill)
    {
        this.fill += fill / end;
        AnimationUpdateFillAmount();
    }

    public void AnimationUpdateFillAmount()
    {
        fillImage.DOFillAmount(fill, 1f);
    }

    public void SwapFill()
    {
        fillImage.fillAmount = Mathf.Abs(1 - fillImage.fillAmount);
    }

    /*public void UpdateFillAmount(Ease ease)
    {
        if (nextLevel == -1)
        {
            fillImage.fillAmount = 1f;
            return;
        }

        fillImage.DOKill();

        int pastLevel = (nextLevel - 1);

        if (pastLevel < 0) { pastLevel = 0; }

        int totalSpaces = levelExperiences.Length - 1; //There are 1 space less - 1 and 50 levels are not connected
        float minT = 0;
        float maxT = 1;
        if (totalSpaces != 0)
        {
            minT = (float)pastLevel / totalSpaces;
            maxT = (float)nextLevel / totalSpaces;
        }
        

        // Update the fill amount of the image
        float differenceNormalized = (currentLevelExperience - levelExperiences[pastLevel]) / (levelExperiences[nextLevel] - levelExperiences[pastLevel]);

        var fill = Mathf.Lerp(minT, maxT, differenceNormalized);
        //prevTween = fillImage.fillAmount.Tween(.6f, .3f, Ease.Linear);
        //fillImage.fillAmount = fill;
        fillImage.DOFillAmount(fill, 2f).SetEase(ease);
    }*/

}