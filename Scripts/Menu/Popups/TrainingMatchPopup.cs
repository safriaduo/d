using System;
using Dawnshard.Menu;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Dawnshard.Menu
{
    public class TrainingMatchPopup : MatchPopup
    {
        protected override async void StartMatchAsync()
        {
            PlayWaitingAnimation();
            try
            {
                await GameController.Instance.StartAIMatch(deckPresenter.Model.Name, false);
                isSearchingForMatch = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowError(e.Message);
            }
        }
    }
}
