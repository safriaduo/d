using System;
using Dawnshard.Menu;

public class FriendlyMatchPopup : RankedMatchPopup
{
    private bool isSender;
    private string friendId;
    private string matchId;

    public void SetChallenge(GameController.FriendChallenge challenge)
    {
        isSender = false;
        matchId = challenge.MatchId;
    }

    public void SetChallengeAsSender(string friendId)
    {
        isSender = true;
        this.friendId = friendId;
    }

    protected override async void StartAIMatchAsync()
    {
        PlayWaitingAnimation();
        try
        {
            if (isSender)
            {
                await GameController.Instance.StartFriendlyMatch(friendId, deckPresenter.Model.Name);
            }
            else
            {
                await GameController.Instance.AcceptFriendlyMatch(matchId, deckPresenter.Model.Name);
            }
            isSearchingForMatch = true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
            ShowError(e.Message);
        }
    }
}
