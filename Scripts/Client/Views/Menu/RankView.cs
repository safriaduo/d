using System;
using System.Collections;
using System.Collections.Generic;
using Dawnshard.Database;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RankView : MonoBehaviour
{
    [SerializeField] private LevelProgressBarView progressBarView;
    
    [SerializeField] private TMP_Text currentRankName;
    [SerializeField] private TMP_Text currentLeaderboardPosition;
    [SerializeField] private TMP_Text nextLeaderboardPosition;
    [SerializeField] private TMP_Text currentTournamentPosition;
    [SerializeField] private TMP_Text nextTournamentPosition;


    [SerializeField] private MMFeedbackTMPText nextRankNameAnimationUp;
    [SerializeField] private MMFeedbackTMPText nextRankNameAnimationDown;

    [SerializeField] private MMFeedbacks fadeInAnimation;
    [SerializeField] private MMFeedbacks fillAnimation;
    [SerializeField] private MMFeedbacks unfillAnimation;
    [SerializeField] private MMFeedbacks rankUpAnimation;
    [SerializeField] private MMFeedbacks rankDownAnimation;
    [SerializeField] private MMFeedbacks changeLeaderboardPositionDownAnimation;
    [SerializeField] private MMFeedbacks changeLeaderboardPositionUpAnimation;
    [SerializeField] private MMFeedbacks changeTournamentPositionDownAnimation;
    [FormerlySerializedAs("changeTournamentdPositionUpAnimation")] [SerializeField] private MMFeedbacks changeTournamentPositionUpAnimation;

    
    [SerializeField] private Image nextRankIcon;
    [SerializeField] private Image currentRankIcon;

    [SerializeField] private GameObject expBar;
    [SerializeField] private GameObject rankPosition;

    [SerializeField] private GameObject parent;

    private void Start()
    {
        expBar.SetActive(false);
        rankPosition.SetActive(false);
    }

    public IEnumerator PlayRankExpChangeAnimation(string currentRank, string nextRank, int currentRankLevel, int nextRankLevel, int currentExp, int gainedExp, int expToNextLevel)
    {
        if(gainedExp==0)
            yield break;
        Debug.Log("GainedExp: "+gainedExp);
        Debug.Log("current rank: "+currentRank+currentRankLevel);
        Debug.Log("next rank: "+nextRank+nextRankLevel);
        parent.SetActive(true);
        expBar.SetActive(true);
        
        SetFields(currentRank, nextRank, currentRankLevel, nextRankLevel, currentExp, gainedExp, expToNextLevel);
        
        yield return new WaitForSeconds(1f);
        var waitingTime = ChainRankUpAnimations(currentExp, gainedExp, expToNextLevel);
        fadeInAnimation.PlayFeedbacks();
        yield return new WaitForSeconds(waitingTime);
    }

    public IEnumerator PlayAscendantRankLeaderboardAnimation(int previousRankPos, int currentRankPos)
    {
        parent.SetActive(true);
        rankPosition.SetActive(true);
        
        SetFieldsAscendant(previousRankPos, currentRankPos, previousRankPos==0);

        yield return new WaitForSeconds(1f);
        var waitingTime = ChainPositionUpAnimations(previousRankPos, currentRankPos);
        fadeInAnimation.PlayFeedbacks();
        yield return new WaitForSeconds(waitingTime);    
    }

    private float ChainRankUpAnimations(int currentExp, int gainedExp, int expToNextLevel)
    {
        if (gainedExp == 0)
        {
            return fadeInAnimation.TotalDuration + 2f;
        }
        if (gainedExp > 0 && gainedExp + currentExp >= expToNextLevel)
        {
            fadeInAnimation.Events.OnComplete.AddListener(() =>
            {
                fillAnimation.PlayFeedbacks();
            });
            fillAnimation.Events.OnComplete.AddListener(() =>
            {
                rankUpAnimation.PlayFeedbacks();
            });
            return fadeInAnimation.TotalDuration+fillAnimation.TotalDuration+rankUpAnimation.TotalDuration+2f;
        }
        if(gainedExp>0)
        {
            fadeInAnimation.Events.OnComplete.AddListener(() =>
            {
                fillAnimation.PlayFeedbacks();
            });
            return fadeInAnimation.TotalDuration + fillAnimation.TotalDuration + 2f;
        }
        if (currentExp + gainedExp < 0 && gainedExp < 0)
        {
            fadeInAnimation.Events.OnComplete.AddListener(() =>
            {
                rankDownAnimation.PlayFeedbacks();
            });
            rankDownAnimation.Events.OnComplete.AddListener(() =>
            {
                unfillAnimation.PlayFeedbacks();
            });
            return fadeInAnimation.TotalDuration + unfillAnimation.TotalDuration + rankDownAnimation.TotalDuration + 2f;
        }
        if(gainedExp<0)
        {
            fadeInAnimation.Events.OnComplete.AddListener(() =>
            {
                unfillAnimation.PlayFeedbacks();
            });
            return fadeInAnimation.TotalDuration + unfillAnimation.TotalDuration + 2f;
        }

        return 0;
    }

    private void SetFields(string currentRank, string nextRank, int currentRankLevel, int nextRankLevel, int currentExp,
        int gainedExp, int expToNextLevel)
    {
        progressBarView.InitializeStandardFill(currentExp,gainedExp, expToNextLevel);
        string currentRankLevelName = "";
        string nextRankLevelName = "";
        for (int i = 0; i < currentRankLevel; i++)
            currentRankLevelName += "I";
        for (int i = 0; i < nextRankLevel; i++)
            nextRankLevelName += "I";
        currentRankName.text = currentRank+" "+currentRankLevelName;
        nextRankNameAnimationUp.NewText = nextRank+" "+nextRankLevelName;
        nextRankNameAnimationDown.NewText = nextRank+" "+nextRankLevelName;
        currentRankIcon.sprite = AssetDatabase.Instance.GetRankRecord(currentRank).rankSprite;
        nextRankIcon.sprite = AssetDatabase.Instance.GetRankRecord(nextRank).rankSprite;
    }
    
    private void SetFieldsAscendant(int startPos, int endPos, bool newOnRanked)
    {
        var currentRank = Constants.AscendantRank;

        if (newOnRanked)
        {
            currentLeaderboardPosition.text = "???";
        }
        else
        {
            currentLeaderboardPosition.text = startPos.ToString();
        }

        currentRankName.text = currentRank;
        currentRankIcon.sprite = AssetDatabase.Instance.GetRankRecord(currentRank).rankSprite;
        nextLeaderboardPosition.text = endPos.ToString();
    }

    private float ChainPositionUpAnimations(int startPos, int endPos)
    {
        if (startPos > endPos)
        {
            fadeInAnimation.Events.OnComplete.AddListener(() =>
            {
                changeLeaderboardPositionUpAnimation.PlayFeedbacks();
            });
            return fadeInAnimation.TotalDuration + changeLeaderboardPositionUpAnimation.TotalDuration + changeLeaderboardPositionUpAnimation.TotalDuration + 2f;
        }
        fadeInAnimation.Events.OnComplete.AddListener(() =>
        {
            changeLeaderboardPositionDownAnimation.PlayFeedbacks();
        });
        return fadeInAnimation.TotalDuration + changeLeaderboardPositionDownAnimation.TotalDuration + 2f;
    }

    public IEnumerator PlayTournamentLeaderboardAnimation(int previousWin, int previousLoss, int nextWin, int nextLoss)
    {
        parent.SetActive(true);
        
        SetFieldsTournament(previousWin,
            previousLoss,
            nextWin,
            nextLoss);

        yield return new WaitForSeconds(1f);
        bool hasPlayerWon = previousWin < nextWin;
        var waitingTime = ChainTournamentPositionChangeAnimations(hasPlayerWon);
        if(hasPlayerWon)
            changeTournamentPositionUpAnimation.PlayFeedbacks();
        else
        {
            changeTournamentPositionDownAnimation.PlayFeedbacks();
        }
        yield return new WaitForSeconds(waitingTime);  
    }

    private float ChainTournamentPositionChangeAnimations(bool hasPlayerWon)
    {
        if (hasPlayerWon)
        {
            return changeTournamentPositionUpAnimation.TotalDuration + 2f;
        }
        else
        {
            return changeTournamentPositionDownAnimation.TotalDuration + 2f;
        }
    }

    private void SetFieldsTournament(int previousWin, int previousLoss, int nextWin, int nextLoss)
    {
        currentTournamentPosition.text = $"{previousWin}W-{previousLoss}L";
        nextTournamentPosition.text = $"{nextWin}W-{nextLoss}L";
    }
}
